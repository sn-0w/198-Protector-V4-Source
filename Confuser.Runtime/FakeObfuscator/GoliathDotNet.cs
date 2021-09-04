using System;
using System.Collections.Generic;
using System.Reflection;

namespace Confuser.Runtime.FakeObfuscator
{
	// Token: 0x0200004C RID: 76
	public class GoliathDotNet
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x000027B2 File Offset: 0x000009B2
		public static Type[] GetTypes()
		{
			return new Type[]
			{
				typeof(GoliathDotNet.GoliathStrongNameChecker)
			};
		}

		// Token: 0x0200004D RID: 77
		internal class GoliathStrongNameChecker
		{
			// Token: 0x060000F6 RID: 246 RVA: 0x00007800 File Offset: 0x00005A00
			public static void AntiTamper(Type t)
			{
				Exception ex = new Exception();
				throw ex;
			}

			// Token: 0x060000F7 RID: 247 RVA: 0x000027C7 File Offset: 0x000009C7
			public byte[] RequiredMethod(Assembly s)
			{
				return null;
			}

			// Token: 0x060000F8 RID: 248 RVA: 0x000027C7 File Offset: 0x000009C7
			public string RequiredMethod(Stack<int> s)
			{
				return null;
			}

			// Token: 0x060000F9 RID: 249 RVA: 0x00002762 File Offset: 0x00000962
			public int RequiredMethod(int i, byte[] b)
			{
				return 0;
			}
		}
	}
}
