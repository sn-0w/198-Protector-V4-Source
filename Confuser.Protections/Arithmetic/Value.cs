using System;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DD RID: 221
	public class Value
	{
		// Token: 0x060003A9 RID: 937 RVA: 0x0000347C File Offset: 0x0000167C
		public Value(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00003494 File Offset: 0x00001694
		public double GetX()
		{
			return this.x;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0000349C File Offset: 0x0000169C
		public double GetY()
		{
			return this.y;
		}

		// Token: 0x040001DB RID: 475
		private double x;

		// Token: 0x040001DC RID: 476
		private double y;
	}
}
