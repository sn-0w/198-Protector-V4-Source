using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000039 RID: 57
	internal static class AntiTamperNormal
	{
		// Token: 0x060000CB RID: 203
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x060000CC RID: 204 RVA: 0x000074AC File Offset: 0x000056AC
		private unsafe static void Initialize()
		{
			Module module = typeof(AntiTamperNormal).Module;
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
			AntiTamperNormal.VirtualProtect((IntPtr)((void*)ptr3), num3 << 2, num12, out num12);
			if (num12 == 64U)
			{
				return;
			}
			uint num13 = 0U;
			for (uint num14 = 0U; num14 < num3; num14 += 1U)
			{
				*ptr3 ^= array[(int)(num13 & 15U)];
				array[(int)(num13 & 15U)] = (array[(int)(num13 & 15U)] ^ *(ptr3++)) + 1035675673U;
				num13 += 1U;
			}
		}
	}
}
