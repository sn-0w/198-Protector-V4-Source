using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000058 RID: 88
	internal static class OverwritesHeaders
	{
		// Token: 0x06000120 RID: 288
		[DllImport("kernel32.dll")]
		public static extern IntPtr ZeroMemory(IntPtr addr, IntPtr size);

		// Token: 0x06000121 RID: 289
		[DllImport("kernel32.dll")]
		public static extern IntPtr VirtualProtect(IntPtr lpAddress, IntPtr dwSize, IntPtr flNewProtect, ref IntPtr lpflOldProtect);

		// Token: 0x06000122 RID: 290 RVA: 0x00007FCC File Offset: 0x000061CC
		private static void Initialize()
		{
			List<int> list = new List<int>
			{
				8,
				12,
				16,
				20,
				24,
				28,
				36
			};
			List<int> list2 = new List<int>
			{
				26,
				27
			};
			List<int> list3 = new List<int>
			{
				4,
				22,
				24,
				64,
				66,
				68,
				70,
				72,
				74,
				76,
				92,
				94
			};
			IntPtr baseAddress = Process.GetCurrentProcess().MainModule.BaseAddress;
			int num = Marshal.ReadInt32((IntPtr)(baseAddress.ToInt32() + 60));
			short num2 = Marshal.ReadInt16((IntPtr)(baseAddress.ToInt32() + num + 6));
			for (int i = 0; i < list3.Count; i++)
			{
				OverwritesHeaders.EraseSection((IntPtr)(baseAddress.ToInt32() + num + list3[i]), 1);
			}
			for (int j = 0; j < list2.Count; j++)
			{
				OverwritesHeaders.EraseSection((IntPtr)(baseAddress.ToInt32() + num + list2[j]), 2);
			}
			int k = 0;
			int num3 = 0;
			while (k <= (int)num2)
			{
				if (num3 == 0)
				{
					OverwritesHeaders.EraseSection((IntPtr)(baseAddress.ToInt32() + num + 250 + 40 * k + 32), 2);
				}
				num3++;
				if (num3 == list.Count)
				{
					k++;
					num3 = 0;
				}
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00008184 File Offset: 0x00006384
		public static void EraseSection(IntPtr address, int size)
		{
			IntPtr intPtr = (IntPtr)size;
			IntPtr flNewProtect = 0;
			OverwritesHeaders.VirtualProtect(address, intPtr, (IntPtr)64, ref flNewProtect);
			OverwritesHeaders.ZeroMemory(address, intPtr);
			IntPtr intPtr2 = 0;
			OverwritesHeaders.VirtualProtect(address, intPtr, flNewProtect, ref intPtr2);
		}
	}
}
