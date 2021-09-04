using System;

namespace Confuser.DynCipher
{
	// Token: 0x02000005 RID: 5
	public static class MathsUtils
	{
		// Token: 0x0600000E RID: 14 RVA: 0x0000286C File Offset: 0x00000A6C
		public static ulong modInv(ulong num, ulong mod)
		{
			ulong num2 = mod;
			ulong num3 = num % mod;
			ulong num4 = 0UL;
			ulong num5 = 1UL;
			while (num3 > 0UL)
			{
				bool flag = num3 == 1UL;
				ulong result;
				if (flag)
				{
					result = num5;
				}
				else
				{
					num4 += num2 / num3 * num5;
					num2 %= num3;
					bool flag2 = num2 == 0UL;
					if (flag2)
					{
						break;
					}
					bool flag3 = num2 == 1UL;
					if (!flag3)
					{
						num5 += num3 / num2 * num4;
						num3 %= num2;
						continue;
					}
					result = mod - num4;
				}
				return result;
			}
			return 0UL;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000028E4 File Offset: 0x00000AE4
		public static uint modInv(uint num)
		{
			return (uint)MathsUtils.modInv((ulong)num, 4294967296UL);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002908 File Offset: 0x00000B08
		public static byte modInv(byte num)
		{
			return (byte)MathsUtils.modInv((ulong)num, 256UL);
		}

		// Token: 0x04000002 RID: 2
		private const ulong MODULO32 = 4294967296UL;
	}
}
