using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Confuser.Runtime
{
	// Token: 0x02000011 RID: 17
	internal static class Constant
	{
		// Token: 0x06000060 RID: 96 RVA: 0x00004C84 File Offset: 0x00002E84
		private static void Initialize()
		{
			uint keyI = (uint)Mutation.KeyI0;
			uint[] array = Mutation.Placeholder<uint[]>(new uint[Mutation.KeyI0]);
			uint[] array2 = new uint[16];
			uint num = (uint)Mutation.KeyI1;
			for (int i = 0; i < 16; i++)
			{
				num ^= num >> 12;
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
			Constant.b = Lzma.Decompress(array4);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00004DA4 File Offset: 0x00002FA4
		private static uint XorShit(string text, string key, string meme)
		{
			StringBuilder stringBuilder = new StringBuilder();
			key += meme;
			for (int i = 0; i < text.Length; i++)
			{
				stringBuilder.Append(text[i] ^ key[i % key.Length]);
			}
			return uint.Parse(stringBuilder.ToString());
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004DFC File Offset: 0x00002FFC
		private static T Get<T>(uint id, string idT, uint id4, string id5)
		{
			T result = default(T);
			if (!object.Equals(Assembly.GetCallingAssembly(), Assembly.GetExecutingAssembly()) || new StackTrace().GetFrame(1).GetMethod().DeclaringType == typeof(RuntimeMethodHandle))
			{
				return result;
			}
			id = Mutation.ForXor(idT, id4.ToString(), id5);
			id = (uint)Mutation.Placeholder<int>((int)id);
			uint num = id >> 30;
			id &= 1073741823U;
			id <<= 2;
			if ((ulong)num == (ulong)((long)Mutation.KeyI0))
			{
				int count = (int)Constant.b[(int)id++] | (int)Constant.b[(int)id++] << 8 | (int)Constant.b[(int)id++] << 16 | (int)Constant.b[(int)id++] << 24;
				result = (T)((object)string.Intern(Encoding.UTF8.GetString(Constant.b, (int)id, count)));
				id4 = id;
			}
			else if ((ulong)num == (ulong)((long)Mutation.KeyI1))
			{
				T[] array = new T[1];
				Buffer.BlockCopy(Constant.b, (int)id, array, 0, Mutation.Value<int>());
				result = array[0];
				id4 = id;
			}
			else if ((ulong)num == (ulong)((long)Mutation.KeyI2))
			{
				int num2 = (int)Constant.b[(int)id++] | (int)Constant.b[(int)id++] << 8 | (int)Constant.b[(int)id++] << 16 | (int)Constant.b[(int)id++] << 24;
				int length = (int)Constant.b[(int)id++] | (int)Constant.b[(int)id++] << 8 | (int)Constant.b[(int)id++] << 16 | (int)Constant.b[(int)id++] << 24;
				Array array2 = Array.CreateInstance(typeof(T).GetElementType(), length);
				Buffer.BlockCopy(Constant.b, (int)id, array2, 0, num2 - 4);
				result = (T)((object)array2);
				id4 = id;
			}
			return result;
		}

		// Token: 0x04000032 RID: 50
		private static byte[] b;
	}
}
