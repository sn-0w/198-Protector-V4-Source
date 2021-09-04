using System;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x0200004B RID: 75
	internal sealed class MethodDefAnalyzer : ContextAnalyzer<MethodDef>
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x00002A43 File Offset: 0x00000C43
		private TypeService Service { get; }

		// Token: 0x060001A6 RID: 422 RVA: 0x00002A4B File Offset: 0x00000C4B
		internal MethodDefAnalyzer(TypeService service)
		{
			Debug.Assert(service != null, "service != null");
			this.Service = service;
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00008E3C File Offset: 0x0000703C
		internal override void Process(ScannedMethod method, Instruction instruction, MethodDef operand)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instruction != null, "instruction != null");
			Debug.Assert(operand != null, "operand != null");
			ScannedMethod item = this.Service.GetItem(operand);
			bool flag = item != null && item.IsScambled;
			if (flag)
			{
				foreach (TypeSig t in item.TrueTypes)
				{
					method.RegisterGeneric(t);
				}
			}
		}
	}
}
