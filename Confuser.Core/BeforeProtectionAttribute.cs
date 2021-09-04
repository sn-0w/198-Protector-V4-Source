using System;

namespace Confuser.Core
{
	// Token: 0x02000063 RID: 99
	[AttributeUsage(AttributeTargets.Class)]
	public class BeforeProtectionAttribute : Attribute
	{
		// Token: 0x0600025E RID: 606 RVA: 0x0000300A File Offset: 0x0000120A
		public BeforeProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600025F RID: 607 RVA: 0x0000301C File Offset: 0x0000121C
		// (set) Token: 0x06000260 RID: 608 RVA: 0x00003024 File Offset: 0x00001224
		public string[] Ids { get; private set; }
	}
}
