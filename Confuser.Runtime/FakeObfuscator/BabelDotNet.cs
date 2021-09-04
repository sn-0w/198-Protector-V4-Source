using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x0200003A RID: 58
	public class BabelDotNet
	{
		// Token: 0x060000CD RID: 205 RVA: 0x000026C8 File Offset: 0x000008C8
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(BabelDotNet.BabelAssemblyResolver),
				typeof(BabelDotNet.BabelStringDecrypter)
			};
		}

		// Token: 0x0200003B RID: 59
		internal class BabelAssemblyResolver
		{
			// Token: 0x060000CF RID: 207 RVA: 0x000076D4 File Offset: 0x000058D4
			private static void Register()
			{
				try
				{
					AppDomain.CurrentDomain.AssemblyResolve += BabelDotNet.BabelAssemblyResolver.OnAssemblyResolve;
				}
				catch (Exception)
				{
				}
			}

			// Token: 0x060000D0 RID: 208 RVA: 0x0000770C File Offset: 0x0000590C
			private static Assembly OnAssemblyResolve(object o, ResolveEventArgs e)
			{
				Assembly result;
				try
				{
					result = null;
				}
				catch (Exception)
				{
					result = null;
				}
				return result;
			}

			// Token: 0x060000D1 RID: 209 RVA: 0x000020DB File Offset: 0x000002DB
			private static void Decrypt(Stream str)
			{
			}

			// Token: 0x040000BB RID: 187
			private object _o;

			// Token: 0x040000BC RID: 188
			private int _i;

			// Token: 0x040000BD RID: 189
			private Hashtable _h;
		}

		// Token: 0x0200003C RID: 60
		internal class BabelStringDecrypter
		{
			// Token: 0x060000D3 RID: 211 RVA: 0x000026EA File Offset: 0x000008EA
			private static string Decrypt(int i)
			{
				return "";
			}

			// Token: 0x0200003D RID: 61
			private class NestedType
			{
				// Token: 0x060000D6 RID: 214 RVA: 0x000026EA File Offset: 0x000008EA
				private string Decrypt(int i)
				{
					return "";
				}

				// Token: 0x040000BE RID: 190
				private Hashtable _o1;

				// Token: 0x040000BF RID: 191
				private BabelDotNet.BabelStringDecrypter.NestedType _o2;
			}
		}
	}
}
