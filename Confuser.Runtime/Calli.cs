using System;

namespace Confuser.Runtime
{
	// Token: 0x0200000F RID: 15
	internal static class Calli
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000048C8 File Offset: 0x00002AC8
		public static IntPtr ResolveToken(int token)
		{
			return typeof(Calli).Module.ResolveMethod(token).MethodHandle.GetFunctionPointer();
		}
	}
}
