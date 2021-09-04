using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000C5 RID: 197
	public interface IDataStore
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000478 RID: 1144
		int Priority { get; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000479 RID: 1145
		int KeyCount { get; }

		// Token: 0x0600047A RID: 1146
		bool IsUsable(MethodDef method);

		// Token: 0x0600047B RID: 1147
		IDataStoreAccessor CreateAccessor(MethodDef method, uint[] keys, byte[] data);
	}
}
