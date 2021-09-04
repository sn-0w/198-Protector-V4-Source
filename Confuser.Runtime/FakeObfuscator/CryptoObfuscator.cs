using System;
using System.Collections.Generic;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x02000042 RID: 66
	public class CryptoObfuscator
	{
		// Token: 0x060000DE RID: 222 RVA: 0x00002727 File Offset: 0x00000927
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(CryptoObfuscator.CryptoObfuscatorMethodDecrypter),
				typeof(CryptoObfuscator.CryptoObfuscatorStringDecrypter),
				typeof(CryptoObfuscator.CryptoObfuscatorConstantsDecrypter)
			};
		}

		// Token: 0x02000043 RID: 67
		internal static class CryptoObfuscatorMethodDecrypter
		{
			// Token: 0x060000E0 RID: 224 RVA: 0x00007778 File Offset: 0x00005978
			public static void Decrypt(int x, int y, int z)
			{
				ModuleHandle moduleHandle = default(ModuleHandle);
			}

			// Token: 0x040000C2 RID: 194
			private static byte[] _b;

			// Token: 0x040000C3 RID: 195
			private static Dictionary<int, int> _d = new Dictionary<int, int>();

			// Token: 0x040000C4 RID: 196
			private static ModuleHandle _m;

			// Token: 0x02000044 RID: 68
			private class Nested
			{
			}
		}

		// Token: 0x02000045 RID: 69
		internal static class CryptoObfuscatorStringDecrypter
		{
			// Token: 0x060000E3 RID: 227 RVA: 0x000026EA File Offset: 0x000008EA
			private static string Decrypt(int i)
			{
				return "";
			}

			// Token: 0x060000E4 RID: 228 RVA: 0x000020DB File Offset: 0x000002DB
			private static void ExtraMethod()
			{
			}

			// Token: 0x040000C5 RID: 197
			private static byte[] _b;
		}

		// Token: 0x02000046 RID: 70
		internal static class CryptoObfuscatorConstantsDecrypter
		{
			// Token: 0x060000E5 RID: 229 RVA: 0x00002762 File Offset: 0x00000962
			private static int RequiredMethod1(int a)
			{
				return 0;
			}

			// Token: 0x060000E6 RID: 230 RVA: 0x00002765 File Offset: 0x00000965
			private static long RequiredMethod2(int a)
			{
				return 0L;
			}

			// Token: 0x060000E7 RID: 231 RVA: 0x00002769 File Offset: 0x00000969
			private static float RequiredMethod3(int a)
			{
				return 0f;
			}

			// Token: 0x060000E8 RID: 232 RVA: 0x00002770 File Offset: 0x00000970
			private static double RequiredMethod4(int a)
			{
				return 0.0;
			}

			// Token: 0x060000E9 RID: 233 RVA: 0x000020DB File Offset: 0x000002DB
			private static void RequiredMethod5(Array arr, int a)
			{
			}

			// Token: 0x060000EA RID: 234 RVA: 0x000020DB File Offset: 0x000002DB
			private static void Extra()
			{
			}

			// Token: 0x040000C6 RID: 198
			private static byte[] _b = new byte[0];
		}
	}
}
