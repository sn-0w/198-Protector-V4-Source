using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000C9 RID: 201
	internal interface IKeyDeriver
	{
		// Token: 0x06000350 RID: 848
		void Init(ConfuserContext ctx, RandomGenerator random);

		// Token: 0x06000351 RID: 849
		uint[] DeriveKey(uint[] a, uint[] b);

		// Token: 0x06000352 RID: 850
		IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src);
	}
}
