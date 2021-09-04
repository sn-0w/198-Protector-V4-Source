using System;

namespace Confuser.Core
{
	// Token: 0x02000065 RID: 101
	public abstract class ProtectionPhase
	{
		// Token: 0x06000264 RID: 612 RVA: 0x00003050 File Offset: 0x00001250
		public ProtectionPhase(ConfuserComponent parent)
		{
			this.Parent = parent;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000265 RID: 613 RVA: 0x00003062 File Offset: 0x00001262
		// (set) Token: 0x06000266 RID: 614 RVA: 0x0000306A File Offset: 0x0000126A
		public ConfuserComponent Parent { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000267 RID: 615
		public abstract ProtectionTargets Targets { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000268 RID: 616
		public abstract string Name { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000269 RID: 617 RVA: 0x00011680 File Offset: 0x0000F880
		public virtual bool ProcessAll
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600026A RID: 618
		protected internal abstract void Execute(ConfuserContext context, ProtectionParameters parameters);
	}
}
