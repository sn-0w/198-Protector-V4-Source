using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000004 RID: 4
	internal static class AntiDebugUltimate
	{
		// Token: 0x06000009 RID: 9 RVA: 0x000029B4 File Offset: 0x00000BB4
		private static void Initialize()
		{
			string str = "COR";
			Type typeFromHandle = typeof(Environment);
			MethodInfo method = typeFromHandle.GetMethod("GetEnvironmentVariable", new Type[]
			{
				typeof(string)
			});
			if (method != null && "1".Equals(method.Invoke(null, new object[]
			{
				str + "_ENABLE_PROFILING"
			})))
			{
				Environment.FailFast(null);
			}
			if (Environment.GetEnvironmentVariable(str + "_PROFILER") != null || Environment.GetEnvironmentVariable(str + "_ENABLE_PROFILING") != null)
			{
				Environment.FailFast(null);
			}
			new Thread(new ParameterizedThreadStart(AntiDebugUltimate.Worker))
			{
				IsBackground = true
			}.Start(null);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002A6C File Offset: 0x00000C6C
		private static void Worker(object thread)
		{
			Thread thread2 = thread as Thread;
			if (thread2 == null)
			{
				thread2 = new Thread(new ParameterizedThreadStart(AntiDebugUltimate.Worker));
				thread2.IsBackground = true;
				thread2.Start(Thread.CurrentThread);
				Thread.Sleep(500);
			}
			for (;;)
			{
				if (Debugger.IsAttached || Debugger.IsLogging())
				{
					Environment.FailFast(null);
				}
				bool flag = false;
				AntiDebugUltimate.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref flag);
				if (flag)
				{
					Environment.FailFast(null);
				}
				if (AntiDebugUltimate.IsDebuggerPresent())
				{
					Environment.FailFast(null);
				}
				Process currentProcess = Process.GetCurrentProcess();
				if (currentProcess.Handle == IntPtr.Zero)
				{
					Environment.FailFast("");
				}
				currentProcess.Close();
				if (AntiDebugUltimate.OutputDebugString("") > IntPtr.Size)
				{
					Environment.FailFast("");
				}
				if (!thread2.IsAlive)
				{
					Environment.FailFast(null);
				}
				Thread.Sleep(1000);
			}
		}

		// Token: 0x0600000B RID: 11
		[DllImport("kernel32.dll")]
		private static extern bool IsDebuggerPresent();

		// Token: 0x0600000C RID: 12
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

		// Token: 0x0600000D RID: 13
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int OutputDebugString(string str);

		// Token: 0x0600000E RID: 14
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);
	}
}
