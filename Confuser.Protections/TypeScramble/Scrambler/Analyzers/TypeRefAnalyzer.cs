using System;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x0200004D RID: 77
	internal sealed class TypeRefAnalyzer : ContextAnalyzer<TypeRef>
	{
		// Token: 0x060001AA RID: 426 RVA: 0x00002A74 File Offset: 0x00000C74
		internal override void Process(ScannedMethod method, Instruction instruction, TypeRef operand)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instruction != null, "instruction != null");
			Debug.Assert(operand != null, "operand != null");
			method.RegisterGeneric(operand.ToTypeSig(true));
		}
	}
}
