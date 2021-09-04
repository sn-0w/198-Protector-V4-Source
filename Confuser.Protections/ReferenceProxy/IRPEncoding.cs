using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000064 RID: 100
	internal interface IRPEncoding
	{
		// Token: 0x060001F1 RID: 497
		Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg);

		// Token: 0x060001F2 RID: 498
		int Encode(MethodDef init, RPContext ctx, int value);
	}
}
