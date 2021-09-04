using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000084 RID: 132
	internal class CFContext
	{
		// Token: 0x06000257 RID: 599 RVA: 0x0000E508 File Offset: 0x0000C708
		public void AddJump(IList<Instruction> instrs, Instruction target)
		{
			bool flag = false;
			if (!this.Method.Module.IsClr40 && this.JunkCode && !this.Method.DeclaringType.HasGenericParameters && !this.Method.HasGenericParameters && (instrs[0].OpCode.FlowControl == FlowControl.Call || instrs[0].OpCode.FlowControl == FlowControl.Next))
			{
				switch (this.Random.NextInt32(2))
				{
				case 0:
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					instrs.Add(Instruction.Create(OpCodes.Brtrue, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					if (this.Random.NextBoolean())
					{
						TypeDef typeDef = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						if (typeDef.HasMethods)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, typeDef.Methods[this.Random.NextInt32(typeDef.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
						}
					}
					break;
				case 1:
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_0));
					instrs.Add(Instruction.Create(OpCodes.Brtrue, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4_1));
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					if (this.Random.NextBoolean())
					{
						TypeDef typeDef2 = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						if (typeDef2.HasMethods)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, typeDef2.Methods[this.Random.NextInt32(typeDef2.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
						}
					}
					break;
				case 2:
				{
					if (this.Random.NextBoolean())
					{
						TypeDef typeDef3 = this.Method.Module.Types[this.Random.NextInt32(this.Method.Module.Types.Count)];
						if (typeDef3.HasMethods)
						{
							instrs.Add(Instruction.Create(OpCodes.Ldtoken, typeDef3.Methods[this.Random.NextInt32(typeDef3.Methods.Count)]));
							instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.GetTypeRef("System", "RuntimeMethodHandle")));
							flag = true;
						}
					}
					if (!flag)
					{
						instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
						instrs.Add(Instruction.Create(OpCodes.Box, this.Method.Module.CorLibTypes.Int32.TypeDefOrRef));
					}
					Instruction item = Instruction.Create(OpCodes.Pop);
					instrs.Add(Instruction.Create(OpCodes.Brfalse, instrs[0]));
					instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.Random.NextBoolean() ? 0 : 1));
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					instrs.Add(item);
					break;
				}
				}
			}
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
			instrs.Add(Instruction.Create(OpCodes.Br, target));
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000EA20 File Offset: 0x0000CC20
		public void AddJunk(IList<Instruction> instrs)
		{
			if (!this.Method.Module.IsClr40 && this.JunkCode)
			{
				switch (this.Random.NextInt32(5))
				{
				case 0:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(131)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					return;
				case 1:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(1907)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					return;
				case 2:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(34)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					return;
				case 3:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(67)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					return;
				case 4:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(291)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					return;
				case 5:
					instrs.Add(Instruction.Create(OpCodes.Pop));
					instrs.Add(Instruction.Create(OpCodes.Dup));
					instrs.Add(Instruction.Create(OpCodes.Throw));
					instrs.Add(Instruction.Create(OpCodes.Unbox));
					instrs.Add(Instruction.Create(OpCodes.Stind_R4));
					instrs.Add(Instruction.Create(OpCodes.Mkrefany));
					instrs.Add(Instruction.Create(OpCodes.Calli));
					instrs.Add(Instruction.Create(OpCodes.Add));
					instrs.Add(Instruction.Create(OpCodes.Sub));
					instrs.Add(Instruction.Create(OpCodes.Ldarg, new Parameter(255)));
					instrs.Add(Instruction.Create(OpCodes.Ldloc, new Local(null)));
					instrs.Add(Instruction.Create(OpCodes.Ldtoken, this.Method));
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x040000DA RID: 218
		public ConfuserContext Context;

		// Token: 0x040000DB RID: 219
		public ControlFlowProtection Protection;

		// Token: 0x040000DC RID: 220
		public int Depth;

		// Token: 0x040000DD RID: 221
		public IDynCipherService DynCipher;

		// Token: 0x040000DE RID: 222
		public double Intensity;

		// Token: 0x040000DF RID: 223
		public bool JunkCode;

		// Token: 0x040000E0 RID: 224
		public MethodDef Method;

		// Token: 0x040000E1 RID: 225
		public PredicateType Predicate;

		// Token: 0x040000E2 RID: 226
		public RandomGenerator Random;

		// Token: 0x040000E3 RID: 227
		public CFType Type;
	}
}
