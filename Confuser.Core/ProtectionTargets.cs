using System;

namespace Confuser.Core
{
	// Token: 0x02000067 RID: 103
	[Flags]
	public enum ProtectionTargets
	{
		// Token: 0x040001D6 RID: 470
		Types = 1,
		// Token: 0x040001D7 RID: 471
		Methods = 2,
		// Token: 0x040001D8 RID: 472
		Fields = 4,
		// Token: 0x040001D9 RID: 473
		Events = 8,
		// Token: 0x040001DA RID: 474
		Properties = 16,
		// Token: 0x040001DB RID: 475
		AllMembers = 31,
		// Token: 0x040001DC RID: 476
		Modules = 32,
		// Token: 0x040001DD RID: 477
		AllDefinitions = 63
	}
}
