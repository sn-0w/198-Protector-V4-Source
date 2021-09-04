using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Confuser.Core
{
	// Token: 0x02000071 RID: 113
	public static class Utils
	{
		// Token: 0x060002BD RID: 701 RVA: 0x0001213C File Offset: 0x0001033C
		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defValue = default(TValue))
		{
			TValue tvalue;
			bool flag = dictionary.TryGetValue(key, out tvalue);
			TValue result;
			if (flag)
			{
				result = tvalue;
			}
			else
			{
				result = defValue;
			}
			return result;
		}

		// Token: 0x060002BE RID: 702 RVA: 0x00012160 File Offset: 0x00010360
		public static TValue GetValueOrDefaultLazy<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> defValueFactory)
		{
			TValue tvalue;
			bool flag = dictionary.TryGetValue(key, out tvalue);
			TValue result;
			if (flag)
			{
				result = tvalue;
			}
			else
			{
				result = defValueFactory(key);
			}
			return result;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001218C File Offset: 0x0001038C
		public static void AddListEntry<TKey, TValue>(this IDictionary<TKey, List<TValue>> self, TKey key, TValue value)
		{
			bool flag = key == null;
			if (flag)
			{
				throw new ArgumentNullException("key");
			}
			List<TValue> list;
			bool flag2 = !self.TryGetValue(key, out list);
			if (flag2)
			{
				list = (self[key] = new List<TValue>());
			}
			list.Add(value);
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x000121DC File Offset: 0x000103DC
		public static string GetRelativePath(string filespec, string folder)
		{
			Uri uri = new Uri(filespec);
			bool flag = !folder.EndsWith(Path.DirectorySeparatorChar.ToString());
			if (flag)
			{
				folder += Path.DirectorySeparatorChar.ToString();
			}
			Uri uri2 = new Uri(folder);
			return Uri.UnescapeDataString(uri2.MakeRelativeUri(uri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0001224C File Offset: 0x0001044C
		public static string NullIfEmpty(this string val)
		{
			bool flag = string.IsNullOrEmpty(val);
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = val;
			}
			return result;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00012270 File Offset: 0x00010470
		public static byte[] SHA1(byte[] buffer)
		{
			SHA1Managed sha1Managed = new SHA1Managed();
			return sha1Managed.ComputeHash(buffer);
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00012290 File Offset: 0x00010490
		public static byte[] Xor(byte[] buffer1, byte[] buffer2)
		{
			bool flag = buffer1.Length != buffer2.Length;
			if (flag)
			{
				throw new ArgumentException("Length mismatched.");
			}
			byte[] array = new byte[buffer1.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (buffer1[i] ^ buffer2[i]);
			}
			return array;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000122E8 File Offset: 0x000104E8
		public static byte[] SHA256(byte[] buffer)
		{
			SHA256Managed sha256Managed = new SHA256Managed();
			return sha256Managed.ComputeHash(buffer);
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00012308 File Offset: 0x00010508
		public static string EncodeString(byte[] buff, char[] charset)
		{
			int i = (int)buff[0];
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 1; j < buff.Length; j++)
			{
				for (i = (i << 8) + (int)buff[j]; i >= charset.Length; i /= charset.Length)
				{
					stringBuilder.Append(charset[i % charset.Length]);
				}
			}
			bool flag = i != 0;
			if (flag)
			{
				stringBuilder.Append(charset[i % charset.Length]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00012388 File Offset: 0x00010588
		public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int num2 = str.IndexOf(oldValue, comparison); num2 != -1; num2 = str.IndexOf(oldValue, num2, comparison))
			{
				stringBuilder.Append(str.Substring(num, num2 - num));
				stringBuilder.Append(newValue);
				num2 += oldValue.Length;
				num = num2;
			}
			stringBuilder.Append(str.Substring(num));
			return stringBuilder.ToString();
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00012400 File Offset: 0x00010600
		public static string ToHexString(byte[] buff)
		{
			char[] array = new char[buff.Length * 2];
			int num = 0;
			foreach (byte b in buff)
			{
				array[num++] = Utils.hexCharset[b >> 4];
				array[num++] = Utils.hexCharset[(int)(b & 15)];
			}
			return new string(array);
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x00012464 File Offset: 0x00010664
		public static IList<T> RemoveWhere<T>(this IList<T> self, Predicate<T> match)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				bool flag = match(self[i]);
				if (flag)
				{
					self.RemoveAt(i);
				}
			}
			return self;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000330D File Offset: 0x0000150D
		public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> enumerable, ILogger logger)
		{
			List<T> list = new List<T>(enumerable);
			int i;
			int num;
			for (i = 0; i < list.Count; i = num + 1)
			{
				logger.Progress(i, list.Count);
				yield return list[i];
				num = i;
			}
			logger.Progress(i, list.Count);
			logger.EndProgress();
			yield break;
		}

		// Token: 0x04000218 RID: 536
		private static readonly char[] hexCharset = "0123456789abcdef".ToCharArray();
	}
}
