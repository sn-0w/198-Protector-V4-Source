using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Confuser.Runtime
{
	// Token: 0x02000007 RID: 7
	internal static class AntiDebugAntinet
	{
		// Token: 0x06000016 RID: 22 RVA: 0x000020ED File Offset: 0x000002ED
		private static void Initialize()
		{
			if (!AntiDebugAntinet.InitializeAntiDebugger())
			{
				Environment.FailFast(null);
			}
			AntiDebugAntinet.InitializeAntiProfiler();
			if (AntiDebugAntinet.IsProfilerAttached)
			{
				Environment.FailFast(null);
				AntiDebugAntinet.PreventActiveProfilerFromReceivingProfilingMessages();
			}
		}

		// Token: 0x06000017 RID: 23
		[DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern uint GetCurrentProcessId();

		// Token: 0x06000018 RID: 24
		[DllImport("kernel32")]
		private static extern bool SetEvent(IntPtr hEvent);

		// Token: 0x06000019 RID: 25 RVA: 0x00003348 File Offset: 0x00001548
		private unsafe static bool InitializeAntiDebugger()
		{
			AntiDebugAntinet.Info info = AntiDebugAntinet.GetInfo();
			IntPtr intPtr = AntiDebugAntinet.FindDebuggerRCThreadAddress(info);
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			byte* ptr = (byte*)((void*)(*(IntPtr*)((byte*)((void*)intPtr) + info.DebuggerRCThread_pDebuggerIPCControlBlock)));
			if (Environment.Version.Major == 2)
			{
				ptr = (byte*)((void*)(*(IntPtr*)ptr));
			}
			*(int*)ptr = 0;
			((byte*)((void*)intPtr))[info.DebuggerRCThread_shouldKeepLooping] = 0;
			IntPtr @event = *(IntPtr*)((byte*)((void*)intPtr) + info.DebuggerRCThread_hEvent1);
			AntiDebugAntinet.SetEvent(@event);
			return true;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000033C4 File Offset: 0x000015C4
		private static AntiDebugAntinet.Info GetInfo()
		{
			int major = Environment.Version.Major;
			if (major != 2)
			{
				if (major != 4)
				{
				}
				if (Environment.Version.Revision <= 17020)
				{
					if (IntPtr.Size != 4)
					{
						return AntiDebugAntinet.Infos.info_CLR40_x64;
					}
					return AntiDebugAntinet.Infos.info_CLR40_x86_1;
				}
				else
				{
					if (IntPtr.Size != 4)
					{
						return AntiDebugAntinet.Infos.info_CLR40_x64;
					}
					return AntiDebugAntinet.Infos.info_CLR40_x86_2;
				}
			}
			else
			{
				if (IntPtr.Size != 4)
				{
					return AntiDebugAntinet.Infos.info_CLR20_x64;
				}
				return AntiDebugAntinet.Infos.info_CLR20_x86;
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003434 File Offset: 0x00001634
		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		[SecurityCritical]
		private unsafe static IntPtr FindDebuggerRCThreadAddress(AntiDebugAntinet.Info info)
		{
			uint currentProcessId = AntiDebugAntinet.GetCurrentProcessId();
			try
			{
				AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
				if (clr == null)
				{
					return IntPtr.Zero;
				}
				IntPtr value;
				uint num;
				if (!clr.FindSection(".data", out value, out num))
				{
					return IntPtr.Zero;
				}
				byte* ptr = (byte*)((void*)value);
				byte* ptr2 = (byte*)((void*)value) + num;
				while (ptr + IntPtr.Size == ptr2)
				{
					IntPtr intPtr = *(IntPtr*)ptr;
					if (!(intPtr == IntPtr.Zero))
					{
						try
						{
							if (AntiDebugAntinet.PEInfo.IsAlignedPointer(intPtr))
							{
								uint num2 = *(uint*)((byte*)((void*)intPtr) + info.Debugger_pid);
								if (currentProcessId == num2)
								{
									IntPtr intPtr2 = *(IntPtr*)((byte*)((void*)intPtr) + info.Debugger_pDebuggerRCThread);
									if (AntiDebugAntinet.PEInfo.IsAlignedPointer(intPtr2))
									{
										IntPtr value2 = *(IntPtr*)((byte*)((void*)intPtr2) + info.DebuggerRCThread_pDebugger);
										if (!(intPtr != value2))
										{
											return intPtr2;
										}
									}
								}
							}
						}
						catch
						{
						}
					}
					ptr += IntPtr.Size;
				}
			}
			catch
			{
			}
			return IntPtr.Zero;
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001C RID: 28 RVA: 0x00003550 File Offset: 0x00001750
		private static bool IsProfilerAttached
		{
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			get
			{
				try
				{
					if (AntiDebugAntinet.profilerDetector == null)
					{
						return false;
					}
					return AntiDebugAntinet.profilerDetector.IsProfilerAttached();
				}
				catch
				{
				}
				return false;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000358C File Offset: 0x0000178C
		private static bool WasProfilerAttached
		{
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			get
			{
				try
				{
					if (AntiDebugAntinet.profilerDetector == null)
					{
						return false;
					}
					return AntiDebugAntinet.profilerDetector.WasProfilerAttached();
				}
				catch
				{
				}
				return false;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002114 File Offset: 0x00000314
		private static bool InitializeAntiProfiler()
		{
			AntiDebugAntinet.profilerDetector = AntiDebugAntinet.CreateProfilerDetector();
			return AntiDebugAntinet.profilerDetector.Init();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000212A File Offset: 0x0000032A
		private static AntiDebugAntinet.ProfilerDetector CreateProfilerDetector()
		{
			if (Environment.Version.Major == 2)
			{
				return new AntiDebugAntinet.ProfilerDetectorCLR20();
			}
			return new AntiDebugAntinet.ProfilerDetectorCLR40();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002144 File Offset: 0x00000344
		private static void PreventActiveProfilerFromReceivingProfilingMessages()
		{
			if (AntiDebugAntinet.profilerDetector == null)
			{
				return;
			}
			AntiDebugAntinet.profilerDetector.PreventActiveProfilerFromReceivingProfilingMessages();
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000035C8 File Offset: 0x000017C8
		private static IntPtr GetMax(Dictionary<IntPtr, int> addresses, int minCount)
		{
			IntPtr intPtr = IntPtr.Zero;
			int num = 0;
			foreach (KeyValuePair<IntPtr, int> keyValuePair in addresses)
			{
				if (intPtr == IntPtr.Zero || num < keyValuePair.Value)
				{
					intPtr = keyValuePair.Key;
					num = keyValuePair.Value;
				}
			}
			if (num < minCount)
			{
				return IntPtr.Zero;
			}
			return intPtr;
		}

		// Token: 0x04000011 RID: 17
		private static AntiDebugAntinet.ProfilerDetector profilerDetector;

		// Token: 0x02000008 RID: 8
		private class Info
		{
			// Token: 0x04000012 RID: 18
			public int DebuggerRCThread_hEvent1;

			// Token: 0x04000013 RID: 19
			public int DebuggerRCThread_pDebugger;

			// Token: 0x04000014 RID: 20
			public int DebuggerRCThread_pDebuggerIPCControlBlock;

			// Token: 0x04000015 RID: 21
			public int DebuggerRCThread_shouldKeepLooping;

			// Token: 0x04000016 RID: 22
			public int Debugger_pDebuggerRCThread;

			// Token: 0x04000017 RID: 23
			public int Debugger_pid;
		}

		// Token: 0x02000009 RID: 9
		private static class Infos
		{
			// Token: 0x04000018 RID: 24
			public static readonly AntiDebugAntinet.Info info_CLR20_x86 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 4,
				Debugger_pid = 8,
				DebuggerRCThread_pDebugger = 48,
				DebuggerRCThread_pDebuggerIPCControlBlock = 52,
				DebuggerRCThread_shouldKeepLooping = 60,
				DebuggerRCThread_hEvent1 = 64
			};

			// Token: 0x04000019 RID: 25
			public static readonly AntiDebugAntinet.Info info_CLR20_x64 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 16,
				DebuggerRCThread_pDebugger = 88,
				DebuggerRCThread_pDebuggerIPCControlBlock = 96,
				DebuggerRCThread_shouldKeepLooping = 112,
				DebuggerRCThread_hEvent1 = 120
			};

			// Token: 0x0400001A RID: 26
			public static readonly AntiDebugAntinet.Info info_CLR40_x86_1 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 12,
				DebuggerRCThread_pDebugger = 52,
				DebuggerRCThread_pDebuggerIPCControlBlock = 56,
				DebuggerRCThread_shouldKeepLooping = 64,
				DebuggerRCThread_hEvent1 = 68
			};

			// Token: 0x0400001B RID: 27
			public static readonly AntiDebugAntinet.Info info_CLR40_x86_2 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 8,
				Debugger_pid = 12,
				DebuggerRCThread_pDebugger = 48,
				DebuggerRCThread_pDebuggerIPCControlBlock = 52,
				DebuggerRCThread_shouldKeepLooping = 60,
				DebuggerRCThread_hEvent1 = 64
			};

			// Token: 0x0400001C RID: 28
			public static readonly AntiDebugAntinet.Info info_CLR40_x64 = new AntiDebugAntinet.Info
			{
				Debugger_pDebuggerRCThread = 16,
				Debugger_pid = 24,
				DebuggerRCThread_pDebugger = 88,
				DebuggerRCThread_pDebuggerIPCControlBlock = 96,
				DebuggerRCThread_shouldKeepLooping = 112,
				DebuggerRCThread_hEvent1 = 120
			};
		}

		// Token: 0x0200000A RID: 10
		private abstract class ProfilerDetector
		{
			// Token: 0x06000024 RID: 36
			public abstract bool IsProfilerAttached();

			// Token: 0x06000025 RID: 37
			public abstract bool WasProfilerAttached();

			// Token: 0x06000026 RID: 38
			public abstract bool Init();

			// Token: 0x06000027 RID: 39
			public abstract void PreventActiveProfilerFromReceivingProfilingMessages();
		}

		// Token: 0x0200000B RID: 11
		private class ProfilerDetectorCLR20 : AntiDebugAntinet.ProfilerDetector
		{
			// Token: 0x06000029 RID: 41 RVA: 0x00002158 File Offset: 0x00000358
			public unsafe override bool IsProfilerAttached()
			{
				return !(this.profilerStatusFlag == IntPtr.Zero) && (*(uint*)((void*)this.profilerStatusFlag) & 6U) > 0U;
			}

			// Token: 0x0600002A RID: 42 RVA: 0x0000217F File Offset: 0x0000037F
			public override bool WasProfilerAttached()
			{
				return this.wasAttached;
			}

			// Token: 0x0600002B RID: 43 RVA: 0x00003778 File Offset: 0x00001978
			public override bool Init()
			{
				bool result = this.FindProfilerStatus();
				this.wasAttached = this.IsProfilerAttached();
				return result;
			}

			// Token: 0x0600002C RID: 44 RVA: 0x0000379C File Offset: 0x0000199C
			private unsafe bool FindProfilerStatus()
			{
				Dictionary<IntPtr, int> dictionary = new Dictionary<IntPtr, int>();
				try
				{
					AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
					if (clr == null)
					{
						return false;
					}
					IntPtr value;
					uint num;
					if (!clr.FindSection(".text", out value, out num))
					{
						return false;
					}
					byte* ptr = (byte*)((void*)value);
					byte* ptr2 = (byte*)((void*)value) + num;
					while (ptr < ptr2)
					{
						if (*ptr == 246 && ptr[1] == 5 && ptr[6] == 6)
						{
							IntPtr intPtr;
							if (IntPtr.Size == 4)
							{
								intPtr = new IntPtr(*(uint*)(ptr + 2));
							}
							else
							{
								intPtr = new IntPtr((void*)(ptr + 7 + *(int*)(ptr + 2)));
							}
							if (AntiDebugAntinet.PEInfo.IsAligned(intPtr, 4U) && clr.IsValidImageAddress(intPtr, 4U))
							{
								try
								{
									*(int*)((void*)intPtr) = (int)(*(uint*)((void*)intPtr));
								}
								catch
								{
									goto IL_DD;
								}
								int num2 = 0;
								dictionary.TryGetValue(intPtr, out num2);
								num2++;
								dictionary[intPtr] = num2;
								if (num2 >= 50)
								{
									break;
								}
							}
						}
						IL_DD:
						ptr++;
					}
				}
				catch
				{
				}
				IntPtr max = AntiDebugAntinet.GetMax(dictionary, 5);
				if (max == IntPtr.Zero)
				{
					return false;
				}
				this.profilerStatusFlag = max;
				return true;
			}

			// Token: 0x0600002D RID: 45 RVA: 0x00002187 File Offset: 0x00000387
			public unsafe override void PreventActiveProfilerFromReceivingProfilingMessages()
			{
				if (this.profilerStatusFlag == IntPtr.Zero)
				{
					return;
				}
				*(uint*)((void*)this.profilerStatusFlag) &= 4294967289U;
			}

			// Token: 0x0400001D RID: 29
			private IntPtr profilerStatusFlag;

			// Token: 0x0400001E RID: 30
			private bool wasAttached;
		}

		// Token: 0x0200000C RID: 12
		private class ProfilerDetectorCLR40 : AntiDebugAntinet.ProfilerDetector
		{
			// Token: 0x0600002F RID: 47
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern uint GetCurrentProcessId();

			// Token: 0x06000030 RID: 48
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern void Sleep(uint dwMilliseconds);

			// Token: 0x06000031 RID: 49
			[DllImport("kernel32", SetLastError = true)]
			private static extern SafeFileHandle CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);

			// Token: 0x06000032 RID: 50
			[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

			// Token: 0x06000033 RID: 51
			[DllImport("kernel32")]
			private static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);

			// Token: 0x06000034 RID: 52 RVA: 0x000021B5 File Offset: 0x000003B5
			public unsafe override bool IsProfilerAttached()
			{
				return !(this.profilerControlBlock == IntPtr.Zero) && *(uint*)((byte*)((byte*)((void*)this.profilerControlBlock) + IntPtr.Size) + 4) > 0U;
			}

			// Token: 0x06000035 RID: 53 RVA: 0x000021E2 File Offset: 0x000003E2
			public override bool WasProfilerAttached()
			{
				return this.wasAttached;
			}

			// Token: 0x06000036 RID: 54 RVA: 0x000038D8 File Offset: 0x00001AD8
			public override bool Init()
			{
				bool flag = this.FindProfilerControlBlock();
				flag &= (this.TakeOwnershipOfNamedPipe() || this.CreateNamedPipe());
				flag &= this.PatchAttacherThreadProc();
				this.wasAttached = this.IsProfilerAttached();
				return flag;
			}

			// Token: 0x06000037 RID: 55 RVA: 0x00003918 File Offset: 0x00001B18
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool TakeOwnershipOfNamedPipe()
			{
				try
				{
					if (this.CreateNamedPipe())
					{
						return true;
					}
					IntPtr intPtr = AntiDebugAntinet.ProfilerDetectorCLR40.FindThreadingModeAddress();
					IntPtr intPtr2 = AntiDebugAntinet.ProfilerDetectorCLR40.FindTimeOutOptionAddress();
					if (intPtr2 == IntPtr.Zero)
					{
						return false;
					}
					if (intPtr != IntPtr.Zero && *(uint*)((void*)intPtr) == 2U)
					{
						*(int*)((void*)intPtr) = 1;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.FixTimeOutOption(intPtr2);
					using (SafeFileHandle safeFileHandle = this.CreatePipeFileHandleWait())
					{
						if (safeFileHandle == null)
						{
							return false;
						}
						if (safeFileHandle.IsInvalid)
						{
							return false;
						}
					}
					return this.CreateNamedPipeWait();
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06000038 RID: 56 RVA: 0x000039C4 File Offset: 0x00001BC4
			private bool CreateNamedPipeWait()
			{
				for (int i = 100; i > 0; i -= 5)
				{
					if (this.CreateNamedPipe())
					{
						return true;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.Sleep(5U);
				}
				return this.CreateNamedPipe();
			}

			// Token: 0x06000039 RID: 57 RVA: 0x000039F4 File Offset: 0x00001BF4
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static void FixTimeOutOption(IntPtr timeOutOptionAddr)
			{
				if (timeOutOptionAddr == IntPtr.Zero)
				{
					return;
				}
				uint flNewProtect;
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(timeOutOptionAddr, (int)(AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue + 4U), 64U, out flNewProtect);
				try
				{
					*(int*)((byte*)((void*)timeOutOptionAddr) + AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue) = 0;
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(timeOutOptionAddr, (int)(AntiDebugAntinet.ProfilerDetectorCLR40.ConfigDWORDInfo_defValue + 4U), flNewProtect, out flNewProtect);
				}
				char* ptr = *(IntPtr*)((void*)timeOutOptionAddr);
				IntPtr lpAddress = new IntPtr((void*)ptr);
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(lpAddress, "ProfAPIMaxWaitForTriggerMs".Length * 2, 64U, out flNewProtect);
				try
				{
					Random random = new Random();
					for (int i = 0; i < "ProfAPIMaxWaitForTriggerMs".Length; i++)
					{
						ptr[i] = (char)random.Next(1, 65535);
					}
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(lpAddress, IntPtr.Size, flNewProtect, out flNewProtect);
				}
			}

			// Token: 0x0600003A RID: 58 RVA: 0x00003AD0 File Offset: 0x00001CD0
			private SafeFileHandle CreatePipeFileHandleWait()
			{
				for (int i = 100; i > 0; i -= 5)
				{
					if (this.CreateNamedPipe())
					{
						return null;
					}
					SafeFileHandle safeFileHandle = AntiDebugAntinet.ProfilerDetectorCLR40.CreatePipeFileHandle();
					if (!safeFileHandle.IsInvalid)
					{
						return safeFileHandle;
					}
					AntiDebugAntinet.ProfilerDetectorCLR40.Sleep(5U);
				}
				return AntiDebugAntinet.ProfilerDetectorCLR40.CreatePipeFileHandle();
			}

			// Token: 0x0600003B RID: 59 RVA: 0x000021EA File Offset: 0x000003EA
			private static SafeFileHandle CreatePipeFileHandle()
			{
				return AntiDebugAntinet.ProfilerDetectorCLR40.CreateFile(AntiDebugAntinet.ProfilerDetectorCLR40.GetPipeName(), 3221225472U, 0U, IntPtr.Zero, 3U, 1073741824U, IntPtr.Zero);
			}

			// Token: 0x0600003C RID: 60 RVA: 0x00003B10 File Offset: 0x00001D10
			private static string GetPipeName()
			{
				return string.Format("\\\\.\\pipe\\CPFATP_{0}_v{1}.{2}.{3}", new object[]
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.GetCurrentProcessId(),
					Environment.Version.Major,
					Environment.Version.Minor,
					Environment.Version.Build
				});
			}

			// Token: 0x0600003D RID: 61 RVA: 0x00003B70 File Offset: 0x00001D70
			private bool CreateNamedPipe()
			{
				if (this.profilerPipe != null && !this.profilerPipe.IsInvalid)
				{
					return true;
				}
				this.profilerPipe = AntiDebugAntinet.ProfilerDetectorCLR40.CreateNamedPipe(AntiDebugAntinet.ProfilerDetectorCLR40.GetPipeName(), 1073741827U, 6U, 1U, 36U, 824U, 1000U, IntPtr.Zero);
				return !this.profilerPipe.IsInvalid;
			}

			// Token: 0x0600003E RID: 62 RVA: 0x00003BCC File Offset: 0x00001DCC
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static IntPtr FindThreadingModeAddress()
			{
				try
				{
					AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
					if (clr == null)
					{
						return IntPtr.Zero;
					}
					IntPtr value;
					uint num;
					if (!clr.FindSection(".text", out value, out num))
					{
						return IntPtr.Zero;
					}
					byte* ptr = (byte*)((void*)value);
					byte* ptr2 = (byte*)((void*)value) + num;
					while (ptr < ptr2)
					{
						try
						{
							byte* ptr3 = ptr;
							if (*ptr3 == 131 && ptr3[1] == 61 && ptr3[6] == 2)
							{
								IntPtr intPtr;
								if (IntPtr.Size == 4)
								{
									intPtr = new IntPtr(*(uint*)(ptr3 + 2));
								}
								else
								{
									intPtr = new IntPtr((void*)(ptr3 + 7 + *(int*)(ptr3 + 2)));
								}
								if (AntiDebugAntinet.PEInfo.IsAligned(intPtr, 4U))
								{
									if (clr.IsValidImageAddress(intPtr))
									{
										ptr3 += 7;
										if (*(uint*)((void*)intPtr) >= 1U && *(uint*)((void*)intPtr) <= 2U)
										{
											*(int*)((void*)intPtr) = (int)(*(uint*)((void*)intPtr));
											if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref ptr3))
											{
												AntiDebugAntinet.ProfilerDetectorCLR40.SkipRex(ref ptr3);
												if (*ptr3 == 131 && ptr3[2] == 0)
												{
													if (ptr3[1] - 232 > 7)
													{
														goto IL_187;
													}
													ptr3 += 3;
												}
												else
												{
													if (*ptr3 != 133)
													{
														goto IL_187;
													}
													int num2 = ptr3[1] >> 3 & 7;
													int num3 = (int)(ptr3[1] & 7);
													if (num2 != num3)
													{
														goto IL_187;
													}
													ptr3 += 2;
												}
												if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref ptr3))
												{
													if (AntiDebugAntinet.ProfilerDetectorCLR40.SkipDecReg(ref ptr3))
													{
														if (AntiDebugAntinet.ProfilerDetectorCLR40.NextJz(ref ptr3))
														{
															if (AntiDebugAntinet.ProfilerDetectorCLR40.SkipDecReg(ref ptr3))
															{
																return intPtr;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						catch
						{
						}
						IL_187:
						ptr++;
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x0600003F RID: 63 RVA: 0x00003DB0 File Offset: 0x00001FB0
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static IntPtr FindTimeOutOptionAddress()
			{
				try
				{
					AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
					if (clr == null)
					{
						return IntPtr.Zero;
					}
					IntPtr value;
					uint num;
					if (!clr.FindSection(".rdata", out value, out num) && !clr.FindSection(".text", out value, out num))
					{
						return IntPtr.Zero;
					}
					byte* ptr = (byte*)((void*)value);
					byte* ptr2 = (byte*)((void*)value) + num;
					while (ptr < ptr2)
					{
						try
						{
							char* ptr3 = *(IntPtr*)ptr;
							if (AntiDebugAntinet.PEInfo.IsAligned(new IntPtr((void*)ptr3), 2U))
							{
								if (clr.IsValidImageAddress((void*)ptr3))
								{
									if (AntiDebugAntinet.ProfilerDetectorCLR40.Equals(ptr3, "ProfAPIMaxWaitForTriggerMs"))
									{
										return new IntPtr((void*)ptr);
									}
								}
							}
						}
						catch
						{
						}
						ptr++;
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x06000040 RID: 64 RVA: 0x00003E84 File Offset: 0x00002084
			private unsafe static bool Equals(char* s1, string s2)
			{
				for (int i = 0; i < s2.Length; i++)
				{
					if (char.ToUpperInvariant(s1[i]) != char.ToUpperInvariant(s2[i]))
					{
						return false;
					}
				}
				return s1[s2.Length] == '\0';
			}

			// Token: 0x06000041 RID: 65 RVA: 0x0000220C File Offset: 0x0000040C
			private unsafe static void SkipRex(ref byte* p)
			{
				if (IntPtr.Size != 8)
				{
					return;
				}
				if (*p >= 72 && *p <= 79)
				{
					p++;
				}
			}

			// Token: 0x06000042 RID: 66 RVA: 0x00003ED0 File Offset: 0x000020D0
			private unsafe static bool SkipDecReg(ref byte* p)
			{
				AntiDebugAntinet.ProfilerDetectorCLR40.SkipRex(ref p);
				if (IntPtr.Size == 4 && *p >= 72 && *p <= 79)
				{
					p++;
				}
				else
				{
					if (*p != 255 || *(p + 1) < 200 || *(p + 1) > 207)
					{
						return false;
					}
					p += 2;
				}
				return true;
			}

			// Token: 0x06000043 RID: 67 RVA: 0x0000222B File Offset: 0x0000042B
			private unsafe static bool NextJz(ref byte* p)
			{
				if (*p == 116)
				{
					p += 2;
					return true;
				}
				if (*p == 15 && *(p + 1) == 132)
				{
					p += 6;
					return true;
				}
				return false;
			}

			// Token: 0x06000044 RID: 68 RVA: 0x00003F30 File Offset: 0x00002130
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool PatchAttacherThreadProc()
			{
				IntPtr intPtr = this.FindAttacherThreadProc();
				if (intPtr == IntPtr.Zero)
				{
					return false;
				}
				byte* ptr = (byte*)((void*)intPtr);
				uint flNewProtect;
				AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(new IntPtr((void*)ptr), 5, 64U, out flNewProtect);
				try
				{
					if (IntPtr.Size == 4)
					{
						*ptr = 51;
						ptr[1] = 192;
						ptr[2] = 194;
						ptr[3] = 4;
						ptr[4] = 0;
					}
					else
					{
						*ptr = 51;
						ptr[1] = 192;
						ptr[2] = 195;
					}
				}
				finally
				{
					AntiDebugAntinet.ProfilerDetectorCLR40.VirtualProtect(new IntPtr((void*)ptr), 5, flNewProtect, out flNewProtect);
				}
				return true;
			}

			// Token: 0x06000045 RID: 69 RVA: 0x00003FD0 File Offset: 0x000021D0
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe IntPtr FindAttacherThreadProc()
			{
				try
				{
					AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
					if (clr == null)
					{
						return IntPtr.Zero;
					}
					IntPtr value;
					uint num;
					if (!clr.FindSection(".text", out value, out num))
					{
						return IntPtr.Zero;
					}
					byte* ptr = (byte*)((void*)value);
					byte* codeStart = ptr;
					byte* ptr2 = (byte*)((void*)value) + num;
					if (IntPtr.Size == 4)
					{
						while (ptr < ptr2)
						{
							byte b = *ptr;
							if (b >= 80 && b <= 87 && ptr[1] == b && ptr[2] == b && ptr[8] == b && ptr[9] == b && ptr[3] == 104 && ptr[10] == 255 && ptr[11] == 21)
							{
								IntPtr intPtr = new IntPtr(*(uint*)(ptr + 4));
								if (AntiDebugAntinet.ProfilerDetectorCLR40.CheckThreadProc(codeStart, ptr2, intPtr))
								{
									return intPtr;
								}
							}
							ptr++;
						}
					}
					else
					{
						while (ptr < ptr2)
						{
							if ((*ptr == 69 || ptr[1] == 51 || ptr[2] == 201) && (ptr[3] == 76 || ptr[4] == 141 || ptr[5] == 5) && (ptr[10] == 51 || ptr[11] == 210) && (ptr[12] == 51 || ptr[13] == 201) && (ptr[14] == 255 || ptr[15] == 21))
							{
								IntPtr intPtr2 = new IntPtr((void*)(ptr + 10 + *(int*)(ptr + 6)));
								if (AntiDebugAntinet.ProfilerDetectorCLR40.CheckThreadProc(codeStart, ptr2, intPtr2))
								{
									return intPtr2;
								}
							}
							ptr++;
						}
					}
				}
				catch
				{
				}
				return IntPtr.Zero;
			}

			// Token: 0x06000046 RID: 70 RVA: 0x0000417C File Offset: 0x0000237C
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe static bool CheckThreadProc(byte* codeStart, byte* codeEnd, IntPtr threadProc)
			{
				try
				{
					byte* ptr = (byte*)((void*)threadProc);
					if (ptr < codeStart || ptr >= codeEnd)
					{
						return false;
					}
					for (int i = 0; i < 32; i++)
					{
						if (*(uint*)(ptr + i) == 16384U)
						{
							return true;
						}
					}
				}
				catch
				{
				}
				return false;
			}

			// Token: 0x06000047 RID: 71 RVA: 0x000041D0 File Offset: 0x000023D0
			[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
			[SecurityCritical]
			private unsafe bool FindProfilerControlBlock()
			{
				Dictionary<IntPtr, int> dictionary = new Dictionary<IntPtr, int>();
				try
				{
					AntiDebugAntinet.PEInfo clr = AntiDebugAntinet.PEInfo.GetCLR();
					if (clr == null)
					{
						return false;
					}
					IntPtr value;
					uint num;
					if (!clr.FindSection(".text", out value, out num))
					{
						return false;
					}
					byte* ptr = (byte*)((void*)value);
					byte* ptr2 = (byte*)((void*)value) + num;
					while (ptr < ptr2)
					{
						IntPtr intPtr;
						if (*ptr == 161 && ptr[5] == 131 && ptr[6] == 248 && ptr[7] == 4)
						{
							if (IntPtr.Size == 4)
							{
								intPtr = new IntPtr(*(uint*)(ptr + 1));
								goto IL_14B;
							}
							intPtr = new IntPtr((void*)(ptr + 5 + *(int*)(ptr + 1)));
							goto IL_14B;
						}
						else if (*ptr == 139 && ptr[1] == 5 && ptr[6] == 131 && ptr[7] == 248 && ptr[8] == 4)
						{
							if (IntPtr.Size == 4)
							{
								intPtr = new IntPtr(*(uint*)(ptr + 2));
								goto IL_14B;
							}
							intPtr = new IntPtr((void*)(ptr + 6 + *(int*)(ptr + 2)));
							goto IL_14B;
						}
						else if (*ptr == 131 && ptr[1] == 61 && ptr[6] == 4)
						{
							if (IntPtr.Size == 4)
							{
								intPtr = new IntPtr(*(uint*)(ptr + 2));
								goto IL_14B;
							}
							intPtr = new IntPtr((void*)(ptr + 7 + *(int*)(ptr + 2)));
							goto IL_14B;
						}
						IL_1A7:
						ptr++;
						continue;
						IL_14B:
						if (!AntiDebugAntinet.PEInfo.IsAligned(intPtr, 4U) || !clr.IsValidImageAddress(intPtr, 4U))
						{
							goto IL_1A7;
						}
						try
						{
							if (*(uint*)((void*)intPtr) > 4U)
							{
								goto IL_1A7;
							}
							*(int*)((void*)intPtr) = (int)(*(uint*)((void*)intPtr));
						}
						catch
						{
							goto IL_1A7;
						}
						int num2 = 0;
						dictionary.TryGetValue(intPtr, out num2);
						num2++;
						dictionary[intPtr] = num2;
						if (num2 < 50)
						{
							goto IL_1A7;
						}
						break;
					}
				}
				catch
				{
				}
				IntPtr max = AntiDebugAntinet.GetMax(dictionary, 5);
				if (max == IntPtr.Zero)
				{
					return false;
				}
				this.profilerControlBlock = new IntPtr((void*)((byte*)((void*)max) - (IntPtr.Size + 4)));
				return true;
			}

			// Token: 0x06000048 RID: 72 RVA: 0x00002258 File Offset: 0x00000458
			public unsafe override void PreventActiveProfilerFromReceivingProfilingMessages()
			{
				if (this.profilerControlBlock == IntPtr.Zero)
				{
					return;
				}
				*(int*)((byte*)((byte*)((void*)this.profilerControlBlock) + IntPtr.Size) + 4) = 0;
			}

			// Token: 0x0400001F RID: 31
			private const uint PIPE_ACCESS_DUPLEX = 3U;

			// Token: 0x04000020 RID: 32
			private const uint PIPE_TYPE_MESSAGE = 4U;

			// Token: 0x04000021 RID: 33
			private const uint PIPE_READMODE_MESSAGE = 2U;

			// Token: 0x04000022 RID: 34
			private const uint FILE_FLAG_OVERLAPPED = 1073741824U;

			// Token: 0x04000023 RID: 35
			private const uint GENERIC_READ = 2147483648U;

			// Token: 0x04000024 RID: 36
			private const uint GENERIC_WRITE = 1073741824U;

			// Token: 0x04000025 RID: 37
			private const uint OPEN_EXISTING = 3U;

			// Token: 0x04000026 RID: 38
			private const uint PAGE_EXECUTE_READWRITE = 64U;

			// Token: 0x04000027 RID: 39
			private const uint ConfigDWORDInfo_name = 0U;

			// Token: 0x04000028 RID: 40
			private const string ProfAPIMaxWaitForTriggerMs_name = "ProfAPIMaxWaitForTriggerMs";

			// Token: 0x04000029 RID: 41
			private static readonly uint ConfigDWORDInfo_defValue = (uint)IntPtr.Size;

			// Token: 0x0400002A RID: 42
			private IntPtr profilerControlBlock;

			// Token: 0x0400002B RID: 43
			private SafeFileHandle profilerPipe;

			// Token: 0x0400002C RID: 44
			private bool wasAttached;
		}

		// Token: 0x0200000D RID: 13
		private class PEInfo
		{
			// Token: 0x0600004B RID: 75 RVA: 0x0000228E File Offset: 0x0000048E
			public PEInfo(IntPtr addr)
			{
				this.imageBase = addr;
				this.Init();
			}

			// Token: 0x0600004C RID: 76
			[DllImport("kernel32", CharSet = CharSet.Auto)]
			private static extern IntPtr GetModuleHandle(string name);

			// Token: 0x0600004D RID: 77 RVA: 0x00004400 File Offset: 0x00002600
			public static AntiDebugAntinet.PEInfo GetCLR()
			{
				IntPtr clraddress = AntiDebugAntinet.PEInfo.GetCLRAddress();
				if (clraddress == IntPtr.Zero)
				{
					return null;
				}
				return new AntiDebugAntinet.PEInfo(clraddress);
			}

			// Token: 0x0600004E RID: 78 RVA: 0x000022A3 File Offset: 0x000004A3
			private static IntPtr GetCLRAddress()
			{
				if (Environment.Version.Major == 2)
				{
					return AntiDebugAntinet.PEInfo.GetModuleHandle("mscorwks");
				}
				return AntiDebugAntinet.PEInfo.GetModuleHandle("clr");
			}

			// Token: 0x0600004F RID: 79 RVA: 0x00004428 File Offset: 0x00002628
			private unsafe void Init()
			{
				byte* ptr = (byte*)((void*)this.imageBase);
				ptr += *(uint*)(ptr + 60);
				ptr += 6;
				this.numSects = (int)(*(ushort*)ptr);
				ptr += 18;
				bool flag = *(ushort*)ptr == 267;
				uint num = *(uint*)(ptr + 56);
				this.imageEnd = new IntPtr((void*)((byte*)((void*)this.imageBase) + num));
				ptr += (flag ? 96 : 112);
				ptr += 128;
				this.sectionsAddr = new IntPtr((void*)ptr);
			}

			// Token: 0x06000050 RID: 80 RVA: 0x000022C7 File Offset: 0x000004C7
			public unsafe bool IsValidImageAddress(IntPtr addr)
			{
				return this.IsValidImageAddress((void*)addr, 0U);
			}

			// Token: 0x06000051 RID: 81 RVA: 0x000022D6 File Offset: 0x000004D6
			public unsafe bool IsValidImageAddress(IntPtr addr, uint size)
			{
				return this.IsValidImageAddress((void*)addr, size);
			}

			// Token: 0x06000052 RID: 82 RVA: 0x000022E5 File Offset: 0x000004E5
			public unsafe bool IsValidImageAddress(void* addr)
			{
				return this.IsValidImageAddress(addr, 0U);
			}

			// Token: 0x06000053 RID: 83 RVA: 0x000044A4 File Offset: 0x000026A4
			public unsafe bool IsValidImageAddress(void* addr, uint size)
			{
				if (addr < (void*)this.imageBase)
				{
					return false;
				}
				if (addr >= (void*)this.imageEnd)
				{
					return false;
				}
				if (size != 0U)
				{
					if ((byte*)addr + size < (byte*)addr)
					{
						return false;
					}
					if ((byte*)addr + size != (byte*)((void*)this.imageEnd))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06000054 RID: 84 RVA: 0x000044F4 File Offset: 0x000026F4
			public unsafe bool FindSection(string name, out IntPtr sectionStart, out uint sectionSize)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(name + "\0\0\0\0\0\0\0\0");
				for (int i = 0; i < this.numSects; i++)
				{
					byte* ptr = (byte*)((void*)this.sectionsAddr) + i * 40;
					if (AntiDebugAntinet.PEInfo.CompareSectionName(ptr, bytes))
					{
						sectionStart = new IntPtr((void*)((byte*)((void*)this.imageBase) + *(uint*)(ptr + 12)));
						sectionSize = Math.Max(*(uint*)(ptr + 8), *(uint*)(ptr + 16));
						return true;
					}
				}
				sectionStart = IntPtr.Zero;
				sectionSize = 0U;
				return false;
			}

			// Token: 0x06000055 RID: 85 RVA: 0x00004578 File Offset: 0x00002778
			private unsafe static bool CompareSectionName(byte* sectionName, byte[] nameBytes)
			{
				for (int i = 0; i < 8; i++)
				{
					if (*sectionName != nameBytes[i])
					{
						return false;
					}
					sectionName++;
				}
				return true;
			}

			// Token: 0x06000056 RID: 86 RVA: 0x000022EF File Offset: 0x000004EF
			public static bool IsAlignedPointer(IntPtr addr)
			{
				return ((int)addr.ToInt64() & IntPtr.Size - 1) == 0;
			}

			// Token: 0x06000057 RID: 87 RVA: 0x00002304 File Offset: 0x00000504
			public static bool IsAligned(IntPtr addr, uint alignment)
			{
				return ((uint)addr.ToInt64() & alignment - 1U) == 0U;
			}

			// Token: 0x0400002D RID: 45
			private readonly IntPtr imageBase;

			// Token: 0x0400002E RID: 46
			private IntPtr imageEnd;

			// Token: 0x0400002F RID: 47
			private int numSects;

			// Token: 0x04000030 RID: 48
			private IntPtr sectionsAddr;
		}
	}
}
