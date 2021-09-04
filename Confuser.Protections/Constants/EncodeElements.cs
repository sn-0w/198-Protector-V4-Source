using System;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000AD RID: 173
	[Flags]
	internal enum EncodeElements
	{
		// Token: 0x0400015C RID: 348
		Strings = 1,
		// Token: 0x0400015D RID: 349
		Numbers = 2,
		// Token: 0x0400015E RID: 350
		Primitive = 4,
		// Token: 0x0400015F RID: 351
		Initializers = 8
	}
}
