using System;

namespace Confuser.Core
{
	// Token: 0x0200003A RID: 58
	public class ConfuserException : Exception
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00002855 File Offset: 0x00000A55
		public ConfuserException(Exception innerException) : base("Exception occurred during the protection process.", innerException)
		{
		}
	}
}
