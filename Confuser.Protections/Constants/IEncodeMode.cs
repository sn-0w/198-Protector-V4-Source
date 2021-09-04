using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000B4 RID: 180
	internal interface IEncodeMode
	{
		// Token: 0x06000308 RID: 776
		IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key);

		// Token: 0x06000309 RID: 777
		uint[] Encrypt(uint[] data, int offset, uint[] key);

		// Token: 0x0600030A RID: 778
		object CreateDecoder(MethodDef decoder, CEContext ctx);

		// Token: 0x0600030B RID: 779
		uint Encode(object data, CEContext ctx, uint id);
	}
}
