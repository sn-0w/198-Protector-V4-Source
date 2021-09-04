using System;

namespace Confuser.Core
{
	// Token: 0x02000045 RID: 69
	public abstract class ConfuserComponent
	{
		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000172 RID: 370
		public abstract string Name { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000173 RID: 371
		public abstract string Description { get; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000174 RID: 372
		public abstract string Author { get; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000175 RID: 373
		public abstract string Id { get; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000176 RID: 374
		public abstract string FullId { get; }

		// Token: 0x06000177 RID: 375
		protected internal abstract void Initialize(ConfuserContext context);

		// Token: 0x06000178 RID: 376
		protected internal abstract void PopulatePipeline(ProtectionPipeline pipeline);
	}
}
