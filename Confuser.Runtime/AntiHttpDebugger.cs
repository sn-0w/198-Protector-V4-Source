using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000052 RID: 82
	internal static class AntiHttpDebugger
	{
		// Token: 0x0600010F RID: 271
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr h, string m, string c, int type);

		// Token: 0x06000110 RID: 272 RVA: 0x00002851 File Offset: 0x00000A51
		private static void Init()
		{
			AntiHttpDebugger.Invoke();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00002858 File Offset: 0x00000A58
		private static void Invoke()
		{
			AntiHttpDebugger.Read();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00007BF4 File Offset: 0x00005DF4
		private static void Closeprogram()
		{
			string location = Assembly.GetExecutingAssembly().Location;
			Process.Start(new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + location + "\"")
			{
				WindowStyle = ProcessWindowStyle.Hidden
			}).Dispose();
			Environment.Exit(0);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00007CB0 File Offset: 0x00005EB0
		private static void Read()
		{
			if (File.Exists("C:\\Program Files (x86)\\HTTPDebuggerPro\\HTTPDebuggerBrowser.dll"))
			{
				AntiHttpDebugger.MessageBox((IntPtr)0, "Http Debugger has been detected on ur computer, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				AntiHttpDebugger.Closeprogram();
				return;
			}
			if (File.Exists("D:\\Program Files (x86)\\HTTPDebuggerPro\\HTTPDebuggerBrowser.dll"))
			{
				AntiHttpDebugger.MessageBox((IntPtr)0, "Http Debugger has been detected on ur computer, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				AntiHttpDebugger.Closeprogram();
				return;
			}
			if (File.Exists("F:\\Program Files (x86)\\HTTPDebuggerPro\\HTTPDebuggerBrowser.dll"))
			{
				AntiHttpDebugger.MessageBox((IntPtr)0, "Http Debugger has been detected on ur computer, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				AntiHttpDebugger.Closeprogram();
			}
		}
	}
}
