using System;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x0200005D RID: 93
	internal class NullLogger : ILogger
	{
		// Token: 0x06000236 RID: 566 RVA: 0x00002583 File Offset: 0x00000783
		private NullLogger()
		{
		}

		// Token: 0x06000237 RID: 567 RVA: 0x000026EF File Offset: 0x000008EF
		public void Debug(string msg)
		{
		}

		// Token: 0x06000238 RID: 568 RVA: 0x000026EF File Offset: 0x000008EF
		public void DebugFormat(string format, params object[] args)
		{
		}

		// Token: 0x06000239 RID: 569 RVA: 0x000026EF File Offset: 0x000008EF
		public void Info(string msg)
		{
		}

		// Token: 0x0600023A RID: 570 RVA: 0x000026EF File Offset: 0x000008EF
		public void InfoFormat(string format, params object[] args)
		{
		}

		// Token: 0x0600023B RID: 571 RVA: 0x000026EF File Offset: 0x000008EF
		public void Warn(string msg)
		{
		}

		// Token: 0x0600023C RID: 572 RVA: 0x000026EF File Offset: 0x000008EF
		public void WarnFormat(string format, params object[] args)
		{
		}

		// Token: 0x0600023D RID: 573 RVA: 0x000026EF File Offset: 0x000008EF
		public void WarnException(string msg, Exception ex)
		{
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000026EF File Offset: 0x000008EF
		public void Error(string msg)
		{
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000026EF File Offset: 0x000008EF
		public void ErrorFormat(string format, params object[] args)
		{
		}

		// Token: 0x06000240 RID: 576 RVA: 0x000026EF File Offset: 0x000008EF
		public void ErrorException(string msg, Exception ex)
		{
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000026EF File Offset: 0x000008EF
		public void Progress(int overall, int progress)
		{
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000026EF File Offset: 0x000008EF
		public void EndProgress()
		{
		}

		// Token: 0x06000243 RID: 579 RVA: 0x000026EF File Offset: 0x000008EF
		public void Finish(bool successful)
		{
		}

		// Token: 0x06000244 RID: 580 RVA: 0x000026EF File Offset: 0x000008EF
		public void BeginModule(ModuleDef module)
		{
		}

		// Token: 0x06000245 RID: 581 RVA: 0x000026EF File Offset: 0x000008EF
		public void EndModule(ModuleDef module)
		{
		}

		// Token: 0x040001CE RID: 462
		public static readonly NullLogger Instance = new NullLogger();
	}
}
