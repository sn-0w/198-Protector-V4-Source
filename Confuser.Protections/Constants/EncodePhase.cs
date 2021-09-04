using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000AE RID: 174
	internal class EncodePhase : ProtectionPhase
	{
		// Token: 0x060002EF RID: 751 RVA: 0x00002E01 File Offset: 0x00001001
		public EncodePhase(ConstantProtection parent) : base(parent)
		{
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x000030FF File Offset: 0x000012FF
		public override string Name
		{
			get
			{
				return "Constants encoding";
			}
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00014448 File Offset: 0x00012648
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			CEContext moduleCtx = context.Annotations.Get<CEContext>(context.CurrentModule, ConstantProtection.ContextKey, null);
			if (parameters.Targets.Any<IDnlibDef>() && moduleCtx != null)
			{
				Dictionary<object, List<Tuple<MethodDef, Instruction>>> dictionary = new Dictionary<object, List<Tuple<MethodDef, Instruction>>>();
				Dictionary<byte[], List<Tuple<MethodDef, Instruction>>> dictionary2 = new Dictionary<byte[], List<Tuple<MethodDef, Instruction>>>(new EncodePhase.ByteArrayComparer());
				this.ExtractConstants(context, parameters, moduleCtx, dictionary, dictionary2);
				moduleCtx.ReferenceRepl = new Dictionary<MethodDef, List<Tuple<Instruction, uint, IMethod>>>();
				moduleCtx.EncodedBuffer = new List<uint>();
				foreach (KeyValuePair<byte[], List<Tuple<MethodDef, Instruction>>> keyValuePair in dictionary2.WithProgress(context.Logger))
				{
					this.EncodeInitializer(moduleCtx, keyValuePair.Key, keyValuePair.Value);
					context.CheckCancellation();
				}
				foreach (KeyValuePair<object, List<Tuple<MethodDef, Instruction>>> keyValuePair2 in dictionary.WithProgress(context.Logger))
				{
					if (keyValuePair2.Key is string)
					{
						this.EncodeString(moduleCtx, (string)keyValuePair2.Key, keyValuePair2.Value);
					}
					else if (keyValuePair2.Key is int)
					{
						this.EncodeConstant32(moduleCtx, (uint)((int)keyValuePair2.Key), context.CurrentModule.CorLibTypes.Int32, keyValuePair2.Value);
					}
					else if (keyValuePair2.Key is long)
					{
						this.EncodeConstant64(moduleCtx, (uint)((long)keyValuePair2.Key >> 32), (uint)((long)keyValuePair2.Key), context.CurrentModule.CorLibTypes.Int64, keyValuePair2.Value);
					}
					else if (keyValuePair2.Key is float)
					{
						EncodePhase.RTransform rtransform = default(EncodePhase.RTransform);
						rtransform.R4 = (float)keyValuePair2.Key;
						this.EncodeConstant32(moduleCtx, rtransform.Lo, context.CurrentModule.CorLibTypes.Single, keyValuePair2.Value);
					}
					else
					{
						if (!(keyValuePair2.Key is double))
						{
							throw new UnreachableException();
						}
						EncodePhase.RTransform rtransform2 = default(EncodePhase.RTransform);
						rtransform2.R8 = (double)keyValuePair2.Key;
						this.EncodeConstant64(moduleCtx, rtransform2.Hi, rtransform2.Lo, context.CurrentModule.CorLibTypes.Double, keyValuePair2.Value);
					}
					context.CheckCancellation();
				}
				ReferenceReplacer.ReplaceReference(moduleCtx, parameters);
				byte[] array = new byte[moduleCtx.EncodedBuffer.Count * 4];
				int i = 0;
				foreach (uint num in moduleCtx.EncodedBuffer)
				{
					array[i++] = (byte)(num & 255U);
					array[i++] = (byte)(num >> 8 & 255U);
					array[i++] = (byte)(num >> 16 & 255U);
					array[i++] = (byte)(num >> 24 & 255U);
				}
				array = context.Registry.GetService<ICompressionService>().Compress(array, null);
				context.CheckCancellation();
				uint[] array2 = new uint[(array.Length + 3) / 4 + 15 & -16];
				Buffer.BlockCopy(array, 0, array2, 0, array.Length);
				uint num2 = moduleCtx.Random.NextUInt32();
				uint[] array3 = new uint[16];
				uint num3 = num2;
				for (int j = 0; j < 16; j++)
				{
					num3 ^= num3 >> 12;
					num3 ^= num3 << 25;
					num3 ^= num3 >> 27;
					array3[j] = num3;
				}
				byte[] array4 = new byte[array2.Length * 4];
				for (i = 0; i < array2.Length; i += 16)
				{
					uint[] src = moduleCtx.ModeHandler.Encrypt(array2, i, array3);
					for (int k = 0; k < 16; k++)
					{
						array3[k] ^= array2[i + k];
					}
					Buffer.BlockCopy(src, 0, array4, i * 4, 64);
				}
				moduleCtx.DataField.InitialValue = array4;
				moduleCtx.DataField.HasFieldRVA = true;
				moduleCtx.DataType.ClassLayout = new ClassLayoutUser(0, (uint)array4.Length);
				MutationHelper.InjectKeys(moduleCtx.InitMethod, new int[]
				{
					0,
					1
				}, new int[]
				{
					array4.Length / 4,
					(int)num2
				});
				MutationHelper.ReplacePlaceholder(moduleCtx.InitMethod, delegate(Instruction[] arg)
				{
					List<Instruction> list = new List<Instruction>();
					list.AddRange(arg);
					list.Add(Instruction.Create(OpCodes.Dup));
					list.Add(Instruction.Create(OpCodes.Ldtoken, moduleCtx.DataField));
					list.Add(Instruction.Create(OpCodes.Call, moduleCtx.Module.Import(typeof(RuntimeHelpers).GetMethod("InitializeArray"))));
					return list.ToArray();
				});
			}
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00014978 File Offset: 0x00012B78
		private void EncodeString(CEContext moduleCtx, string value, List<Tuple<MethodDef, Instruction>> references)
		{
			int buffIndex = this.EncodeByteArray(moduleCtx, Encoding.UTF8.GetBytes(value));
			this.UpdateReference(moduleCtx, moduleCtx.Module.CorLibTypes.String, references, buffIndex, (DecoderDesc desc) => desc.StringID);
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x000149D0 File Offset: 0x00012BD0
		private void EncodeConstant32(CEContext moduleCtx, uint value, TypeSig valueType, List<Tuple<MethodDef, Instruction>> references)
		{
			int num = moduleCtx.EncodedBuffer.IndexOf(value);
			if (num == -1)
			{
				num = moduleCtx.EncodedBuffer.Count;
				moduleCtx.EncodedBuffer.Add(value);
			}
			this.UpdateReference(moduleCtx, valueType, references, num, (DecoderDesc desc) => desc.NumberID);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00014A34 File Offset: 0x00012C34
		private void EncodeConstant64(CEContext moduleCtx, uint hi, uint lo, TypeSig valueType, List<Tuple<MethodDef, Instruction>> references)
		{
			int num = -1;
			do
			{
				num = moduleCtx.EncodedBuffer.IndexOf(lo, num + 1);
			}
			while ((num + 1 >= moduleCtx.EncodedBuffer.Count || moduleCtx.EncodedBuffer[num + 1] != hi) && num >= 0);
			if (num == -1)
			{
				num = moduleCtx.EncodedBuffer.Count;
				moduleCtx.EncodedBuffer.Add(lo);
				moduleCtx.EncodedBuffer.Add(hi);
			}
			this.UpdateReference(moduleCtx, valueType, references, num, (DecoderDesc desc) => desc.NumberID);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00014AD4 File Offset: 0x00012CD4
		private void EncodeInitializer(CEContext moduleCtx, byte[] init, List<Tuple<MethodDef, Instruction>> references)
		{
			int num = -1;
			foreach (Tuple<MethodDef, Instruction> tuple in references)
			{
				IList<Instruction> instructions = tuple.Item1.Body.Instructions;
				int num2 = instructions.IndexOf(tuple.Item2);
				if (num == -1)
				{
					num = this.EncodeByteArray(moduleCtx, init);
				}
				Tuple<MethodDef, DecoderDesc> tuple2 = moduleCtx.Decoders[moduleCtx.Random.NextInt32(moduleCtx.Decoders.Count)];
				uint num3 = (uint)(num | (int)tuple2.Item2.InitializerID << 30);
				num3 = moduleCtx.ModeHandler.Encode(tuple2.Item2.Data, moduleCtx, num3);
				instructions[num2 - 4].Operand = (int)num3;
				instructions[num2 - 3].OpCode = OpCodes.Call;
				SZArraySig arg = new SZArraySig(((ITypeDefOrRef)instructions[num2 - 3].Operand).ToTypeSig(true));
				instructions[num2 - 3].Operand = new MethodSpecUser(tuple2.Item1, new GenericInstMethodSig(arg));
				instructions.RemoveAt(num2 - 2);
				instructions.RemoveAt(num2 - 2);
				instructions.RemoveAt(num2 - 2);
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00014C30 File Offset: 0x00012E30
		private int EncodeByteArray(CEContext moduleCtx, byte[] buff)
		{
			int count = moduleCtx.EncodedBuffer.Count;
			moduleCtx.EncodedBuffer.Add((uint)buff.Length);
			int num = buff.Length / 4;
			int num2 = buff.Length % 4;
			for (int i = 0; i < num; i++)
			{
				uint item = (uint)((int)buff[i * 4] | (int)buff[i * 4 + 1] << 8 | (int)buff[i * 4 + 2] << 16 | (int)buff[i * 4 + 3] << 24);
				moduleCtx.EncodedBuffer.Add(item);
			}
			if (num2 > 0)
			{
				int num3 = num * 4;
				uint num4 = 0U;
				for (int j = 0; j < num2; j++)
				{
					num4 |= (uint)((uint)buff[num3 + j] << j * 8);
				}
				moduleCtx.EncodedBuffer.Add(num4);
			}
			return count;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00014CE4 File Offset: 0x00012EE4
		private void UpdateReference(CEContext moduleCtx, TypeSig valueType, List<Tuple<MethodDef, Instruction>> references, int buffIndex, Func<DecoderDesc, byte> typeID)
		{
			foreach (Tuple<MethodDef, Instruction> tuple in references)
			{
				Tuple<MethodDef, DecoderDesc> tuple2 = moduleCtx.Decoders[moduleCtx.Random.NextInt32(moduleCtx.Decoders.Count)];
				uint num = (uint)(buffIndex | (int)typeID(tuple2.Item2) << 30);
				num = moduleCtx.ModeHandler.Encode(tuple2.Item2.Data, moduleCtx, num);
				MethodSpecUser item = new MethodSpecUser(tuple2.Item1, new GenericInstMethodSig(valueType));
				moduleCtx.ReferenceRepl.AddListEntry(tuple.Item1, Tuple.Create<Instruction, uint, IMethod>(tuple.Item2, num, item));
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00014DB4 File Offset: 0x00012FB4
		private void RemoveDataFieldRefs(ConfuserContext context, HashSet<FieldDef> dataFields, HashSet<Instruction> fieldRefs)
		{
			foreach (TypeDef typeDef in context.CurrentModule.GetTypes())
			{
				foreach (MethodDef methodDef in from m in typeDef.Methods
				where m.HasBody
				select m)
				{
					foreach (Instruction instruction in methodDef.Body.Instructions)
					{
						if (instruction.Operand is FieldDef && !fieldRefs.Contains(instruction))
						{
							dataFields.Remove((FieldDef)instruction.Operand);
						}
					}
				}
			}
			foreach (FieldDef fieldDef in dataFields)
			{
				fieldDef.DeclaringType.Fields.Remove(fieldDef);
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00014F10 File Offset: 0x00013110
		private void ExtractConstants(ConfuserContext context, ProtectionParameters parameters, CEContext moduleCtx, Dictionary<object, List<Tuple<MethodDef, Instruction>>> ldc, Dictionary<byte[], List<Tuple<MethodDef, Instruction>>> ldInit)
		{
			HashSet<FieldDef> hashSet = new HashSet<FieldDef>();
			HashSet<Instruction> hashSet2 = new HashSet<Instruction>();
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				if (methodDef.HasBody)
				{
					moduleCtx.Elements = (EncodeElements)0;
					string parameter = parameters.GetParameter<string>(context, methodDef, "elements", "SI");
					int j = 0;
					while (j < parameter.Length)
					{
						char c = parameter[j];
						if (c <= 'S')
						{
							if (c <= 'N')
							{
								if (c == 'I')
								{
									goto IL_F0;
								}
								if (c == 'N')
								{
									goto IL_D0;
								}
							}
							else
							{
								if (c == 'P')
								{
									goto IL_E0;
								}
								if (c == 'S')
								{
									goto IL_C0;
								}
							}
						}
						else if (c <= 'n')
						{
							if (c == 'i')
							{
								goto IL_F0;
							}
							if (c == 'n')
							{
								goto IL_D0;
							}
						}
						else
						{
							if (c == 'p')
							{
								goto IL_E0;
							}
							if (c == 's')
							{
								goto IL_C0;
							}
						}
						IL_B8:
						j++;
						continue;
						IL_C0:
						moduleCtx.Elements |= EncodeElements.Strings;
						goto IL_B8;
						IL_D0:
						moduleCtx.Elements |= EncodeElements.Numbers;
						goto IL_B8;
						IL_E0:
						moduleCtx.Elements |= EncodeElements.Primitive;
						goto IL_B8;
						IL_F0:
						moduleCtx.Elements |= EncodeElements.Initializers;
						goto IL_B8;
					}
					if (moduleCtx.Elements != (EncodeElements)0)
					{
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							bool flag = false;
							if (instruction.OpCode == OpCodes.Ldstr && (moduleCtx.Elements & EncodeElements.Strings) > (EncodeElements)0)
							{
								if (string.IsNullOrEmpty((string)instruction.Operand) && (moduleCtx.Elements & EncodeElements.Primitive) == (EncodeElements)0)
								{
									continue;
								}
								flag = true;
							}
							else if (instruction.OpCode == OpCodes.Call && (moduleCtx.Elements & EncodeElements.Initializers) > (EncodeElements)0)
							{
								IMethod method = (IMethod)instruction.Operand;
								if (method.DeclaringType.DefinitionAssembly.IsCorLib() && method.DeclaringType.Namespace == "System.Runtime.CompilerServices" && method.DeclaringType.Name == "RuntimeHelpers" && method.Name == "InitializeArray")
								{
									IList<Instruction> instrs = methodDef.Body.Instructions;
									int i = instrs.IndexOf(instruction);
									if (instrs[i - 1].OpCode != OpCodes.Ldtoken || instrs[i - 2].OpCode != OpCodes.Dup || instrs[i - 3].OpCode != OpCodes.Newarr || instrs[i - 4].OpCode != OpCodes.Ldc_I4)
									{
										continue;
									}
									FieldDef fieldDef = instrs[i - 1].Operand as FieldDef;
									if (fieldDef == null || !fieldDef.HasFieldRVA || fieldDef.InitialValue == null)
									{
										continue;
									}
									int num = (int)instrs[i - 4].Operand;
									if (ldc.ContainsKey(num))
									{
										List<Tuple<MethodDef, Instruction>> list = ldc[num];
										list.RemoveWhere((Tuple<MethodDef, Instruction> entry) => entry.Item2 == instrs[i - 4]);
										if (list.Count == 0)
										{
											ldc.Remove(num);
										}
									}
									hashSet.Add(fieldDef);
									hashSet2.Add(instrs[i - 1]);
									byte[] array = new byte[fieldDef.InitialValue.Length + 4];
									array[0] = (byte)num;
									array[1] = (byte)(num >> 8);
									array[2] = (byte)(num >> 16);
									array[3] = (byte)(num >> 24);
									Buffer.BlockCopy(fieldDef.InitialValue, 0, array, 4, fieldDef.InitialValue.Length);
									ldInit.AddListEntry(array, Tuple.Create<MethodDef, Instruction>(methodDef, instruction));
								}
							}
							else if ((moduleCtx.Elements & EncodeElements.Numbers) > (EncodeElements)0)
							{
								if (instruction.OpCode == OpCodes.Ldc_I4)
								{
									int num2 = (int)instruction.Operand;
									if (num2 >= -1 && num2 <= 8 && (moduleCtx.Elements & EncodeElements.Primitive) == (EncodeElements)0)
									{
										continue;
									}
									flag = true;
								}
								else if (instruction.OpCode == OpCodes.Ldc_I8)
								{
									long num3 = (long)instruction.Operand;
									if (num3 >= -1L && num3 <= 1L && (moduleCtx.Elements & EncodeElements.Primitive) == (EncodeElements)0)
									{
										continue;
									}
									flag = true;
								}
								else if (instruction.OpCode == OpCodes.Ldc_R4)
								{
									float num4 = (float)instruction.Operand;
									if ((num4 == -1f || num4 == 0f || num4 == 1f) && (moduleCtx.Elements & EncodeElements.Primitive) == (EncodeElements)0)
									{
										continue;
									}
									flag = true;
								}
								else if (instruction.OpCode == OpCodes.Ldc_R8)
								{
									double num5 = (double)instruction.Operand;
									if ((num5 == -1.0 || num5 == 0.0 || num5 == 1.0) && (moduleCtx.Elements & EncodeElements.Primitive) == (EncodeElements)0)
									{
										continue;
									}
									flag = true;
								}
							}
							if (flag)
							{
								ldc.AddListEntry(instruction.Operand, Tuple.Create<MethodDef, Instruction>(methodDef, instruction));
							}
						}
						context.CheckCancellation();
					}
				}
			}
			this.RemoveDataFieldRefs(context, hashSet, hashSet2);
		}

		// Token: 0x020000AF RID: 175
		private class ByteArrayComparer : IEqualityComparer<byte[]>
		{
			// Token: 0x060002FB RID: 763 RVA: 0x00003106 File Offset: 0x00001306
			public bool Equals(byte[] x, byte[] y)
			{
				return x.SequenceEqual(y);
			}

			// Token: 0x060002FC RID: 764 RVA: 0x00015510 File Offset: 0x00013710
			public int GetHashCode(byte[] obj)
			{
				int num = 31;
				foreach (byte b in obj)
				{
					num = num * 17 + (int)b;
				}
				return num;
			}
		}

		// Token: 0x020000B0 RID: 176
		[StructLayout(LayoutKind.Explicit)]
		private struct RTransform
		{
			// Token: 0x04000160 RID: 352
			[FieldOffset(0)]
			public float R4;

			// Token: 0x04000161 RID: 353
			[FieldOffset(0)]
			public double R8;

			// Token: 0x04000162 RID: 354
			[FieldOffset(4)]
			public readonly uint Hi;

			// Token: 0x04000163 RID: 355
			[FieldOffset(0)]
			public readonly uint Lo;
		}
	}
}
