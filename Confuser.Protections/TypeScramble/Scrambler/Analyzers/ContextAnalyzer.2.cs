using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x02000048 RID: 72
	internal abstract class ContextAnalyzer<T> : ContextAnalyzer
	{
		// Token: 0x06000198 RID: 408 RVA: 0x0000290A File Offset: 0x00000B0A
		internal override Type TargetType()
		{
			return typeof(T);
		}

		// Token: 0x06000199 RID: 409
		internal abstract void Process(ScannedMethod method, Instruction instruction, T operand);

		// Token: 0x0600019A RID: 410 RVA: 0x000029B5 File Offset: 0x00000BB5
		internal override void ProcessOperand(ScannedMethod method, Instruction instruction, object operand)
		{
			this.Process(method, instruction, (T)((object)operand));
		}
	}
}
