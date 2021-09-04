using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F6 RID: 246
	internal interface IKeyDeriver
	{
		// Token: 0x060003FC RID: 1020
		void Init(ConfuserContext ctx, RandomGenerator random);

		// Token: 0x060003FD RID: 1021
		uint[] DeriveKey(uint[] a, uint[] b);

		// Token: 0x060003FE RID: 1022
		IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src);
	}
}
