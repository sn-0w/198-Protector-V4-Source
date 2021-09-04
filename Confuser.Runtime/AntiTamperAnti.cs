using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x0200000E RID: 14
	internal static class AntiTamperAnti
	{
		// Token: 0x06000058 RID: 88
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x06000059 RID: 89
		[DllImport("kernel32.dll", EntryPoint = "VirtualProtect")]
		private unsafe static extern bool VirtualProtect1(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x0600005A RID: 90
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, ref bool isDebuggerPresent);

		// Token: 0x0600005B RID: 91 RVA: 0x000045A0 File Offset: 0x000027A0
		private unsafe static void Initialize()
		{
			Module module = typeof(AntiTamperAnti).Module;
			string fullyQualifiedName = module.FullyQualifiedName;
			bool flag = fullyQualifiedName.Length > 0 && fullyQualifiedName[0] == '<';
			byte* ptr = (byte*)((void*)Marshal.GetHINSTANCE(module));
			byte* ptr2 = ptr + *(uint*)(ptr + 60);
			ushort num = *(ushort*)(ptr2 + 6);
			ushort num2 = *(ushort*)(ptr2 + 20);
			uint* ptr3 = null;
			uint num3 = 0U;
			uint* ptr4 = (uint*)(ptr2 + 24 + num2);
			uint num4 = (uint)Mutation.KeyI1;
			uint num5 = (uint)Mutation.KeyI2;
			uint num6 = (uint)Mutation.KeyI3;
			uint num7 = (uint)Mutation.KeyI4;
			for (int i = 0; i < (int)num; i++)
			{
				uint num8 = *(ptr4++) * *(ptr4++);
				if (num8 == (uint)Mutation.KeyI0)
				{
					ptr3 = (uint*)(ptr + (UIntPtr)(flag ? ptr4[3] : ptr4[1]) / 4);
					num3 = (flag ? ptr4[2] : (*ptr4)) >> 2;
				}
				else if (num8 != 0U)
				{
					uint* ptr5 = (uint*)(ptr + (UIntPtr)(flag ? ptr4[3] : ptr4[1]) / 4);
					uint num9 = ptr4[2] >> 2;
					for (uint num10 = 0U; num10 < num9; num10 += 1U)
					{
						uint num11 = (num4 ^ *(ptr5++)) + num5 + num6 * num7;
						num4 = num5;
						num5 = num7;
						num7 = num11;
					}
				}
				ptr4 += 8;
			}
			uint[] array = new uint[16];
			uint[] array2 = new uint[16];
			for (int j = 0; j < 16; j++)
			{
				array[j] = num7;
				array2[j] = num5;
				num4 = (num5 >> 5 | num5 << 27);
				num5 = (num6 >> 3 | num6 << 29);
				num6 = (num7 >> 7 | num7 << 25);
				num7 = (num4 >> 11 | num4 << 21);
			}
			Mutation.Crypt(array, array2);
			uint num12 = 64U;
			AntiTamperAnti.VirtualProtect((IntPtr)((void*)ptr3), num3 << 2, num12, out num12);
			if (num12 == 64U)
			{
				return;
			}
			byte* ptr6 = (byte*)((void*)Marshal.GetHINSTANCE(typeof(AntiDump).Module));
			byte* ptr7 = ptr6 + 60;
			ptr7 = ptr6 + *(uint*)ptr7;
			ptr7 += 6;
			ushort num13 = *(ushort*)ptr7;
			ptr7 += 14;
			ushort num14 = *(ushort*)ptr7;
			ptr7 = ptr7 + 4 + num14;
			byte* ptr8 = stackalloc byte[(UIntPtr)11];
			uint num15;
			AntiTamperAnti.VirtualProtect1(ptr7 - 16, 8, 64U, out num15);
			*(int*)(ptr7 - 12) = 0;
			byte* ptr9 = ptr6 + *(uint*)(ptr7 - 16);
			*(int*)(ptr7 - 16) = 0;
			AntiTamperAnti.VirtualProtect1(ptr9, 72, 64U, out num15);
			byte* ptr10 = ptr6 + *(uint*)(ptr9 + 8);
			*(int*)ptr9 = 0;
			*(int*)(ptr9 + 4) = 0;
			*(int*)(ptr9 + (IntPtr)2 * 4) = 0;
			*(int*)(ptr9 + (IntPtr)3 * 4) = 0;
			AntiTamperAnti.VirtualProtect1(ptr10, 4, 64U, out num15);
			*(int*)ptr10 = 0;
			for (int k = 0; k < (int)num13; k++)
			{
				AntiTamperAnti.VirtualProtect1(ptr7, 8, 64U, out num15);
				Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr7), 8);
				ptr7 += 40;
			}
			uint num16 = 0U;
			for (uint num17 = 0U; num17 < num3; num17 += 1U)
			{
				*ptr3 ^= array[(int)(num16 & 15U)];
				array[(int)(num16 & 15U)] = (array[(int)(num16 & 15U)] ^ *(ptr3++)) + 1035675673U;
				num16 += 1U;
			}
		}
	}
}
