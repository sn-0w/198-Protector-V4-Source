using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000FE RID: 254
	internal class JITMode : IModeHandler
	{
		// Token: 0x06000435 RID: 1077 RVA: 0x0001B0CC File Offset: 0x000192CC
		public void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.context = context;
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator(parent.FullId);
			this.z = this.random.NextUInt32();
			this.x = this.random.NextUInt32();
			this.c = this.random.NextUInt32();
			this.v = this.random.NextUInt32();
			this._sectionName = new ValueTuple<uint, uint>(this.random.NextUInt32() & 2139062143U, this.random.NextUInt32() & 2139062143U);
			this.key = this.random.NextUInt32();
			this.fieldLayout = new byte[6];
			for (int i = 0; i < 6; i++)
			{
				int num = this.random.NextInt32(0, 6);
				while (this.fieldLayout[num] > 0)
				{
					num = this.random.NextInt32(0, 6);
				}
				this.fieldLayout[num] = (byte)i;
			}
			Mode parameter = parameters.GetParameter<Mode>(context, context.CurrentModule, "key", Mode.Normal);
			Mode mode = parameter;
			if (mode != Mode.Normal)
			{
				if (mode != Mode.Dynamic)
				{
					throw new UnreachableException();
				}
				this.deriver = new DynamicDeriver();
			}
			else
			{
				this.deriver = new NormalDeriver();
			}
			this.deriver.Init(context, this.random);
			IRuntimeService service = context.Registry.GetService<IRuntimeService>();
			TypeDef runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiTamperJIT");
			IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, context.CurrentModule.GlobalType, context.CurrentModule);
			this.initMethod = enumerable.OfType<MethodDef>().Single((MethodDef method) => method.Name == "Initialize");
			this.initMethod.Body.SimplifyMacros(this.initMethod.Parameters);
			List<Instruction> list = this.initMethod.Body.Instructions.ToList<Instruction>();
			for (int j = 0; j < list.Count; j++)
			{
				Instruction instruction = list[j];
				bool flag = instruction.OpCode == OpCodes.Ldtoken;
				if (flag)
				{
					instruction.Operand = context.CurrentModule.GlobalType;
				}
				else
				{
					bool flag2 = instruction.OpCode == OpCodes.Call;
					if (flag2)
					{
						IMethod method2 = (IMethod)instruction.Operand;
						bool flag3 = method2.DeclaringType.Name == "Mutation" && method2.Name == "Crypt";
						if (flag3)
						{
							Instruction instruction2 = list[j - 2];
							Instruction instruction3 = list[j - 1];
							Debug.Assert(instruction2.OpCode == OpCodes.Ldloc && instruction3.OpCode == OpCodes.Ldloc);
							list.RemoveAt(j);
							list.RemoveAt(j - 1);
							list.RemoveAt(j - 2);
							list.InsertRange(j - 2, this.deriver.EmitDerivation(this.initMethod, context, (Local)instruction2.Operand, (Local)instruction3.Operand));
						}
					}
				}
			}
			this.initMethod.Body.Instructions.Clear();
			foreach (Instruction item in list)
			{
				this.initMethod.Body.Instructions.Add(item);
			}
			MutationHelper.InjectKeys(this.initMethod, new int[]
			{
				0,
				1,
				2,
				3,
				4
			}, new int[]
			{
				(int)(this._sectionName.Item1 * this._sectionName.Item2),
				(int)this.z,
				(int)this.x,
				(int)this.c,
				(int)this.v
			});
			INameService service2 = context.Registry.GetService<INameService>();
			IMarkerService service3 = context.Registry.GetService<IMarkerService>();
			this.cctor = context.CurrentModule.GlobalType.FindStaticConstructor();
			this.cctorRepl = new MethodDefUser(service2.RandomName(), MethodSig.CreateStatic(context.CurrentModule.CorLibTypes.Void));
			this.cctorRepl.IsStatic = true;
			this.cctorRepl.Access = MethodAttributes.PrivateScope;
			this.cctorRepl.Body = new CilBody();
			this.cctorRepl.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			context.CurrentModule.GlobalType.Methods.Add(this.cctorRepl);
			service2.MarkHelper(this.cctorRepl, service3, parent);
			MutationHelper.InjectKeys(enumerable.OfType<MethodDef>().Single((MethodDef method) => method.Name == "HookHandler"), new int[1], new int[]
			{
				(int)this.key
			});
			foreach (IDnlibDef dnlibDef in enumerable)
			{
				bool flag4 = dnlibDef.Name == "MethodData";
				if (flag4)
				{
					TypeDef typeDef = (TypeDef)dnlibDef;
					FieldDef[] array = typeDef.Fields.ToArray<FieldDef>();
					byte[] array2 = this.fieldLayout.Clone() as byte[];
					Array.Sort<byte, FieldDef>(array2, array);
					for (byte b = 0; b < 6; b += 1)
					{
						array2[(int)b] = b;
					}
					Array.Sort<byte, byte>(this.fieldLayout, array2);
					this.fieldLayout = array2;
					typeDef.Fields.Clear();
					foreach (FieldDef item2 in array)
					{
						typeDef.Fields.Add(item2);
					}
				}
				service2.MarkHelper(dnlibDef, service3, parent);
				bool flag5 = dnlibDef is MethodDef;
				if (flag5)
				{
					parent.ExcludeMethod(context, (MethodDef)dnlibDef);
				}
			}
			parent.ExcludeMethod(context, this.cctor);
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0001B74C File Offset: 0x0001994C
		public void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters)
		{
			this.cctorRepl.Body = this.cctor.Body;
			this.cctor.Body = new CilBody();
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.initMethod));
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, this.cctorRepl));
			this.cctor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			this.methods = ImmutableList.ToImmutableList<MethodDef>(from method in parameters.Targets.OfType<MethodDef>()
			where method.HasBody
			select method);
			context.CurrentModuleWriterOptions.WriterEvent += this.OnWriterEvent;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0001B840 File Offset: 0x00019A40
		private void OnWriterEvent(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase writer = (ModuleWriterBase)sender;
			bool flag = e.Event == ModuleWriterEvent.MDBeginWriteMethodBodies;
			if (flag)
			{
				this.context.Logger.Debug("Extracting method bodies...");
				this.CreateSection(writer);
			}
			else
			{
				bool flag2 = e.Event == ModuleWriterEvent.BeginStrongNameSign;
				if (flag2)
				{
					this.context.Logger.Debug("Encrypting method section...");
					this.EncryptSection(writer);
				}
			}
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0001B8B8 File Offset: 0x00019AB8
		private void CreateSection(ModuleWriterBase writer)
		{
			PESection pesection = new PESection("", 1610612768U);
			bool flag = false;
			bool flag2 = writer.StrongNameSignature != null;
			if (flag2)
			{
				uint value = writer.TextSection.Remove(writer.StrongNameSignature).Value;
				pesection.Add(writer.StrongNameSignature, value);
				flag = true;
			}
			ModuleWriter moduleWriter = writer as ModuleWriter;
			bool flag3 = moduleWriter != null;
			if (flag3)
			{
				bool flag4 = moduleWriter.ImportAddressTable != null;
				if (flag4)
				{
					uint value = writer.TextSection.Remove(moduleWriter.ImportAddressTable).Value;
					pesection.Add(moduleWriter.ImportAddressTable, value);
					flag = true;
				}
				bool flag5 = moduleWriter.StartupStub != null;
				if (flag5)
				{
					uint value = writer.TextSection.Remove(moduleWriter.StartupStub).Value;
					pesection.Add(moduleWriter.StartupStub, value);
					flag = true;
				}
			}
			bool flag6 = flag;
			if (flag6)
			{
				writer.Sections.AddBeforeReloc(pesection);
			}
			byte[] bytes = new byte[]
			{
				(byte)this._sectionName.Item1,
				(byte)(this._sectionName.Item1 >> 8),
				(byte)(this._sectionName.Item1 >> 16),
				(byte)(this._sectionName.Item1 >> 24),
				(byte)this._sectionName.Item2,
				(byte)(this._sectionName.Item2 >> 8),
				(byte)(this._sectionName.Item2 >> 16),
				(byte)(this._sectionName.Item2 >> 24)
			};
			PESection pesection2 = new PESection(Encoding.ASCII.GetString(bytes), 3758096448U);
			writer.Sections.InsertBeforeReloc(this.random.NextInt32(writer.Sections.Count), pesection2);
			pesection2.Add(new ByteArrayChunk(this.random.NextBytes(16).ToArray<byte>()), 16U);
			IImmutableList<MethodDef> immutableList = this.methods.RemoveAll((MethodDef m) => !m.HasBody);
			JITBodyIndex jitbodyIndex = new JITBodyIndex(from method in immutableList
			select writer.Metadata.GetToken(method).Raw);
			pesection2.Add(jitbodyIndex, 16U);
			CilBody body = new CilBody
			{
				Instructions = 
				{
					Instruction.Create(OpCodes.Ldnull),
					Instruction.Create(OpCodes.Throw)
				}
			};
			foreach (MethodDef methodDef in immutableList)
			{
				MDToken token = writer.Metadata.GetToken(methodDef);
				JITMethodBody jitmethodBody = new JITMethodBody();
				JITMethodBodyWriter jitmethodBodyWriter = new JITMethodBodyWriter(writer.Metadata, methodDef.Body, jitmethodBody, this.random.NextUInt32(), writer.Metadata.KeepOldMaxStack || methodDef.Body.KeepOldMaxStack);
				jitmethodBodyWriter.Write();
				jitmethodBody.Serialize(token.Raw, this.key, this.fieldLayout);
				jitbodyIndex.Add(token.Raw, jitmethodBody);
				methodDef.Body = body;
				RawMethodRow rawMethodRow = writer.Metadata.TablesHeap.MethodTable[token.Rid];
				writer.Metadata.TablesHeap.MethodTable[token.Rid] = new RawMethodRow(rawMethodRow.RVA, rawMethodRow.ImplFlags | 8, rawMethodRow.Flags, rawMethodRow.Name, rawMethodRow.Signature, rawMethodRow.ParamList);
			}
			jitbodyIndex.PopulateSection(pesection2);
			pesection2.Add(new ByteArrayChunk(new byte[4]), 4U);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0001BD00 File Offset: 0x00019F00
		private void EncryptSection(ModuleWriterBase writer)
		{
			Stream destinationStream = writer.DestinationStream;
			BinaryReader binaryReader = new BinaryReader(destinationStream);
			destinationStream.Position = 60L;
			destinationStream.Position = (long)((ulong)binaryReader.ReadUInt32());
			destinationStream.Position += 6L;
			ushort num = binaryReader.ReadUInt16();
			destinationStream.Position += 12L;
			ushort num2 = binaryReader.ReadUInt16();
			destinationStream.Position += (long)(2 + num2);
			uint num3 = 0U;
			uint num4 = 0U;
			int num5 = -1;
			ModuleDefMD moduleDefMD;
			bool flag;
			if (writer is NativeModuleWriter)
			{
				ModuleDef module = writer.Module;
				moduleDefMD = (module as ModuleDefMD);
				flag = (moduleDefMD != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				num5 = moduleDefMD.Metadata.PEImage.ImageSectionHeaders.Count;
			}
			for (int i = 0; i < (int)num; i++)
			{
				bool flag3 = num5 > 0;
				uint num6;
				if (flag3)
				{
					num5--;
					destinationStream.Write(new byte[8], 0, 8);
					num6 = 0U;
				}
				else
				{
					num6 = binaryReader.ReadUInt32() * binaryReader.ReadUInt32();
				}
				destinationStream.Position += 8L;
				bool flag4 = num6 == this._sectionName.Item1 * this._sectionName.Item2;
				if (flag4)
				{
					num4 = binaryReader.ReadUInt32();
					num3 = binaryReader.ReadUInt32();
				}
				else
				{
					bool flag5 = num6 > 0U;
					if (flag5)
					{
						uint size = binaryReader.ReadUInt32();
						uint offset = binaryReader.ReadUInt32();
						this.Hash(destinationStream, binaryReader, offset, size);
					}
					else
					{
						destinationStream.Position += 8L;
					}
				}
				destinationStream.Position += 16L;
			}
			uint[] array = this.DeriveKey();
			num4 >>= 2;
			destinationStream.Position = (long)((ulong)num3);
			uint[] array2 = new uint[num4];
			for (uint num7 = 0U; num7 < num4; num7 += 1U)
			{
				uint num8 = binaryReader.ReadUInt32();
				array2[(int)num7] = (num8 ^ array[(int)(num7 & 15U)]);
				array[(int)(num7 & 15U)] = (array[(int)(num7 & 15U)] ^ num8) + 1035675673U;
			}
			byte[] array3 = new byte[num4 << 2];
			Buffer.BlockCopy(array2, 0, array3, 0, array3.Length);
			destinationStream.Position = (long)((ulong)num3);
			destinationStream.Write(array3, 0, array3.Length);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0001BF48 File Offset: 0x0001A148
		private void Hash(Stream stream, BinaryReader reader, uint offset, uint size)
		{
			long position = stream.Position;
			stream.Position = (long)((ulong)offset);
			size >>= 2;
			for (uint num = 0U; num < size; num += 1U)
			{
				uint num2 = reader.ReadUInt32();
				uint num3 = (this.z ^ num2) + this.x + this.c * this.v;
				this.z = this.x;
				this.x = this.c;
				this.x = this.v;
				this.v = num3;
			}
			stream.Position = position;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x0001BFDC File Offset: 0x0001A1DC
		private uint[] DeriveKey()
		{
			uint[] array = new uint[16];
			uint[] array2 = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				array[i] = this.v;
				array2[i] = this.x;
				this.z = (this.x >> 5 | this.x << 27);
				this.x = (this.c >> 3 | this.c << 29);
				this.c = (this.v >> 7 | this.v << 25);
				this.v = (this.z >> 11 | this.z << 21);
			}
			return this.deriver.DeriveKey(array, array2);
		}

		// Token: 0x04000212 RID: 530
		protected const uint CNT_CODE = 32U;

		// Token: 0x04000213 RID: 531
		protected const uint CNT_INITIALIZED_DATA = 64U;

		// Token: 0x04000214 RID: 532
		protected const uint MEM_EXECUTE = 536870912U;

		// Token: 0x04000215 RID: 533
		protected const uint MEM_READ = 1073741824U;

		// Token: 0x04000216 RID: 534
		protected const uint MEM_WRITE = 2147483648U;

		// Token: 0x04000217 RID: 535
		private MethodDef cctor;

		// Token: 0x04000218 RID: 536
		private MethodDef cctorRepl;

		// Token: 0x04000219 RID: 537
		private ConfuserContext context;

		// Token: 0x0400021A RID: 538
		private IKeyDeriver deriver;

		// Token: 0x0400021B RID: 539
		private byte[] fieldLayout;

		// Token: 0x0400021C RID: 540
		private MethodDef initMethod;

		// Token: 0x0400021D RID: 541
		private uint key;

		// Token: 0x0400021E RID: 542
		private IImmutableList<MethodDef> methods;

		// Token: 0x0400021F RID: 543
		[TupleElementNames(new string[]
		{
			"Part1",
			"Part2"
		})]
		private ValueTuple<uint, uint> _sectionName;

		// Token: 0x04000220 RID: 544
		private RandomGenerator random;

		// Token: 0x04000221 RID: 545
		private uint c;

		// Token: 0x04000222 RID: 546
		private uint v;

		// Token: 0x04000223 RID: 547
		private uint x;

		// Token: 0x04000224 RID: 548
		private uint z;
	}
}
