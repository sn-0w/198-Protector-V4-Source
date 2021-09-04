using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Confuser.Runtime
{
	// Token: 0x02000056 RID: 86
	internal static class AntiDnspy
	{
		// Token: 0x0600011C RID: 284 RVA: 0x00007F50 File Offset: 0x00006150
		private static void Initialize()
		{
			if (File.Exists(Environment.ExpandEnvironmentVariables("%appdata%") + "\\dnSpy\\dnSpy.xml"))
			{
				AntiDnspy.MessageBox((IntPtr)0, "dnSpy has been detected on ur computer, since it can be used for malicious ending, the program will be deleted from ur computer...", "G.L.O.B.A.L.F.A.L.L", 0);
				string location = Assembly.GetExecutingAssembly().Location;
				Process.Start(new ProcessStartInfo("cmd.exe", "/C ping 1.1.1.1 -n 1 -w 3000 > Nul & Del \"" + location + "\"")
				{
					WindowStyle = ProcessWindowStyle.Hidden
				}).Dispose();
				Environment.Exit(0);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x000028E0 File Offset: 0x00000AE0
		internal static void CrossAppDomainSerializer(string A_0)
		{
			Process.Start(new ProcessStartInfo("cmd.exe", "/c " + A_0)
			{
				CreateNoWindow = true,
				UseShellExecute = false
			});
		}

		// Token: 0x0600011E RID: 286
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr h, string m, string c, int type);
	}
}
