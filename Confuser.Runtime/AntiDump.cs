using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000005 RID: 5
	internal static class AntiDump
	{
		// Token: 0x0600000F RID: 15
		[DllImport("kernel32.dll")]
		private unsafe static extern bool VirtualProtect(byte* lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x06000010 RID: 16 RVA: 0x00002B50 File Offset: 0x00000D50
		private unsafe static void Initialize()
		{
			Module module = typeof(AntiDump).Module;
			byte* ptr = (byte*)((void*)Marshal.GetHINSTANCE(module));
			byte* ptr2 = ptr + 60;
			ptr2 = ptr + *(uint*)ptr2;
			ptr2 += 6;
			ushort num = *(ushort*)ptr2;
			ptr2 += 14;
			ushort num2 = *(ushort*)ptr2;
			ptr2 = ptr2 + 4 + num2;
			byte* ptr3 = stackalloc byte[(UIntPtr)11];
			uint num3;
			if (module.FullyQualifiedName[0] != '<')
			{
				AntiDump.VirtualProtect(ptr2 - 16, 8, 64U, out num3);
				*(int*)(ptr2 - 12) = 0;
				byte* ptr4 = ptr + *(uint*)(ptr2 - 16);
				*(int*)(ptr2 - 16) = 0;
				if (*(uint*)(ptr2 - 120) != 0U)
				{
					byte* ptr5 = ptr + *(uint*)(ptr2 - 120);
					byte* ptr6 = ptr + *(uint*)ptr5;
					byte* ptr7 = ptr + *(uint*)(ptr5 + 12);
					byte* ptr8 = ptr + *(uint*)ptr6 + 2;
					AntiDump.VirtualProtect(ptr7, 11, 64U, out num3);
					*(int*)ptr3 = 1818522734;
					*(int*)(ptr3 + 4) = 1818504812;
					*(short*)(ptr3 + (IntPtr)4 * 2) = 108;
					ptr3[10] = 0;
					for (int i = 0; i < 11; i++)
					{
						ptr7[i] = ptr3[i];
					}
					AntiDump.VirtualProtect(ptr8, 11, 64U, out num3);
					*(int*)ptr3 = 1866691662;
					*(int*)(ptr3 + 4) = 1852404846;
					*(short*)(ptr3 + (IntPtr)4 * 2) = 25973;
					ptr3[10] = 0;
					for (int j = 0; j < 11; j++)
					{
						ptr8[j] = ptr3[j];
					}
				}
				for (int k = 0; k < (int)num; k++)
				{
					AntiDump.VirtualProtect(ptr2, 8, 64U, out num3);
					Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr2), 8);
					ptr2 += 40;
				}
				AntiDump.VirtualProtect(ptr4, 72, 64U, out num3);
				byte* ptr9 = ptr + *(uint*)(ptr4 + 8);
				*(int*)ptr4 = 0;
				*(int*)(ptr4 + 4) = 0;
				*(int*)(ptr4 + (IntPtr)2 * 4) = 0;
				*(int*)(ptr4 + (IntPtr)3 * 4) = 0;
				AntiDump.VirtualProtect(ptr9, 4, 64U, out num3);
				*(int*)ptr9 = 0;
				ptr9 += 12;
				ptr9 += *(uint*)ptr9;
				ptr9 = (ptr9 + 7L & -4L);
				ptr9 += 2;
				ushort num4 = (ushort)(*ptr9);
				ptr9 += 2;
				for (int l = 0; l < (int)num4; l++)
				{
					AntiDump.VirtualProtect(ptr9, 8, 64U, out num3);
					*(int*)ptr9 = 0;
					ptr9 += 4;
					*(int*)ptr9 = 0;
					ptr9 += 4;
					for (int m = 0; m < 8; m++)
					{
						AntiDump.VirtualProtect(ptr9, 4, 64U, out num3);
						*ptr9 = 0;
						ptr9++;
						if (*ptr9 == 0)
						{
							ptr9 += 3;
							break;
						}
						*ptr9 = 0;
						ptr9++;
						if (*ptr9 == 0)
						{
							ptr9 += 2;
							break;
						}
						*ptr9 = 0;
						ptr9++;
						if (*ptr9 == 0)
						{
							ptr9++;
							break;
						}
						*ptr9 = 0;
						ptr9++;
					}
				}
				return;
			}
			AntiDump.VirtualProtect(ptr2 - 16, 8, 64U, out num3);
			*(int*)(ptr2 - 12) = 0;
			uint num5 = *(uint*)(ptr2 - 16);
			*(int*)(ptr2 - 16) = 0;
			uint num6 = *(uint*)(ptr2 - 120);
			uint[] array = new uint[(int)num];
			uint[] array2 = new uint[(int)num];
			uint[] array3 = new uint[(int)num];
			for (int n = 0; n < (int)num; n++)
			{
				AntiDump.VirtualProtect(ptr2, 8, 64U, out num3);
				Marshal.Copy(new byte[8], 0, (IntPtr)((void*)ptr2), 8);
				array[n] = *(uint*)(ptr2 + 12);
				array2[n] = *(uint*)(ptr2 + 8);
				array3[n] = *(uint*)(ptr2 + 20);
				ptr2 += 40;
			}
			if (num6 != 0U)
			{
				for (int num7 = 0; num7 < (int)num; num7++)
				{
					if (array[num7] <= num6 && num6 < array[num7] + array2[num7])
					{
						num6 = num6 - array[num7] + array3[num7];
						break;
					}
				}
				byte* ptr10 = ptr + num6;
				uint num8 = *(uint*)ptr10;
				for (int num9 = 0; num9 < (int)num; num9++)
				{
					if (array[num9] <= num8 && num8 < array[num9] + array2[num9])
					{
						num8 = num8 - array[num9] + array3[num9];
						break;
					}
				}
				byte* ptr11 = ptr + num8;
				uint num10 = *(uint*)(ptr10 + 12);
				for (int num11 = 0; num11 < (int)num; num11++)
				{
					if (array[num11] <= num10 && num10 < array[num11] + array2[num11])
					{
						num10 = num10 - array[num11] + array3[num11];
						break;
					}
				}
				uint num12 = *(uint*)ptr11 + 2U;
				for (int num13 = 0; num13 < (int)num; num13++)
				{
					if (array[num13] <= num12 && num12 < array[num13] + array2[num13])
					{
						num12 = num12 - array[num13] + array3[num13];
						break;
					}
				}
				AntiDump.VirtualProtect(ptr + num10, 11, 64U, out num3);
				*(int*)ptr3 = 1818522734;
				*(int*)(ptr3 + 4) = 1818504812;
				*(short*)(ptr3 + (IntPtr)4 * 2) = 108;
				ptr3[10] = 0;
				for (int num14 = 0; num14 < 11; num14++)
				{
					(ptr + num10)[num14] = ptr3[num14];
				}
				AntiDump.VirtualProtect(ptr + num12, 11, 64U, out num3);
				*(int*)ptr3 = 1866691662;
				*(int*)(ptr3 + 4) = 1852404846;
				*(short*)(ptr3 + (IntPtr)4 * 2) = 25973;
				ptr3[10] = 0;
				for (int num15 = 0; num15 < 11; num15++)
				{
					(ptr + num12)[num15] = ptr3[num15];
				}
			}
			for (int num16 = 0; num16 < (int)num; num16++)
			{
				if (array[num16] <= num5 && num5 < array[num16] + array2[num16])
				{
					num5 = num5 - array[num16] + array3[num16];
					break;
				}
			}
			byte* ptr12 = ptr + num5;
			AntiDump.VirtualProtect(ptr12, 72, 64U, out num3);
			uint num17 = *(uint*)(ptr12 + 8);
			for (int num18 = 0; num18 < (int)num; num18++)
			{
				if (array[num18] <= num17 && num17 < array[num18] + array2[num18])
				{
					num17 = num17 - array[num18] + array3[num18];
					break;
				}
			}
			*(int*)ptr12 = 0;
			*(int*)(ptr12 + 4) = 0;
			*(int*)(ptr12 + (IntPtr)2 * 4) = 0;
			*(int*)(ptr12 + (IntPtr)3 * 4) = 0;
			byte* ptr13 = ptr + num17;
			AntiDump.VirtualProtect(ptr13, 4, 64U, out num3);
			*(int*)ptr13 = 0;
			ptr13 += 12;
			ptr13 += *(uint*)ptr13;
			ptr13 = (ptr13 + 7L & -4L);
			ptr13 += 2;
			ushort num19 = (ushort)(*ptr13);
			ptr13 += 2;
			for (int num20 = 0; num20 < (int)num19; num20++)
			{
				AntiDump.VirtualProtect(ptr13, 8, 64U, out num3);
				*(int*)ptr13 = 0;
				ptr13 += 4;
				*(int*)ptr13 = 0;
				ptr13 += 4;
				for (int num21 = 0; num21 < 8; num21++)
				{
					AntiDump.VirtualProtect(ptr13, 4, 64U, out num3);
					*ptr13 = 0;
					ptr13++;
					if (*ptr13 == 0)
					{
						ptr13 += 3;
						break;
					}
					*ptr13 = 0;
					ptr13++;
					if (*ptr13 == 0)
					{
						ptr13 += 2;
						break;
					}
					*ptr13 = 0;
					ptr13++;
					if (*ptr13 == 0)
					{
						ptr13++;
						break;
					}
					*ptr13 = 0;
					ptr13++;
				}
			}
		}
	}
}
