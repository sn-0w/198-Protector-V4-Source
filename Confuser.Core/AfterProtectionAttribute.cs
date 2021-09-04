using System;

namespace Confuser.Core
{
	// Token: 0x02000064 RID: 100
	[AttributeUsage(AttributeTargets.Class)]
	public class AfterProtectionAttribute : Attribute
	{
		// Token: 0x06000261 RID: 609 RVA: 0x0000302D File Offset: 0x0000122D
		public AfterProtectionAttribute(params string[] ids)
		{
			this.Ids = ids;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0000303F File Offset: 0x0000123F
		// (set) Token: 0x06000263 RID: 611 RVA: 0x00003047 File Offset: 0x00001247
		public string[] Ids { get; private set; }
	}
}
