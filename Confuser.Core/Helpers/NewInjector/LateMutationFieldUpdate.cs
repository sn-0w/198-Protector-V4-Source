using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000BE RID: 190
	public sealed class LateMutationFieldUpdate
	{
		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x00003BEF File Offset: 0x00001DEF
		[TupleElementNames(new string[]
		{
			"Method",
			"Instruction"
		})]
		private IList<ValueTuple<MethodDef, Instruction>> UpdateInstructions { [return: TupleElementNames(new string[]
		{
			"Method",
			"Instruction"
		})] get; } = new List<ValueTuple<MethodDef, Instruction>>();

		// Token: 0x06000455 RID: 1109 RVA: 0x00003BF7 File Offset: 0x00001DF7
		internal void AddUpdateInstruction(MethodDef method, Instruction instruction)
		{
			this.UpdateInstructions.Add(new ValueTuple<MethodDef, Instruction>(method, instruction));
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00019F6C File Offset: 0x0001816C
		public void ApplyValue(int value)
		{
			foreach (ValueTuple<MethodDef, Instruction> valueTuple in this.UpdateInstructions)
			{
				Instruction item = valueTuple.Item2;
				MethodDef item2 = valueTuple.Item1;
				bool flag = item2.HasBody && item2.Body.HasInstructions;
				if (flag)
				{
					bool flag2 = item2.Body.Instructions.Contains(item);
					if (flag2)
					{
						item.OpCode = OpCodes.Ldc_I4;
						item.Operand = value;
					}
					else
					{
						Debug.Fail("Instruction is not in method anymore?!");
					}
				}
				else
				{
					Debug.Fail("Method has no body anymore?!");
				}
			}
		}
	}
}
