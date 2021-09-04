using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000036 RID: 54
	internal static class AntiDebugSafe
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00007100 File Offset: 0x00005300
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
			new Thread(new ParameterizedThreadStart(AntiDebugSafe.Worker))
			{
				IsBackground = true
			}.Start(null);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00007190 File Offset: 0x00005390
		private static void Worker(object thread)
		{
			Thread thread2 = thread as Thread;
			if (thread2 == null)
			{
				thread2 = new Thread(new ParameterizedThreadStart(AntiDebugSafe.Worker));
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
				if (!thread2.IsAlive)
				{
					Environment.FailFast(null);
				}
				Thread.Sleep(1000);
			}
		}
	}
}
