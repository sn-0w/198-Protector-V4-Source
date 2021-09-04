using System;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x02000047 RID: 71
	public class Dotfuscator
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00002788 File Offset: 0x00000988
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(Dotfuscator.DotfuscatorStringDecrypter)
			};
		}

		// Token: 0x02000048 RID: 72
		internal class DotfuscatorStringDecrypter
		{
			// Token: 0x060000EE RID: 238 RVA: 0x000077A8 File Offset: 0x000059A8
			private static string Decrypt(string s, int i)
			{
				string.Intern(s);
				char[] arg = s.ToCharArray();
				string arg2 = 5.ToString();
				return arg2 + arg;
			}
		}
	}
}
