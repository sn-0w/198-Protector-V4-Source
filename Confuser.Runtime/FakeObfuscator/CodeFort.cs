using System;
using System.Globalization;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x0200003E RID: 62
	public class CodeFort
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x000026F1 File Offset: 0x000008F1
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(CodeFort.CodeFortStringDecrypter)
			};
		}

		// Token: 0x0200003F RID: 63
		internal static class CodeFortStringDecrypter
		{
			// Token: 0x060000D9 RID: 217 RVA: 0x00007734 File Offset: 0x00005934
			private static string Decrypt(string s)
			{
				return 3992.0.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
