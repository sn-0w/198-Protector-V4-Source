using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000067 RID: 103
	internal class StrongMode : RPMode
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0000B18C File Offset: 0x0000938C
		private static int? TraceBeginning(RPContext ctx, int index, int argCount)
		{
			bool flag = ctx.BranchTargets.Contains(ctx.Body.Instructions[index]);
			int? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int i = argCount;
				int num = index;
				while (i > 0)
				{
					num--;
					Instruction instruction = ctx.Body.Instructions[num];
					bool flag2 = instruction.OpCode == OpCodes.Pop || instruction.OpCode == OpCodes.Dup;
					if (flag2)
					{
						return null;
					}
					FlowControl flowControl = instruction.OpCode.FlowControl;
					FlowControl flowControl2 = flowControl;
					if (flowControl2 - FlowControl.Break > 1 && flowControl2 - FlowControl.Meta > 1)
					{
						return null;
					}
					int num2;
					int num3;
					instruction.CalculateStackUsage(out num2, out num3);
					i += num3;
					i -= num2;
					bool flag3 = ctx.BranchTargets.Contains(instruction) && i != 0;
					if (flag3)
					{
						return null;
					}
				}
				bool flag4 = i < 0;
				if (flag4)
				{
					result = null;
				}
				else
				{
					result = new int?(num);
				}
			}
			return result;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000B2C4 File Offset: 0x000094C4
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod method = instruction.Operand as IMethod;
			bool flag = method == null;
			if (!flag)
			{
				TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
				bool flag2 = !typeDef.Module.IsILOnly;
				if (!flag2)
				{
					bool isGlobalModuleType = typeDef.IsGlobalModuleType;
					if (!isGlobalModuleType)
					{
						int num;
						int argCount;
						instruction.CalculateStackUsage(out num, out argCount);
						int? num2 = StrongMode.TraceBeginning(ctx, instrIndex, argCount);
						bool flag3 = num2 == null;
						bool flag4 = flag3;
						if (flag4)
						{
							this.ProcessBridge(ctx, instrIndex);
						}
						else
						{
							this.ProcessInvoke(ctx, instrIndex, num2.Value);
						}
					}
				}
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B378 File Offset: 0x00009578
		private void ProcessBridge(RPContext ctx, int instrIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod method = instruction.Operand as IMethod;
			bool flag = method == null;
			if (!flag)
			{
				TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
				bool flag2 = !typeDef.Module.IsILOnly;
				if (!flag2)
				{
					bool isGlobalModuleType = typeDef.IsGlobalModuleType;
					if (!isGlobalModuleType)
					{
						ValueTuple<Code, IMethod, IRPEncoding> key = new ValueTuple<Code, IMethod, IRPEncoding>(instruction.OpCode.Code, method, ctx.EncodingHandler);
						ValueTuple<FieldDef, MethodDef> valueTuple;
						bool flag3 = this._fields.TryGetValue(key, out valueTuple);
						if (flag3)
						{
							bool flag4 = valueTuple.Item2 != null;
							if (flag4)
							{
								instruction.OpCode = OpCodes.Call;
								instruction.Operand = valueTuple.Item2;
								return;
							}
						}
						MethodSig sig = RPMode.CreateProxySignature(ctx, method, instruction.OpCode.Code == Code.Newobj);
						TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
						bool flag5 = valueTuple.Item1 == null;
						if (flag5)
						{
							valueTuple = new ValueTuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), valueTuple.Item2);
						}
						Debug.Assert(valueTuple.Item2 == null);
						valueTuple = new ValueTuple<FieldDef, MethodDef>(valueTuple.Item1, this.CreateBridge(ctx, delegateType, valueTuple.Item1, sig));
						this._fields[key] = valueTuple;
						instruction.OpCode = OpCodes.Call;
						instruction.Operand = valueTuple.Item2;
						MethodDef methodDef = method.ResolveMethodDef();
						bool flag6 = methodDef != null;
						if (flag6)
						{
							ctx.Context.Annotations.Set<object>(methodDef, ReferenceProxyProtection.Targeted, ReferenceProxyProtection.Targeted);
						}
					}
				}
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B518 File Offset: 0x00009718
		private void ProcessInvoke(RPContext ctx, int instrIndex, int argBeginIndex)
		{
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			IMethod method = instruction.Operand as IMethod;
			bool flag = method == null;
			if (!flag)
			{
				MethodSig sig = RPMode.CreateProxySignature(ctx, method, instruction.OpCode.Code == Code.Newobj);
				TypeDef delegateType = RPMode.GetDelegateType(ctx, sig);
				ValueTuple<Code, IMethod, IRPEncoding> key = new ValueTuple<Code, IMethod, IRPEncoding>(instruction.OpCode.Code, method, ctx.EncodingHandler);
				ValueTuple<FieldDef, MethodDef> valueTuple;
				bool flag2 = !this._fields.TryGetValue(key, out valueTuple);
				if (flag2)
				{
					valueTuple = new ValueTuple<FieldDef, MethodDef>(this.CreateField(ctx, delegateType), null);
					this._fields[key] = valueTuple;
				}
				bool flag3 = argBeginIndex == instrIndex;
				if (flag3)
				{
					ctx.Body.Instructions.Insert(instrIndex + 1, new Instruction(OpCodes.Call, delegateType.FindMethod("Invoke")));
					instruction.OpCode = OpCodes.Ldsfld;
					instruction.Operand = valueTuple.Item1;
				}
				else
				{
					Instruction instruction2 = ctx.Body.Instructions[argBeginIndex];
					ctx.Body.Instructions.Insert(argBeginIndex + 1, new Instruction(instruction2.OpCode, instruction2.Operand));
					instruction2.OpCode = OpCodes.Ldsfld;
					instruction2.Operand = valueTuple.Item1;
					instruction.OpCode = OpCodes.Call;
					instruction.Operand = delegateType.FindMethod("Invoke");
				}
				MethodDef methodDef = method.ResolveMethodDef();
				bool flag4 = methodDef != null;
				if (flag4)
				{
					ctx.Context.Annotations.Set<object>(methodDef, ReferenceProxyProtection.Targeted, ReferenceProxyProtection.Targeted);
				}
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B6C0 File Offset: 0x000098C0
		private MethodDef CreateBridge(RPContext ctx, TypeDef delegateType, FieldDef field, MethodSig sig)
		{
			MethodDefUser methodDefUser = new MethodDefUser(ctx.Name.RandomName(), sig);
			methodDefUser.Attributes = MethodAttributes.Static;
			methodDefUser.ImplAttributes = MethodImplAttributes.IL;
			methodDefUser.Body = new CilBody();
			methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, field));
			foreach (Parameter parameter in methodDefUser.Parameters)
			{
				methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
			}
			methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Call, delegateType.FindMethod("Invoke")));
			methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			delegateType.Methods.Add(methodDefUser);
			ctx.MarkMember(methodDefUser);
			return methodDefUser;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B7DC File Offset: 0x000099DC
		private FieldDef CreateField(RPContext ctx, TypeDef delegateType)
		{
			TypeDef typeDef;
			do
			{
				typeDef = ctx.Module.Types[ctx.Random.NextInt32(ctx.Module.Types.Count)];
			}
			while (typeDef.HasGenericParameters || typeDef.IsGlobalModuleType || typeDef.IsDelegate());
			TypeSig type = new CModOptSig(typeDef, delegateType.ToTypeSig(true));
			FieldDefUser fieldDefUser = new FieldDefUser("", new FieldSig(type), FieldAttributes.Private | FieldAttributes.FamANDAssem | FieldAttributes.Static);
			fieldDefUser.CustomAttributes.Add(new CustomAttribute(this.GetKeyAttr(ctx).FindInstanceConstructors().First<MethodDef>()));
			bool netGuard = this.NetGuard;
			if (netGuard)
			{
				ctx.Module.GlobalType.Fields.Add(fieldDefUser);
			}
			else
			{
				delegateType.Fields.Add(fieldDefUser);
			}
			ctx.MarkMember(fieldDefUser);
			return fieldDefUser;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B8C0 File Offset: 0x00009AC0
		private TypeDef GetKeyAttr(RPContext ctx)
		{
			bool flag = this._keyAttrs == null;
			if (flag)
			{
				this._keyAttrs = new ValueTuple<TypeDef, Func<int, int>>[16];
			}
			int num = ctx.Random.NextInt32(this._keyAttrs.Length);
			bool flag2 = this._keyAttrs[num].Item1 == null;
			if (flag2)
			{
				TypeDef runtimeType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.RefProxyKey");
				TypeDef typeDef = InjectHelper.Inject(runtimeType, ctx.Module);
				typeDef.Name = ctx.Name.RandomName();
				typeDef.Namespace = string.Empty;
				Variable variable = new Variable("{VAR}");
				Variable variable2 = new Variable("{RESULT}");
				Expression expression;
				Expression inverse;
				ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
				{
					Variable = variable
				}, new VariableExpression
				{
					Variable = variable2
				}, ctx.Depth, out expression, out inverse);
				Func<int, int> func = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
				{
					Tuple.Create<string, Type>("{VAR}", typeof(int))
				}).GenerateCIL(expression).Compile<Func<int, int>>();
				MethodDef ctor = typeDef.FindMethod(".ctor");
				MutationHelper.ReplacePlaceholder(ctor, delegate(Instruction[] arg)
				{
					List<Instruction> list = new List<Instruction>();
					new StrongMode.CodeGen(arg, ctor, list).GenerateCIL(inverse);
					return list.ToArray();
				});
				this._keyAttrs[num] = new ValueTuple<TypeDef, Func<int, int>>(typeDef, func);
				ctx.Module.AddAsNonNestedType(typeDef);
				foreach (IDnlibDef dnlibDef in typeDef.FindDefinitions())
				{
					bool flag3 = dnlibDef.Name == "GetHashCode";
					if (flag3)
					{
						ctx.Name.MarkHelper(dnlibDef, ctx.Marker, ctx.Protection);
						((MethodDef)dnlibDef).Access = MethodAttributes.Public;
					}
					else
					{
						ctx.Name.MarkHelper(dnlibDef, ctx.Marker, ctx.Protection);
					}
				}
			}
			return this._keyAttrs[num].Item1;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000BB0C File Offset: 0x00009D0C
		private StrongMode.InitMethodDesc GetInitMethod(RPContext ctx, IRPEncoding encoding)
		{
			StrongMode.InitMethodDesc[] array;
			bool flag = !this._initMethods.TryGetValue(encoding, out array);
			if (flag)
			{
				array = (this._initMethods[encoding] = new StrongMode.InitMethodDesc[ctx.InitCount]);
			}
			int num = ctx.Random.NextInt32(array.Length);
			bool flag2 = array[num] == null;
			if (flag2)
			{
				TypeDef runtimeType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.RefProxyStrong");
				MethodDef injectedMethod = InjectHelper.Inject(runtimeType.FindMethod("Initialize"), ctx.Module);
				ctx.Module.GlobalType.Methods.Add(injectedMethod);
				injectedMethod.Access = MethodAttributes.PrivateScope;
				injectedMethod.Name = ctx.Name.RandomName();
				ctx.Name.SetCanRename(injectedMethod, false);
				ctx.Marker.Mark(injectedMethod, ctx.Protection);
				StrongMode.InitMethodDesc initMethodDesc = new StrongMode.InitMethodDesc
				{
					Method = injectedMethod
				};
				int[] array2 = Enumerable.Range(0, 5).ToArray<int>();
				ctx.Random.Shuffle<int>(array2);
				initMethodDesc.OpCodeIndex = array2[4];
				initMethodDesc.TokenNameOrder = new int[4];
				Array.Copy(array2, 0, initMethodDesc.TokenNameOrder, 0, 4);
				initMethodDesc.TokenByteOrder = (from x in Enumerable.Range(0, 4)
				select x * 8).ToArray<int>();
				ctx.Random.Shuffle<int>(initMethodDesc.TokenByteOrder);
				int[] array3 = new int[9];
				Array.Copy(initMethodDesc.TokenNameOrder, 0, array3, 0, 4);
				Array.Copy(initMethodDesc.TokenByteOrder, 0, array3, 4, 4);
				array3[8] = initMethodDesc.OpCodeIndex;
				MutationHelper.InjectKeys(injectedMethod, Enumerable.Range(0, 9).ToArray<int>(), array3);
				MutationHelper.ReplacePlaceholder(injectedMethod, (Instruction[] arg) => encoding.EmitDecode(injectedMethod, ctx, arg));
				initMethodDesc.Encoding = encoding;
				array[num] = initMethodDesc;
			}
			return array[num];
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000BDEC File Offset: 0x00009FEC
		public override void Finalize(RPContext ctx)
		{
			foreach (KeyValuePair<ValueTuple<Code, IMethod, IRPEncoding>, ValueTuple<FieldDef, MethodDef>> keyValuePair in this._fields)
			{
				StrongMode.InitMethodDesc initMethod = this.GetInitMethod(ctx, keyValuePair.Key.Item3);
				byte b;
				do
				{
					b = ctx.Random.NextByte();
				}
				while (b == (byte)keyValuePair.Key.Item1);
				TypeDef declaringType = keyValuePair.Value.Item1.DeclaringType;
				bool netGuard = this.NetGuard;
				MethodDef methodDef;
				if (netGuard)
				{
					methodDef = ctx.Module.GlobalType.FindStaticConstructor();
				}
				else
				{
					methodDef = keyValuePair.Value.Item1.DeclaringType.FindOrCreateStaticConstructor();
				}
				methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, initMethod.Method));
				methodDef.Body.Instructions.Insert(0, Instruction.CreateLdcI4((int)b));
				methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldtoken, keyValuePair.Value.Item1));
				this._fieldDescs.Add(new StrongMode.FieldDesc
				{
					Field = keyValuePair.Value.Item1,
					OpCode = keyValuePair.Key.Item1,
					Method = keyValuePair.Key.Item2,
					OpKey = b,
					InitDesc = initMethod
				});
			}
			foreach (TypeDef typeDef in ctx.Delegates.Values)
			{
				MethodDef methodDef2 = typeDef.FindOrCreateStaticConstructor();
				ctx.Marker.Mark(methodDef2, ctx.Protection);
				ctx.Name.SetCanRename(methodDef2, false);
			}
			ctx.Context.CurrentModuleWriterOptions.MetadataOptions.Flags |= MetadataFlags.PreserveExtraSignatureData;
			ctx.Context.CurrentModuleWriterOptions.WriterEvent += this.EncodeField;
			this.encodeCtx = ctx;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000C050 File Offset: 0x0000A250
		private void EncodeField(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
			bool flag = e.Event == ModuleWriterEvent.MDMemberDefRidsAllocated && this._keyAttrs != null;
			if (flag)
			{
				Dictionary<ITypeDefOrRef, Func<int, int>> dictionary = (from entry in this._keyAttrs
				where entry.Item1 != null
				select entry).ToDictionary(([TupleElementNames(new string[]
				{
					"RefProxyKeyTypeDef",
					"EncodeFunc"
				})] ValueTuple<TypeDef, Func<int, int>> entry) => entry.Item1, ([TupleElementNames(new string[]
				{
					"RefProxyKeyTypeDef",
					"EncodeFunc"
				})] ValueTuple<TypeDef, Func<int, int>> entry) => entry.Item2);
				foreach (StrongMode.FieldDesc fieldDesc in this._fieldDescs)
				{
					uint num = moduleWriterBase.Metadata.GetToken(fieldDesc.Method).Raw;
					uint num2 = this.encodeCtx.Random.NextUInt32() | 1U;
					CustomAttribute customAttribute = fieldDesc.Field.CustomAttributes[0];
					int num3 = dictionary[(TypeDef)customAttribute.AttributeType]((int)MathsUtils.modInv(num2));
					customAttribute.ConstructorArguments.Add(new CAArgument(this.encodeCtx.Module.CorLibTypes.Int32, num3));
					num *= num2;
					num = (uint)fieldDesc.InitDesc.Encoding.Encode(fieldDesc.InitDesc.Method, this.encodeCtx, (int)num);
					char[] array = new char[5];
					array[fieldDesc.InitDesc.OpCodeIndex] = (char)((byte)fieldDesc.OpCode ^ fieldDesc.OpKey);
					byte[] array2 = this.encodeCtx.Random.NextBytes(4);
					uint num4 = 0U;
					for (int i = 0; i < 4; i++)
					{
						while (array2[i] == 0)
						{
							array2[i] = this.encodeCtx.Random.NextByte();
						}
						array[fieldDesc.InitDesc.TokenNameOrder[i]] = (char)array2[i];
						num4 |= (uint)((uint)array2[i] << fieldDesc.InitDesc.TokenByteOrder[i]);
					}
					fieldDesc.Field.Name = new string(array);
					FieldSig fieldSig = fieldDesc.Field.FieldSig;
					uint num5 = num - moduleWriterBase.Metadata.GetToken(((CModOptSig)fieldSig.Type).Modifier).Raw ^ num4;
					fieldSig.ExtraData = new byte[]
					{
						192,
						0,
						0,
						(byte)(num5 >> fieldDesc.InitDesc.TokenByteOrder[3]),
						192,
						(byte)(num5 >> fieldDesc.InitDesc.TokenByteOrder[2]),
						(byte)(num5 >> fieldDesc.InitDesc.TokenByteOrder[1]),
						(byte)(num5 >> fieldDesc.InitDesc.TokenByteOrder[0])
					};
				}
			}
		}

		// Token: 0x04000087 RID: 135
		private readonly bool NetGuard = true;

		// Token: 0x04000088 RID: 136
		private readonly List<StrongMode.FieldDesc> _fieldDescs = new List<StrongMode.FieldDesc>();

		// Token: 0x04000089 RID: 137
		[TupleElementNames(new string[]
		{
			"OpCode",
			"TargetMethod",
			"Encoding",
			"Field",
			"BridgeMethod"
		})]
		private readonly Dictionary<ValueTuple<Code, IMethod, IRPEncoding>, ValueTuple<FieldDef, MethodDef>> _fields = new Dictionary<ValueTuple<Code, IMethod, IRPEncoding>, ValueTuple<FieldDef, MethodDef>>();

		// Token: 0x0400008A RID: 138
		private readonly Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]> _initMethods = new Dictionary<IRPEncoding, StrongMode.InitMethodDesc[]>();

		// Token: 0x0400008B RID: 139
		private RPContext encodeCtx;

		// Token: 0x0400008C RID: 140
		[TupleElementNames(new string[]
		{
			"RefProxyKeyTypeDef",
			"EncodeFunc"
		})]
		private ValueTuple<TypeDef, Func<int, int>>[] _keyAttrs;

		// Token: 0x02000068 RID: 104
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000207 RID: 519 RVA: 0x00002C7F File Offset: 0x00000E7F
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x06000208 RID: 520 RVA: 0x0000C394 File Offset: 0x0000A594
			protected override void LoadVar(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				if (flag)
				{
					foreach (Instruction instr in this.arg)
					{
						base.Emit(instr);
					}
				}
				else
				{
					base.LoadVar(var);
				}
			}

			// Token: 0x0400008D RID: 141
			private readonly Instruction[] arg;
		}

		// Token: 0x02000069 RID: 105
		private class FieldDesc
		{
			// Token: 0x0400008E RID: 142
			public FieldDef Field;

			// Token: 0x0400008F RID: 143
			public StrongMode.InitMethodDesc InitDesc;

			// Token: 0x04000090 RID: 144
			public IMethod Method;

			// Token: 0x04000091 RID: 145
			public Code OpCode;

			// Token: 0x04000092 RID: 146
			public byte OpKey;
		}

		// Token: 0x0200006A RID: 106
		private class InitMethodDesc
		{
			// Token: 0x04000093 RID: 147
			public IRPEncoding Encoding;

			// Token: 0x04000094 RID: 148
			public MethodDef Method;

			// Token: 0x04000095 RID: 149
			public int OpCodeIndex;

			// Token: 0x04000096 RID: 150
			public int[] TokenByteOrder;

			// Token: 0x04000097 RID: 151
			public int[] TokenNameOrder;
		}
	}
}
