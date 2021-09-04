using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000006 RID: 6
	internal static class AntiDebugWin32
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000031EC File Offset: 0x000013EC
		private static void Initialize()
		{
			string str = "COR";
			if (Environment.GetEnvironmentVariable(str + "_PROFILER") != null || Environment.GetEnvironmentVariable(str + "_ENABLE_PROFILING") != null)
			{
				Environment.FailFast(null);
			}
			new Thread(new ParameterizedThreadStart(AntiDebugWin32.Worker))
			{
				IsBackground = true
			}.Start(null);
		}

		// Token: 0x06000012 RID: 18
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);

		// Token: 0x06000013 RID: 19
		[DllImport("kernel32.dll")]
		private static extern bool IsDebuggerPresent();

		// Token: 0x06000014 RID: 20
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int OutputDebugString(string str);

		// Token: 0x06000015 RID: 21 RVA: 0x0000324C File Offset: 0x0000144C
		private static void Worker(object thread)
		{
			Thread thread2 = thread as Thread;
			if (thread2 == null)
			{
				thread2 = new Thread(new ParameterizedThreadStart(AntiDebugWin32.Worker));
				thread2.IsBackground = true;
				thread2.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			for (;;)
			{
				if (Debugger.IsAttached || Debugger.IsLogging())
				{
					Environment.FailFast("");
				}
				if (AntiDebugWin32.IsDebuggerPresent())
				{
					Environment.FailFast("");
				}
				Process currentProcess = Process.GetCurrentProcess();
				if (currentProcess.Handle == IntPtr.Zero)
				{
					Environment.FailFast("");
				}
				currentProcess.Close();
				if (AntiDebugWin32.OutputDebugString("") > IntPtr.Size)
				{
					Environment.FailFast("");
				}
				try
				{
					AntiDebugWin32.CloseHandle(IntPtr.Zero);
				}
				catch
				{
					Environment.FailFast("");
				}
				if (!thread2.IsAlive)
				{
					Environment.FailFast("");
				}
				Thread.Sleep(1000);
			}
		}
	}
}
