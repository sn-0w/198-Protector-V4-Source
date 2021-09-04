using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000051 RID: 81
	internal static class AntiFiddler
	{
		// Token: 0x0600010A RID: 266
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr h, string m, string c, int type);

		// Token: 0x0600010B RID: 267 RVA: 0x00002843 File Offset: 0x00000A43
		private static void Init()
		{
			AntiFiddler.Invoke();
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000284A File Offset: 0x00000A4A
		private static void Invoke()
		{
			AntiFiddler.Read();
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00007BF4 File Offset: 0x00005DF4
		private static void Closeprogram()
		{
			string location = Assembly.GetExecutingAssembly().Location;
			Process.Start(new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + location + "\"")
			{
				WindowStyle = ProcessWindowStyle.Hidden
			}).Dispose();
			Environment.Exit(0);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00007C40 File Offset: 0x00005E40
		private static void Read()
		{
			Process[] processesByName = Process.GetProcessesByName("Fiddler");
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Programs\\Fiddler\\App.ico"))
			{
				AntiFiddler.MessageBox((IntPtr)0, "Fiddler has been detected on ur computer, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				AntiFiddler.Closeprogram();
				return;
			}
			if (processesByName.Length != 0)
			{
				AntiFiddler.MessageBox((IntPtr)0, "Fiddler process is openned atm, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				AntiFiddler.Closeprogram();
			}
		}
	}
}
