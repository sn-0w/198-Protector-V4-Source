using System;

namespace Confuser.Core
{
	// Token: 0x0200006A RID: 106
	public enum PipelineStage
	{
		// Token: 0x040001F8 RID: 504
		Inspection,
		// Token: 0x040001F9 RID: 505
		BeginModule,
		// Token: 0x040001FA RID: 506
		ProcessModule,
		// Token: 0x040001FB RID: 507
		OptimizeMethods,
		// Token: 0x040001FC RID: 508
		EndModule,
		// Token: 0x040001FD RID: 509
		WriteModule,
		// Token: 0x040001FE RID: 510
		Debug,
		// Token: 0x040001FF RID: 511
		Pack,
		// Token: 0x04000200 RID: 512
		SaveModules
	}
}
