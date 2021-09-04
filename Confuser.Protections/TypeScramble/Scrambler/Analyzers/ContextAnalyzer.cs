using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x02000047 RID: 71
	internal abstract class ContextAnalyzer
	{
		// Token: 0x06000195 RID: 405
		internal abstract Type TargetType();

		// Token: 0x06000196 RID: 406
		internal abstract void ProcessOperand(ScannedMethod method, Instruction instruction, object operand);
	}
}
