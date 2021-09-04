using System;

namespace Confuser.Runtime
{
	// Token: 0x02000034 RID: 52
	internal class RefProxyKey : Attribute
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x0000268C File Offset: 0x0000088C
		public RefProxyKey(int key)
		{
			this.key = Mutation.Placeholder<int>(key);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000026A0 File Offset: 0x000008A0
		public override int GetHashCode()
		{
			return this.key;
		}

		// Token: 0x040000B8 RID: 184
		private readonly int key;
	}
}
