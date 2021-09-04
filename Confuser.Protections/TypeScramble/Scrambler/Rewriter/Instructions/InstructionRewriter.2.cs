using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x0200003E RID: 62
	internal abstract class InstructionRewriter<T> : InstructionRewriter
	{
		// Token: 0x06000169 RID: 361 RVA: 0x000028EF File Offset: 0x00000AEF
		internal override void ProcessInstruction(TypeService service, MethodDef method, IList<Instruction> body, ref int index, Instruction i)
		{
			this.ProcessOperand(service, method, body, ref index, (T)((object)i.Operand));
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000290A File Offset: 0x00000B0A
		internal override Type TargetType()
		{
			return typeof(T);
		}

		// Token: 0x0600016B RID: 363
		internal abstract void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, T operand);
	}
}
