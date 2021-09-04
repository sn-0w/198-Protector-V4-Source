using System;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x0200004E RID: 78
	public class Xenocode
	{
		// Token: 0x060000FB RID: 251 RVA: 0x000027CA File Offset: 0x000009CA
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(Xenocode.XenocodeStringDecrypter)
			};
		}

		// Token: 0x0200004F RID: 79
		internal class XenocodeStringDecrypter
		{
			// Token: 0x060000FD RID: 253 RVA: 0x00007818 File Offset: 0x00005A18
			public string Decrypt(string x, int y)
			{
				return 1789.ToString();
			}
		}
	}
}
