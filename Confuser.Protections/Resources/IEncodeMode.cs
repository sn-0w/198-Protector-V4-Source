using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources
{
	// Token: 0x02000050 RID: 80
	internal interface IEncodeMode
	{
		// Token: 0x060001B1 RID: 433
		IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key);

		// Token: 0x060001B2 RID: 434
		uint[] Encrypt(uint[] data, int offset, uint[] key);
	}
}
