using System;
using System.Reflection;

namespace Confuser.Runtime
{
	// Token: 0x02000037 RID: 55
	internal static class Resource
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x00007204 File Offset: 0x00005404
		private static void Initialize()
		{
			uint keyI = (uint)Mutation.KeyI0;
			uint[] array = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			uint[] array2 = new uint[16];
			uint num = (uint)Mutation.KeyI1;
			for (int i = 0; i < 16; i++)
			{
				num ^= num >> 13;
				num ^= num << 25;
				num ^= num >> 27;
				array2[i] = num;
			}
			int num2 = 0;
			int num3 = 0;
			uint[] array3 = new uint[16];
			byte[] array4 = new byte[keyI * 4U];
			while ((long)num2 < (long)((ulong)keyI))
			{
				for (int j = 0; j < 16; j++)
				{
					array3[j] = array[num2 + j];
				}
				Mutation.Crypt(array3, array2);
				for (int k = 0; k < 16; k++)
				{
					uint num4 = array3[k];
					array4[num3++] = (byte)num4;
					array4[num3++] = (byte)(num4 >> 8);
					array4[num3++] = (byte)(num4 >> 16);
					array4[num3++] = (byte)(num4 >> 24);
					array2[k] ^= num4;
				}
				num2 += 16;
			}
			Resource.c = Assembly.Load(Lzma.Decompress(array4));
			AppDomain.CurrentDomain.AssemblyResolve += Resource.Handler;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000026A8 File Offset: 0x000008A8
		private static Assembly Handler(object sender, ResolveEventArgs args)
		{
			if (Resource.c.FullName == args.Name)
			{
				return Resource.c;
			}
			return null;
		}

		// Token: 0x040000B9 RID: 185
		private static Assembly c;
	}
}
