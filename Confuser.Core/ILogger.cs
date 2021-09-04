using System;

namespace Confuser.Core
{
	// Token: 0x0200004C RID: 76
	public interface ILogger
	{
		// Token: 0x060001D8 RID: 472
		void Debug(string msg);

		// Token: 0x060001D9 RID: 473
		void DebugFormat(string format, params object[] args);

		// Token: 0x060001DA RID: 474
		void Info(string msg);

		// Token: 0x060001DB RID: 475
		void InfoFormat(string format, params object[] args);

		// Token: 0x060001DC RID: 476
		void Warn(string msg);

		// Token: 0x060001DD RID: 477
		void WarnFormat(string format, params object[] args);

		// Token: 0x060001DE RID: 478
		void WarnException(string msg, Exception ex);

		// Token: 0x060001DF RID: 479
		void Error(string msg);

		// Token: 0x060001E0 RID: 480
		void ErrorFormat(string format, params object[] args);

		// Token: 0x060001E1 RID: 481
		void ErrorException(string msg, Exception ex);

		// Token: 0x060001E2 RID: 482
		void Progress(int progress, int overall);

		// Token: 0x060001E3 RID: 483
		void EndProgress();

		// Token: 0x060001E4 RID: 484
		void Finish(bool successful);
	}
}
