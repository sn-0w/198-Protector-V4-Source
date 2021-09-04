using System;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x0200004C RID: 76
	internal sealed class MethodSpecAnalyzer : ContextAnalyzer<MethodSpec>
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00008ED8 File Offset: 0x000070D8
		internal override void Process(ScannedMethod method, Instruction instruction, MethodSpec operand)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instruction != null, "instruction != null");
			Debug.Assert(operand != null, "operand != null");
			foreach (TypeSig t in operand.GenericInstMethodSig.GenericArguments)
			{
				method.RegisterGeneric(t);
			}
		}
	}
}
