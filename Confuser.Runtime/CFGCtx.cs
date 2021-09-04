using System;

namespace Confuser.Runtime
{
	// Token: 0x02000012 RID: 18
	internal struct CFGCtx
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00004FCC File Offset: 0x000031CC
		public CFGCtx(uint seed)
		{
			seed = (this.A = seed * 557916961U);
			seed = (this.B = seed * 557916961U);
			seed = (this.C = seed * 557916961U);
			seed = (this.D = seed * 557916961U);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000501C File Offset: 0x0000321C
		public uint Next(byte f, uint q)
		{
			if ((f & 128) != 0)
			{
				switch (f & 3)
				{
				case 0:
					this.A = q;
					break;
				case 1:
					this.B = q;
					break;
				case 2:
					this.C = q;
					break;
				case 3:
					this.D = q;
					break;
				}
			}
			else
			{
				switch (f & 3)
				{
				case 0:
					this.A ^= q;
					break;
				case 1:
					this.B += q;
					break;
				case 2:
					this.C ^= q;
					break;
				case 3:
					this.D -= q;
					break;
				}
			}
			switch (f >> 2 & 3)
			{
			case 0:
				return this.A;
			case 1:
				return this.B;
			case 2:
				return this.C;
			default:
				return this.D;
			}
		}

		// Token: 0x04000033 RID: 51
		private uint A;

		// Token: 0x04000034 RID: 52
		private uint B;

		// Token: 0x04000035 RID: 53
		private uint C;

		// Token: 0x04000036 RID: 54
		private uint D;
	}
}
