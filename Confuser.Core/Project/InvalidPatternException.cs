using System;

namespace Confuser.Core.Project
{
	// Token: 0x02000089 RID: 137
	public class InvalidPatternException : Exception
	{
		// Token: 0x06000352 RID: 850 RVA: 0x00003710 File Offset: 0x00001910
		public InvalidPatternException(string message) : base(message)
		{
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000371B File Offset: 0x0000191B
		public InvalidPatternException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
