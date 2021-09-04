using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000077 RID: 119
	public interface ICompressionService
	{
		// Token: 0x060002DF RID: 735
		MethodDef TryGetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);

		// Token: 0x060002E0 RID: 736
		MethodDef GetRuntimeDecompressor(ModuleDef module, Action<IDnlibDef> init);

		// Token: 0x060002E1 RID: 737
		byte[] Compress(byte[] data, Action<double> progressFunc = null);
	}
}
