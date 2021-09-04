using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000C7 RID: 199
	public interface IOpaquePredicateDescriptor
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600047D RID: 1149
		OpaquePredicateType Type { get; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600047E RID: 1150
		int ArgumentCount { get; }

		// Token: 0x0600047F RID: 1151
		bool IsUsable(MethodDef method);

		// Token: 0x06000480 RID: 1152
		IOpaquePredicate CreatePredicate(MethodDef method);
	}
}
