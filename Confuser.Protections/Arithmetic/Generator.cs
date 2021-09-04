using System;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DF RID: 223
	public class Generator
	{
		// Token: 0x060003B4 RID: 948 RVA: 0x00019978 File Offset: 0x00017B78
		public Generator()
		{
			this.random = new Random(Guid.NewGuid().GetHashCode());
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x000034B1 File Offset: 0x000016B1
		public int Next()
		{
			return this.random.Next(int.MaxValue);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000034C3 File Offset: 0x000016C3
		public int Next(int value)
		{
			return this.random.Next(value);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000034D1 File Offset: 0x000016D1
		public int Next(int min, int max)
		{
			return this.random.Next(min, max);
		}

		// Token: 0x040001DD RID: 477
		private Random random;
	}
}
