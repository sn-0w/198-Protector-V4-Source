using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000062 RID: 98
	internal class MildMode : RPMode
	{
		// Token: 0x060001EB RID: 491 RVA: 0x0000A94C File Offset: 0x00008B4C
		public override void ProcessCall(RPContext ctx, int instrIndex)
		{
			bool flag = ctx == null;
			if (flag)
			{
				throw new ArgumentNullException("ctx");
			}
			bool flag2 = instrIndex < 0 || instrIndex >= ctx.Body.Instructions.Count;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("instrIndex", instrIndex, "Instruction index is not within the legal range.");
			}
			Instruction instruction = ctx.Body.Instructions[instrIndex];
			object operand = instruction.Operand;
			IMethod method = operand as IMethod;
			bool flag3 = method == null;
			if (flag3)
			{
				Debug.Assert(false, "target of instruction is not a method.");
			}
			else
			{
				MethodDef methodDef = method.ResolveThrow();
				bool isValueType = methodDef.DeclaringType.ResolveTypeDefThrow().IsValueType;
				if (!isValueType)
				{
					bool flag4 = !methodDef.ResolveThrow().IsPublic && !method.ResolveThrow().IsAssembly;
					if (!flag4)
					{
						ValueTuple<Code, TypeDef, IMethod> key = new ValueTuple<Code, TypeDef, IMethod>(instruction.OpCode.Code, ctx.Method.DeclaringType, method);
						MethodDef methodDef2;
						bool flag5 = !this._proxies.TryGetValue(key, out methodDef2);
						if (flag5)
						{
							MethodSig methodSig = RPMode.CreateProxySignature(ctx, method, instruction.OpCode.Code == Code.Newobj);
							methodDef2 = new MethodDefUser(ctx.Name.RandomName(), methodSig);
							methodDef2.Attributes = MethodAttributes.Static;
							methodDef2.ImplAttributes = MethodImplAttributes.IL;
							ctx.Method.DeclaringType.Methods.Add(methodDef2);
							bool flag6 = instruction.OpCode.Code == Code.Call && methodDef.IsVirtual;
							if (flag6)
							{
								methodDef2.IsStatic = false;
								methodSig.HasThis = true;
								methodSig.Params.RemoveAt(0);
							}
							ctx.Marker.Mark(methodDef2, ctx.Protection);
							ctx.Name.Analyze(methodDef2);
							ctx.Name.SetCanRename(methodDef2, false);
							methodDef2.Body = new CilBody();
							methodDef2.Body.Instructions.Add(Instruction.Create(OpCodes.Nop));
							foreach (Parameter parameter in methodDef2.Parameters)
							{
								methodDef2.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg, parameter));
							}
							methodDef2.Body.Instructions.Add(Instruction.Create(instruction.OpCode, method));
							methodDef2.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
							this._proxies[key] = methodDef2;
						}
						instruction.OpCode = OpCodes.Call;
						bool hasGenericParameters = ctx.Method.DeclaringType.HasGenericParameters;
						if (hasGenericParameters)
						{
							TypeSig[] genArgs = (from i in Enumerable.Range(0, ctx.Method.DeclaringType.GenericParameters.Count)
							select new GenericVar(i)).Cast<TypeSig>().ToArray<TypeSig>();
							instruction.Operand = new MemberRefUser(ctx.Module, methodDef2.Name, methodDef2.MethodSig, new GenericInstSig(ctx.Method.DeclaringType.ToTypeSig(true).ToClassOrValueTypeSig(), genArgs).ToTypeDefOrRef());
						}
						else
						{
							instruction.Operand = methodDef2;
						}
						ctx.Context.Annotations.Set<object>(methodDef, ReferenceProxyProtection.Targeted, ReferenceProxyProtection.Targeted);
					}
				}
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000211A File Offset: 0x0000031A
		public override void Finalize(RPContext ctx)
		{
		}

		// Token: 0x04000081 RID: 129
		[TupleElementNames(new string[]
		{
			"OpCode",
			"CallingType",
			"TargetMethod"
		})]
		private readonly Dictionary<ValueTuple<Code, TypeDef, IMethod>, MethodDef> _proxies = new Dictionary<ValueTuple<Code, TypeDef, IMethod>, MethodDef>();
	}
}
