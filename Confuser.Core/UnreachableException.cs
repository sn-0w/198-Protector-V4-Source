using System;

namespace Confuser.Core
{
	// Token: 0x02000070 RID: 112
	public class UnreachableException : SystemException
	{
		// Token: 0x060002BC RID: 700 RVA: 0x000032FE File Offset: 0x000014FE
		public UnreachableException() : base("Unreachable code reached.")
		{
		}
	}
}
