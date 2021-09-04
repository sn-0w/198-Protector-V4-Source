using System;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000031 RID: 49
	public abstract class TabViewModel : ViewModelBase
	{
		// Token: 0x0600017C RID: 380 RVA: 0x00002C79 File Offset: 0x00002C79
		protected TabViewModel(AppVM app, string header)
		{
			this.App = app;
			this.Header = header;
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00002C93 File Offset: 0x00002C93
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00002C9B File Offset: 0x00002C9B
		public AppVM App { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00002CA4 File Offset: 0x00002CA4
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00002CAC File Offset: 0x00002CAC
		public string Header { get; private set; }
	}
}
