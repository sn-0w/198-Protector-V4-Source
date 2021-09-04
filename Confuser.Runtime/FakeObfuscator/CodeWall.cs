using System;
using System.Collections.Generic;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x02000040 RID: 64
	public class CodeWall
	{
		// Token: 0x060000DA RID: 218 RVA: 0x00002706 File Offset: 0x00000906
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(CodeWall.CodeWallStringDecrypter)
			};
		}

		// Token: 0x02000041 RID: 65
		internal static class CodeWallStringDecrypter
		{
			// Token: 0x060000DC RID: 220 RVA: 0x00007758 File Offset: 0x00005958
			private static string Decrypt(int x, int y, int z)
			{
				return null;
			}

			// Token: 0x040000C0 RID: 192
			private static object _o;

			// Token: 0x040000C1 RID: 193
			private static Dictionary<int, string> _d = new Dictionary<int, string>();
		}
	}
}
