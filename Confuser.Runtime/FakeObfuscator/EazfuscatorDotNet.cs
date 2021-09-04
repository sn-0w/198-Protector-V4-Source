using System;
using System.Reflection;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x02000049 RID: 73
	public class EazfuscatorDotNet
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x0000279D File Offset: 0x0000099D
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(EazfuscatorDotNet.EazfuscatorStringDecrypter)
			};
		}

		// Token: 0x0200004A RID: 74
		internal class EazfuscatorStringDecrypter
		{
			// Token: 0x060000F2 RID: 242 RVA: 0x000077D4 File Offset: 0x000059D4
			public static string Decrypter(int i)
			{
				Assembly assembly = null;
				string text = null;
				assembly.GetManifestResourceStream(text);
				return text;
			}

			// Token: 0x040000C7 RID: 199
			private byte[] _b;

			// Token: 0x040000C8 RID: 200
			private short _s;

			// Token: 0x0200004B RID: 75
			public enum Enum
			{

			}
		}
	}
}
