using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x0200003D RID: 61
	internal abstract class InstructionRewriter
	{
		// Token: 0x06000166 RID: 358
		internal abstract void ProcessInstruction(TypeService service, MethodDef method, IList<Instruction> body, ref int index, Instruction i);

		// Token: 0x06000167 RID: 359
		internal abstract Type TargetType();
	}
}
