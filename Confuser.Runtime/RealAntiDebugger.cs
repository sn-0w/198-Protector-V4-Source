using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000059 RID: 89
	internal static class RealAntiDebugger
	{
		// Token: 0x06000124 RID: 292 RVA: 0x0000290B File Offset: 0x00000B0B
		private static void Initialize()
		{
			new Thread(new ThreadStart(RealAntiDebugger.CheckIt)).Start();
		}

		// Token: 0x06000125 RID: 293 RVA: 0x000081CC File Offset: 0x000063CC
		public static void CheckIt()
		{
			for (;;)
			{
				Process[] processes = Process.GetProcesses();
				string text = string.Empty;
				for (int i = 0; i < processes.Length; i++)
				{
					text = processes[i].MainWindowTitle;
					if (text.Contains("CodeCracker") || text.Contains("dnSpy") || text.Contains("MegaDumper") || text.Contains("Process Hacker") || text.Contains("OllyDbg") || text.Contains("HxD") || text.Contains("Task-Manager") || text.Contains("xVenoxi Dumper") || text.Contains("NativeDumper MFC Application") || text.Contains("JetBrains dotPeek") || text.Contains("ILSpy") || text.Contains("Reflector"))
					{
						string location = Assembly.GetExecutingAssembly().Location;
						if (location == "" || location == null)
						{
							location = Assembly.GetEntryAssembly().Location;
						}
						Process.Start(new ProcessStartInfo
						{
							FileName = "cmd.exe",
							Arguments = "/C ping 1.1.1.1 -n 1 -w 4000 > Nul & Del \"" + location + "\"",
							CreateNoWindow = true,
							WindowStyle = ProcessWindowStyle.Hidden
						});
						Environment.Exit(0);
					}
				}
			}
		}
	}
}
