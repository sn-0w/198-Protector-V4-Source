using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Confuser.Runtime
{
	// Token: 0x02000053 RID: 83
	internal static class ForceElevation
	{
		// Token: 0x06000114 RID: 276
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr h, string m, string c, int type);

		// Token: 0x06000115 RID: 277 RVA: 0x0000285F File Offset: 0x00000A5F
		private static void Init()
		{
			if (!ForceElevation.Invoke())
			{
				ForceElevation.MessageBox((IntPtr)0, " You need to start the program as admin.", "G.L.O.B.A.L.F.A.L.L", 0);
				Environment.Exit(0);
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00002885 File Offset: 0x00000A85
		public static bool Invoke()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}
	}
}
