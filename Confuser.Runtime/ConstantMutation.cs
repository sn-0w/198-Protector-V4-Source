using System;

namespace Confuser.Runtime
{
	// Token: 0x02000028 RID: 40
	internal static class ConstantMutation
	{
		// Token: 0x0600008E RID: 142 RVA: 0x00002441 File Offset: 0x00000641
		public static int ConjetMe(int i)
		{
			while (i != 1)
			{
				if (i % 2 == 0)
				{
					i /= 2;
				}
				else
				{
					i = 3 * i + 1;
				}
			}
			return i;
		}
	}
}
