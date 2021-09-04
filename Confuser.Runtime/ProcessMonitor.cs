using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Confuser.Runtime
{
	// Token: 0x02000054 RID: 84
	internal static class ProcessMonitor
	{
		// Token: 0x06000117 RID: 279 RVA: 0x0000289B File Offset: 0x00000A9B
		private static void Init()
		{
			new Thread(delegate()
			{
				ProcessMonitor.DoWork();
			})
			{
				IsBackground = true
			}.Start();
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00007D38 File Offset: 0x00005F38
		private static void DoWork()
		{
			string[] array = new string[]
			{
				"solarwinds",
				"paessler",
				"cpacket",
				"wireshark",
				"Ethereal",
				"sectools",
				"riverbed",
				"tcpdump",
				"Kismet",
				"EtherApe",
				"Fiddler",
				"telerik",
				"glasswire",
				"HTTPDebuggerSvc",
				"HTTPDebuggerUI",
				"charles",
				"intercepter",
				"snpa",
				"dumcap",
				"comview",
				"netcheat",
				"cheat",
				"winpcap",
				"megadumper",
				"MegaDumper",
				"dnspy",
				"ilspy",
				"reflector"
			};
			if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 2)
			{
				Process.GetCurrentProcess().Kill();
			}
			for (;;)
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					if (Process.GetProcessesByName(array2[i]).Length != 0)
					{
						Environment.Exit(0);
					}
				}
				foreach (Process process in Process.GetProcesses())
				{
					foreach (string value in array)
					{
						bool flag = false;
						if (process.MainWindowTitle.ToLower().Contains(value) || process.ProcessName.ToLower().Contains(value) || process.MainWindowTitle.ToLower().Contains(value) || process.ProcessName.ToLower().Contains(value))
						{
							flag = true;
						}
						if (flag)
						{
							try
							{
								process.Kill();
							}
							catch
							{
								Environment.Exit(0);
							}
						}
					}
				}
				Thread.Sleep(3000);
			}
		}
	}
}
