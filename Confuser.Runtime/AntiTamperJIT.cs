using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000013 RID: 19
	internal static class AntiTamperJIT
	{
		// Token: 0x06000065 RID: 101 RVA: 0x00005104 File Offset: 0x00003304
		public unsafe static void Initialize()
		{
			Module module = typeof(AntiTamperJIT).Module;
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
			uint num12 = 0U;
			uint* ptr6 = ptr3;
			AntiTamperJIT.VirtualProtect((IntPtr)((void*)ptr3), num3 << 2, 64U, out num4);
			for (uint num13 = 0U; num13 < num3; num13 += 1U)
			{
				*ptr3 ^= array[(int)(num12 & 15U)];
				array[(int)(num12 & 15U)] = (array[(int)(num12 & 15U)] ^ *(ptr3++)) + 1035675673U;
				num12 += 1U;
			}
			AntiTamperJIT.ptr = ptr6 + 4;
			AntiTamperJIT.len = *(AntiTamperJIT.ptr++);
			AntiTamperJIT.ver4 = (Environment.Version.Major == 4);
			AntiTamperJIT.ver5 = (AntiTamperJIT.ver4 && Environment.Version.Revision > 17020);
			ModuleHandle moduleHandle = module.ModuleHandle;
			object fieldValue = AntiTamperJIT.GetFieldValue(moduleHandle, "m_ptr");
			if (fieldValue is IntPtr)
			{
				AntiTamperJIT.moduleHnd = (IntPtr)fieldValue;
			}
			else
			{
				if (!(fieldValue.GetType().ToString() == "System.Reflection.RuntimeModule"))
				{
					throw new ApplicationException("Failed to get pointer for module handle: " + moduleHandle.ToString());
				}
				AntiTamperJIT.moduleHnd = (IntPtr)AntiTamperJIT.GetFieldValue(fieldValue, "m_pData");
			}
			AntiTamperJIT.Hook();
		}

		// Token: 0x06000066 RID: 102
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lib);

		// Token: 0x06000067 RID: 103
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr lib, string proc);

		// Token: 0x06000068 RID: 104
		[DllImport("kernel32.dll")]
		private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		// Token: 0x06000069 RID: 105 RVA: 0x000053FC File Offset: 0x000035FC
		private static object GetFieldValue(object obj, string fieldName)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new ApplicationException(string.Format("Could not get field {0}::{1}", obj.GetType(), fieldName));
			}
			return field.GetValue(obj);
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000543C File Offset: 0x0000363C
		private unsafe static void Hook()
		{
			ulong* ptr = stackalloc ulong[(UIntPtr)16];
			if (AntiTamperJIT.ver4)
			{
				*ptr = 7218835248827755619UL;
				ptr[1] = 27756UL;
			}
			else
			{
				*ptr = 8388352820681864045UL;
				ptr[1] = 1819042862UL;
			}
			IntPtr lib = AntiTamperJIT.LoadLibrary(new string((sbyte*)ptr));
			*ptr = 127995569530215UL;
			AntiTamperJIT.getJit getJit = (AntiTamperJIT.getJit)Marshal.GetDelegateForFunctionPointer(AntiTamperJIT.GetProcAddress(lib, new string((sbyte*)ptr)), typeof(AntiTamperJIT.getJit));
			IntPtr intPtr = *getJit();
			IntPtr val = *(IntPtr*)((void*)intPtr);
			IntPtr intPtr2;
			uint num;
			if (IntPtr.Size == 8)
			{
				intPtr2 = Marshal.AllocHGlobal(16);
				ulong* ptr2 = (ulong*)((void*)intPtr2);
				*ptr2 = 18446744073709533256UL;
				ptr2[1] = 10416984890032521215UL;
				AntiTamperJIT.VirtualProtect(intPtr2, 12U, 64U, out num);
				Marshal.WriteIntPtr(intPtr2, 2, val);
			}
			else
			{
				intPtr2 = Marshal.AllocHGlobal(8);
				ulong* ptr3 = (ulong*)((void*)intPtr2);
				*ptr3 = 10439625411221520312UL;
				AntiTamperJIT.VirtualProtect(intPtr2, 7U, 64U, out num);
				Marshal.WriteIntPtr(intPtr2, 1, val);
			}
			AntiTamperJIT.originalDelegate = (AntiTamperJIT.compileMethod)Marshal.GetDelegateForFunctionPointer(intPtr2, typeof(AntiTamperJIT.compileMethod));
			AntiTamperJIT.handler = new AntiTamperJIT.compileMethod(AntiTamperJIT.HookHandler);
			RuntimeHelpers.PrepareDelegate(AntiTamperJIT.originalDelegate);
			RuntimeHelpers.PrepareDelegate(AntiTamperJIT.handler);
			uint flNewProtect;
			AntiTamperJIT.VirtualProtect(intPtr, (uint)IntPtr.Size, 64U, out flNewProtect);
			Marshal.WriteIntPtr(intPtr, Marshal.GetFunctionPointerForDelegate(AntiTamperJIT.handler));
			AntiTamperJIT.VirtualProtect(intPtr, (uint)IntPtr.Size, flNewProtect, out num);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000055BC File Offset: 0x000037BC
		private unsafe static void ExtractLocalVars(AntiTamperJIT.CORINFO_METHOD_INFO* info, uint len, byte* localVar)
		{
			void* ptr2;
			if (AntiTamperJIT.ver4)
			{
				if (IntPtr.Size == 8)
				{
					AntiTamperJIT.CORINFO_METHOD_INFO* ptr = info + 1;
					IntPtr intPtr = AntiTamperJIT.ver5 ? 7 : 5;
					ptr2 = (void*)((byte*)((byte*)ptr + intPtr * 4) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x64));
				}
				else
				{
					AntiTamperJIT.CORINFO_METHOD_INFO* ptr3 = info + 1;
					IntPtr intPtr2 = AntiTamperJIT.ver5 ? 5 : 4;
					ptr2 = (void*)((byte*)((byte*)ptr3 + intPtr2 * 4) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x86));
				}
			}
			else if (IntPtr.Size == 8)
			{
				ptr2 = (void*)(info + 1 + (IntPtr)3 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x64) / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO));
			}
			else
			{
				ptr2 = (void*)(info + 1 + (IntPtr)3 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO) + sizeof(AntiTamperJIT.CORINFO_SIG_INFO_x86) / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO));
			}
			if (IntPtr.Size == 8)
			{
				((AntiTamperJIT.CORINFO_SIG_INFO_x64*)ptr2)->sig = (IntPtr)((void*)localVar);
			}
			else
			{
				((AntiTamperJIT.CORINFO_SIG_INFO_x86*)ptr2)->sig = (IntPtr)((void*)localVar);
			}
			localVar++;
			byte b = *localVar;
			ushort numArgs;
			IntPtr args;
			if ((b & 128) == 0)
			{
				numArgs = (ushort)b;
				args = (IntPtr)((void*)(localVar + 1));
			}
			else
			{
				numArgs = (ushort)(((int)b & -129) << 8 | (int)localVar[1]);
				args = (IntPtr)((void*)(localVar + 2));
			}
			if (IntPtr.Size == 8)
			{
				AntiTamperJIT.CORINFO_SIG_INFO_x64* ptr4 = (AntiTamperJIT.CORINFO_SIG_INFO_x64*)ptr2;
				ptr4->callConv = 0U;
				ptr4->retType = 1;
				ptr4->flags = 1;
				ptr4->numArgs = numArgs;
				ptr4->args = args;
				return;
			}
			AntiTamperJIT.CORINFO_SIG_INFO_x86* ptr5 = (AntiTamperJIT.CORINFO_SIG_INFO_x86*)ptr2;
			ptr5->callConv = 0U;
			ptr5->retType = 1;
			ptr5->flags = 1;
			ptr5->numArgs = numArgs;
			ptr5->args = args;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00005708 File Offset: 0x00003908
		private unsafe static uint HookHandler(IntPtr self, AntiTamperJIT.ICorJitInfo* comp, AntiTamperJIT.CORINFO_METHOD_INFO* info, uint flags, byte** nativeEntry, uint* nativeSizeOfCode)
		{
			if (info != null && info->scope == AntiTamperJIT.moduleHnd && *info->ILCode == 20)
			{
				uint num;
				if (AntiTamperJIT.ver5)
				{
					AntiTamperJIT.getMethodDefFromMethod getMethodDefFromMethod = (AntiTamperJIT.getMethodDefFromMethod)Marshal.GetDelegateForFunctionPointer(comp->vfptr[(IntPtr)100 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getMethodDefFromMethod));
					num = getMethodDefFromMethod((IntPtr)((void*)comp), info->ftn);
				}
				else
				{
					AntiTamperJIT.ICorClassInfo* ptr = AntiTamperJIT.ICorStaticInfo.ICorClassInfo(AntiTamperJIT.ICorDynamicInfo.ICorStaticInfo(AntiTamperJIT.ICorJitInfo.ICorDynamicInfo(comp)));
					int num2 = 12 + (AntiTamperJIT.ver4 ? 2 : 1);
					AntiTamperJIT.getMethodDefFromMethod getMethodDefFromMethod2 = (AntiTamperJIT.getMethodDefFromMethod)Marshal.GetDelegateForFunctionPointer(ptr->vfptr[(IntPtr)num2 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getMethodDefFromMethod));
					num = getMethodDefFromMethod2((IntPtr)((void*)ptr), info->ftn);
				}
				uint num3 = 0U;
				uint num4 = AntiTamperJIT.len;
				uint? num5 = null;
				while (num4 >= num3)
				{
					uint num6 = num3 + (num4 - num3 >> 1);
					uint num7 = AntiTamperJIT.ptr[(ulong)((ulong)num6 << 1) * 4UL / 4UL];
					if (num7 == num)
					{
						num5 = new uint?((AntiTamperJIT.ptr + (ulong)((ulong)num6 << 1) * 4UL / 4UL)[1]);
						break;
					}
					if (num7 < num)
					{
						num3 = num6 + 1U;
					}
					else
					{
						num4 = num6 - 1U;
					}
				}
				if (num5 == null)
				{
					return AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
				}
				uint* ptr2 = AntiTamperJIT.ptr + (ulong)num5.Value * 4UL / 4UL;
				uint num8 = *(ptr2++);
				uint* ptr3 = (uint*)((void*)Marshal.AllocHGlobal((int)((int)num8 << 2)));
				try
				{
					AntiTamperJIT.MethodData* ptr4 = (AntiTamperJIT.MethodData*)ptr3;
					uint* ptr5 = ptr3;
					uint num9 = num * (uint)Mutation.KeyI5;
					uint num10 = num9;
					for (uint num11 = 0U; num11 < num8; num11 += 1U)
					{
						*ptr5 = (*(ptr2++) ^ num9);
						num9 += (*(ptr5++) ^ num10);
						num10 ^= (num9 >> 5 | num9 << 27);
					}
					info->ILCodeSize = ptr4->ILCodeSize;
					if (AntiTamperJIT.ver4)
					{
						*(int*)(info + 1) = (int)ptr4->MaxStack;
						*(int*)(info + 1 + 4 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)ptr4->EHCount;
						*(int*)(info + 1 + (IntPtr)2 * 4 / (IntPtr)sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)ptr4->Options;
					}
					else
					{
						*(short*)(info + 1) = (short)((ushort)ptr4->MaxStack);
						*(short*)(info + 1 + 2 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (short)((ushort)ptr4->EHCount);
						*(int*)(info + 1 + 4 / sizeof(AntiTamperJIT.CORINFO_METHOD_INFO)) = (int)ptr4->Options;
					}
					byte* ptr6 = (byte*)(ptr4 + 1);
					info->ILCode = ptr6;
					ptr6 += info->ILCodeSize;
					if (ptr4->LocalVars != 0U)
					{
						AntiTamperJIT.ExtractLocalVars(info, ptr4->LocalVars, ptr6);
						ptr6 += ptr4->LocalVars;
					}
					AntiTamperJIT.CORINFO_EH_CLAUSE* clauses = (AntiTamperJIT.CORINFO_EH_CLAUSE*)ptr6;
					uint result;
					if (AntiTamperJIT.ver5)
					{
						AntiTamperJIT.CorJitInfoHook corJitInfoHook = AntiTamperJIT.CorJitInfoHook.Hook(comp, info->ftn, clauses);
						result = AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
						corJitInfoHook.Dispose();
					}
					else
					{
						AntiTamperJIT.CorMethodInfoHook corMethodInfoHook = AntiTamperJIT.CorMethodInfoHook.Hook(comp, info->ftn, clauses);
						result = AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
						corMethodInfoHook.Dispose();
					}
					return result;
				}
				finally
				{
					Marshal.FreeHGlobal((IntPtr)((void*)ptr3));
				}
			}
			return AntiTamperJIT.originalDelegate(self, comp, info, flags, nativeEntry, nativeSizeOfCode);
		}

		// Token: 0x04000037 RID: 55
		private unsafe static uint* ptr;

		// Token: 0x04000038 RID: 56
		private static uint len;

		// Token: 0x04000039 RID: 57
		private static IntPtr moduleHnd;

		// Token: 0x0400003A RID: 58
		private static AntiTamperJIT.compileMethod originalDelegate;

		// Token: 0x0400003B RID: 59
		private static bool ver4;

		// Token: 0x0400003C RID: 60
		private static bool ver5;

		// Token: 0x0400003D RID: 61
		private static AntiTamperJIT.compileMethod handler;

		// Token: 0x0400003E RID: 62
		private static bool hasLinkInfo;

		// Token: 0x02000014 RID: 20
		private struct CORINFO_EH_CLAUSE
		{
		}

		// Token: 0x02000015 RID: 21
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct CORINFO_METHOD_INFO
		{
			// Token: 0x0400003F RID: 63
			public IntPtr ftn;

			// Token: 0x04000040 RID: 64
			public IntPtr scope;

			// Token: 0x04000041 RID: 65
			public unsafe byte* ILCode;

			// Token: 0x04000042 RID: 66
			public uint ILCodeSize;
		}

		// Token: 0x02000016 RID: 22
		private struct CORINFO_SIG_INFO_x64
		{
			// Token: 0x04000043 RID: 67
			public uint callConv;

			// Token: 0x04000044 RID: 68
			private uint pad1;

			// Token: 0x04000045 RID: 69
			public IntPtr retTypeClass;

			// Token: 0x04000046 RID: 70
			public IntPtr retTypeSigClass;

			// Token: 0x04000047 RID: 71
			public byte retType;

			// Token: 0x04000048 RID: 72
			public byte flags;

			// Token: 0x04000049 RID: 73
			public ushort numArgs;

			// Token: 0x0400004A RID: 74
			private uint pad2;

			// Token: 0x0400004B RID: 75
			public AntiTamperJIT.CORINFO_SIG_INST_x64 sigInst;

			// Token: 0x0400004C RID: 76
			public IntPtr args;

			// Token: 0x0400004D RID: 77
			public IntPtr sig;

			// Token: 0x0400004E RID: 78
			public IntPtr scope;

			// Token: 0x0400004F RID: 79
			public uint token;

			// Token: 0x04000050 RID: 80
			private uint pad3;
		}

		// Token: 0x02000017 RID: 23
		private struct CORINFO_SIG_INFO_x86
		{
			// Token: 0x04000051 RID: 81
			public uint callConv;

			// Token: 0x04000052 RID: 82
			public IntPtr retTypeClass;

			// Token: 0x04000053 RID: 83
			public IntPtr retTypeSigClass;

			// Token: 0x04000054 RID: 84
			public byte retType;

			// Token: 0x04000055 RID: 85
			public byte flags;

			// Token: 0x04000056 RID: 86
			public ushort numArgs;

			// Token: 0x04000057 RID: 87
			public AntiTamperJIT.CORINFO_SIG_INST_x86 sigInst;

			// Token: 0x04000058 RID: 88
			public IntPtr args;

			// Token: 0x04000059 RID: 89
			public IntPtr sig;

			// Token: 0x0400005A RID: 90
			public IntPtr scope;

			// Token: 0x0400005B RID: 91
			public uint token;
		}

		// Token: 0x02000018 RID: 24
		private struct CORINFO_SIG_INST_x64
		{
		}

		// Token: 0x02000019 RID: 25
		private struct CORINFO_SIG_INST_x86
		{
		}

		// Token: 0x0200001A RID: 26
		private struct ICorClassInfo
		{
			// Token: 0x0400005C RID: 92
			public unsafe readonly IntPtr* vfptr;
		}

		// Token: 0x0200001B RID: 27
		private struct ICorDynamicInfo
		{
			// Token: 0x0600006D RID: 109 RVA: 0x00002315 File Offset: 0x00000515
			public unsafe static AntiTamperJIT.ICorStaticInfo* ICorStaticInfo(AntiTamperJIT.ICorDynamicInfo* ptr)
			{
				return (AntiTamperJIT.ICorStaticInfo*)(&ptr->vbptr) + ptr->vbptr[(AntiTamperJIT.hasLinkInfo ? 9 : 8) * 4] / sizeof(AntiTamperJIT.ICorStaticInfo);
			}

			// Token: 0x0400005D RID: 93
			public unsafe IntPtr* vfptr;

			// Token: 0x0400005E RID: 94
			public unsafe int* vbptr;
		}

		// Token: 0x0200001C RID: 28
		private struct ICorJitInfo
		{
			// Token: 0x0600006E RID: 110 RVA: 0x00005A54 File Offset: 0x00003C54
			public unsafe static AntiTamperJIT.ICorDynamicInfo* ICorDynamicInfo(AntiTamperJIT.ICorJitInfo* ptr)
			{
				AntiTamperJIT.hasLinkInfo = (ptr->vbptr[10] > 0 && ptr->vbptr[10] >> 16 == 0);
				return (AntiTamperJIT.ICorDynamicInfo*)(&ptr->vbptr) + ptr->vbptr[(AntiTamperJIT.hasLinkInfo ? 10 : 9) * 4] / sizeof(AntiTamperJIT.ICorDynamicInfo);
			}

			// Token: 0x0400005F RID: 95
			public unsafe IntPtr* vfptr;

			// Token: 0x04000060 RID: 96
			public unsafe int* vbptr;
		}

		// Token: 0x0200001D RID: 29
		private struct ICorMethodInfo
		{
			// Token: 0x04000061 RID: 97
			public unsafe IntPtr* vfptr;
		}

		// Token: 0x0200001E RID: 30
		private struct ICorModuleInfo
		{
			// Token: 0x04000062 RID: 98
			public unsafe IntPtr* vfptr;
		}

		// Token: 0x0200001F RID: 31
		private struct ICorStaticInfo
		{
			// Token: 0x0600006F RID: 111 RVA: 0x00002336 File Offset: 0x00000536
			public unsafe static AntiTamperJIT.ICorMethodInfo* ICorMethodInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorMethodInfo*)(&ptr->vbptr) + ptr->vbptr[1] / sizeof(AntiTamperJIT.ICorMethodInfo);
			}

			// Token: 0x06000070 RID: 112 RVA: 0x00002349 File Offset: 0x00000549
			public unsafe static AntiTamperJIT.ICorModuleInfo* ICorModuleInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorModuleInfo*)(&ptr->vbptr) + ptr->vbptr[2] / sizeof(AntiTamperJIT.ICorModuleInfo);
			}

			// Token: 0x06000071 RID: 113 RVA: 0x0000235F File Offset: 0x0000055F
			public unsafe static AntiTamperJIT.ICorClassInfo* ICorClassInfo(AntiTamperJIT.ICorStaticInfo* ptr)
			{
				return (AntiTamperJIT.ICorClassInfo*)(&ptr->vbptr) + ptr->vbptr[3] / sizeof(AntiTamperJIT.ICorClassInfo);
			}

			// Token: 0x04000063 RID: 99
			public unsafe IntPtr* vfptr;

			// Token: 0x04000064 RID: 100
			public unsafe int* vbptr;
		}

		// Token: 0x02000020 RID: 32
		private class CorMethodInfoHook
		{
			// Token: 0x06000072 RID: 114 RVA: 0x00002375 File Offset: 0x00000575
			private unsafe void hookEHInfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause)
			{
				if (ftn == this.ftn)
				{
					*clause = this.clauses[(ulong)EHnumber * (ulong)((long)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)) / (ulong)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)];
					return;
				}
				this.o_getEHinfo(self, ftn, EHnumber, clause);
			}

			// Token: 0x06000073 RID: 115 RVA: 0x000023B4 File Offset: 0x000005B4
			public unsafe void Dispose()
			{
				Marshal.FreeHGlobal((IntPtr)((void*)this.newVfTbl));
				this.info->vfptr = this.oldVfTbl;
			}

			// Token: 0x06000074 RID: 116 RVA: 0x00005AAC File Offset: 0x00003CAC
			public unsafe static AntiTamperJIT.CorMethodInfoHook Hook(AntiTamperJIT.ICorJitInfo* comp, IntPtr ftn, AntiTamperJIT.CORINFO_EH_CLAUSE* clauses)
			{
				AntiTamperJIT.ICorMethodInfo* ptr = AntiTamperJIT.ICorStaticInfo.ICorMethodInfo(AntiTamperJIT.ICorDynamicInfo.ICorStaticInfo(AntiTamperJIT.ICorJitInfo.ICorDynamicInfo(comp)));
				IntPtr* vfptr = ptr->vfptr;
				IntPtr* ptr2 = (IntPtr*)((void*)Marshal.AllocHGlobal(27 * IntPtr.Size));
				for (int i = 0; i < 27; i++)
				{
					ptr2[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = vfptr[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
				}
				if (AntiTamperJIT.CorMethodInfoHook.ehNum == -1)
				{
					for (int j = 0; j < 27; j++)
					{
						bool flag = true;
						byte* ptr3 = (byte*)((void*)vfptr[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]);
						while (*ptr3 != 233)
						{
							if ((IntPtr.Size == 8) ? (*ptr3 == 72 && ptr3[1] == 129 && ptr3[2] == 233) : (*ptr3 == 131 && ptr3[1] == 233))
							{
								flag = false;
								break;
							}
							ptr3++;
						}
						if (flag)
						{
							AntiTamperJIT.CorMethodInfoHook.ehNum = j;
							break;
						}
					}
				}
				AntiTamperJIT.CorMethodInfoHook corMethodInfoHook = new AntiTamperJIT.CorMethodInfoHook
				{
					ftn = ftn,
					info = ptr,
					clauses = clauses,
					newVfTbl = ptr2,
					oldVfTbl = vfptr
				};
				corMethodInfoHook.n_getEHinfo = new AntiTamperJIT.getEHinfo(corMethodInfoHook.hookEHInfo);
				corMethodInfoHook.o_getEHinfo = (AntiTamperJIT.getEHinfo)Marshal.GetDelegateForFunctionPointer(vfptr[(IntPtr)AntiTamperJIT.CorMethodInfoHook.ehNum * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getEHinfo));
				ptr2[(IntPtr)AntiTamperJIT.CorMethodInfoHook.ehNum * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = Marshal.GetFunctionPointerForDelegate(corMethodInfoHook.n_getEHinfo);
				ptr->vfptr = ptr2;
				return corMethodInfoHook;
			}

			// Token: 0x04000065 RID: 101
			private static int ehNum = -1;

			// Token: 0x04000066 RID: 102
			public unsafe AntiTamperJIT.CORINFO_EH_CLAUSE* clauses;

			// Token: 0x04000067 RID: 103
			public IntPtr ftn;

			// Token: 0x04000068 RID: 104
			public unsafe AntiTamperJIT.ICorMethodInfo* info;

			// Token: 0x04000069 RID: 105
			public AntiTamperJIT.getEHinfo n_getEHinfo;

			// Token: 0x0400006A RID: 106
			public unsafe IntPtr* newVfTbl;

			// Token: 0x0400006B RID: 107
			public AntiTamperJIT.getEHinfo o_getEHinfo;

			// Token: 0x0400006C RID: 108
			public unsafe IntPtr* oldVfTbl;
		}

		// Token: 0x02000021 RID: 33
		private class CorJitInfoHook
		{
			// Token: 0x06000077 RID: 119 RVA: 0x000023DF File Offset: 0x000005DF
			private unsafe void hookEHInfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause)
			{
				if (ftn == this.ftn)
				{
					*clause = this.clauses[(ulong)EHnumber * (ulong)((long)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)) / (ulong)sizeof(AntiTamperJIT.CORINFO_EH_CLAUSE)];
					return;
				}
				this.o_getEHinfo(self, ftn, EHnumber, clause);
			}

			// Token: 0x06000078 RID: 120 RVA: 0x0000241E File Offset: 0x0000061E
			public unsafe void Dispose()
			{
				Marshal.FreeHGlobal((IntPtr)((void*)this.newVfTbl));
				this.info->vfptr = this.oldVfTbl;
			}

			// Token: 0x06000079 RID: 121 RVA: 0x00005C3C File Offset: 0x00003E3C
			public unsafe static AntiTamperJIT.CorJitInfoHook Hook(AntiTamperJIT.ICorJitInfo* comp, IntPtr ftn, AntiTamperJIT.CORINFO_EH_CLAUSE* clauses)
			{
				IntPtr* vfptr = comp->vfptr;
				IntPtr* ptr = (IntPtr*)((void*)Marshal.AllocHGlobal(158 * IntPtr.Size));
				for (int i = 0; i < 158; i++)
				{
					ptr[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = vfptr[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)];
				}
				AntiTamperJIT.CorJitInfoHook corJitInfoHook = new AntiTamperJIT.CorJitInfoHook
				{
					ftn = ftn,
					info = comp,
					clauses = clauses,
					newVfTbl = ptr,
					oldVfTbl = vfptr
				};
				corJitInfoHook.n_getEHinfo = new AntiTamperJIT.getEHinfo(corJitInfoHook.hookEHInfo);
				corJitInfoHook.o_getEHinfo = (AntiTamperJIT.getEHinfo)Marshal.GetDelegateForFunctionPointer(vfptr[(IntPtr)8 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)], typeof(AntiTamperJIT.getEHinfo));
				ptr[(IntPtr)8 * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)] = Marshal.GetFunctionPointerForDelegate(corJitInfoHook.n_getEHinfo);
				comp->vfptr = ptr;
				return corJitInfoHook;
			}

			// Token: 0x0400006D RID: 109
			public unsafe AntiTamperJIT.CORINFO_EH_CLAUSE* clauses;

			// Token: 0x0400006E RID: 110
			public IntPtr ftn;

			// Token: 0x0400006F RID: 111
			public unsafe AntiTamperJIT.ICorJitInfo* info;

			// Token: 0x04000070 RID: 112
			public AntiTamperJIT.getEHinfo n_getEHinfo;

			// Token: 0x04000071 RID: 113
			public unsafe IntPtr* newVfTbl;

			// Token: 0x04000072 RID: 114
			public AntiTamperJIT.getEHinfo o_getEHinfo;

			// Token: 0x04000073 RID: 115
			public unsafe IntPtr* oldVfTbl;
		}

		// Token: 0x02000022 RID: 34
		private struct MethodData
		{
			// Token: 0x04000074 RID: 116
			public readonly uint ILCodeSize;

			// Token: 0x04000075 RID: 117
			public readonly uint MaxStack;

			// Token: 0x04000076 RID: 118
			public readonly uint EHCount;

			// Token: 0x04000077 RID: 119
			public readonly uint LocalVars;

			// Token: 0x04000078 RID: 120
			public readonly uint Options;

			// Token: 0x04000079 RID: 121
			public readonly uint MulSeed;
		}

		// Token: 0x02000023 RID: 35
		// (Invoke) Token: 0x0600007C RID: 124
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private unsafe delegate uint compileMethod(IntPtr self, AntiTamperJIT.ICorJitInfo* comp, AntiTamperJIT.CORINFO_METHOD_INFO* info, uint flags, byte** nativeEntry, uint* nativeSizeOfCode);

		// Token: 0x02000024 RID: 36
		// (Invoke) Token: 0x06000080 RID: 128
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private unsafe delegate void getEHinfo(IntPtr self, IntPtr ftn, uint EHnumber, AntiTamperJIT.CORINFO_EH_CLAUSE* clause);

		// Token: 0x02000025 RID: 37
		// (Invoke) Token: 0x06000084 RID: 132
		private unsafe delegate IntPtr* getJit();

		// Token: 0x02000026 RID: 38
		// (Invoke) Token: 0x06000088 RID: 136
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate uint getMethodDefFromMethod(IntPtr self, IntPtr ftn);
	}
}
