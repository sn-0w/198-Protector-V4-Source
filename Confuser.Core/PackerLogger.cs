using System;

namespace Confuser.Core
{
	// Token: 0x0200005F RID: 95
	internal class PackerLogger : ILogger
	{
		// Token: 0x0600024A RID: 586 RVA: 0x00002EF5 File Offset: 0x000010F5
		public PackerLogger(ILogger baseLogger)
		{
			this.baseLogger = baseLogger;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00002F06 File Offset: 0x00001106
		public void Debug(string msg)
		{
			this.baseLogger.Debug(msg);
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00002F16 File Offset: 0x00001116
		public void DebugFormat(string format, params object[] args)
		{
			this.baseLogger.DebugFormat(format, args);
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00002F27 File Offset: 0x00001127
		public void Info(string msg)
		{
			this.baseLogger.Info(msg);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00002F37 File Offset: 0x00001137
		public void InfoFormat(string format, params object[] args)
		{
			this.baseLogger.InfoFormat(format, args);
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00002F48 File Offset: 0x00001148
		public void Warn(string msg)
		{
			this.baseLogger.Warn(msg);
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00002F58 File Offset: 0x00001158
		public void WarnFormat(string format, params object[] args)
		{
			this.baseLogger.WarnFormat(format, args);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00002F69 File Offset: 0x00001169
		public void WarnException(string msg, Exception ex)
		{
			this.baseLogger.WarnException(msg, ex);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00002F7A File Offset: 0x0000117A
		public void Error(string msg)
		{
			this.baseLogger.Error(msg);
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00002F8A File Offset: 0x0000118A
		public void ErrorFormat(string format, params object[] args)
		{
			this.baseLogger.ErrorFormat(format, args);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00002F9B File Offset: 0x0000119B
		public void ErrorException(string msg, Exception ex)
		{
			this.baseLogger.ErrorException(msg, ex);
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00002FAC File Offset: 0x000011AC
		public void Progress(int progress, int overall)
		{
			this.baseLogger.Progress(progress, overall);
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00002FBD File Offset: 0x000011BD
		public void EndProgress()
		{
			this.baseLogger.EndProgress();
		}

		// Token: 0x06000257 RID: 599 RVA: 0x000115E0 File Offset: 0x0000F7E0
		public void Finish(bool successful)
		{
			bool flag = !successful;
			if (flag)
			{
				throw new ConfuserException(null);
			}
			this.baseLogger.Info("Finish protecting packer stub.");
		}

		// Token: 0x040001CF RID: 463
		private readonly ILogger baseLogger;
	}
}
