using System;
using System.Text;

namespace Confuser.Runtime.AntiMemoryEditing
{
	// Token: 0x02000050 RID: 80
	public class ObfuscatedValue<T>
	{
		// Token: 0x060000FF RID: 255 RVA: 0x000027DF File Offset: 0x000009DF
		public ObfuscatedValue(T val)
		{
			this._salt = ObfuscatedValue<T>.Rand.Next(int.MinValue, int.MaxValue);
			this._obf = ObfuscatedValue<T>.Obfuscate(val, this._salt);
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00002813 File Offset: 0x00000A13
		public static implicit operator T(ObfuscatedValue<T> value)
		{
			return ObfuscatedValue<T>.Deobfuscate(value._obf, value._salt);
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00002826 File Offset: 0x00000A26
		public static implicit operator ObfuscatedValue<T>(T value)
		{
			return new ObfuscatedValue<T>(value);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00007834 File Offset: 0x00005A34
		public override string ToString()
		{
			T t = ObfuscatedValue<T>.Deobfuscate(this._obf, this._salt);
			return t.ToString();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000282E File Offset: 0x00000A2E
		private static T Deobfuscate(T currentvalue, int salt)
		{
			return ObfuscatedValue<T>.Obfuscate(currentvalue, salt);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007860 File Offset: 0x00005A60
		private static T Obfuscate(T currentvalue, int salt)
		{
			Type type = currentvalue.GetType();
			string text = currentvalue as string;
			if (text != null)
			{
				return (T)((object)ObfuscatedValue<T>.XorS(text, salt));
			}
			if (currentvalue is sbyte)
			{
				sbyte b = currentvalue as sbyte;
				return (T)((object)((int)b ^ salt));
			}
			if (currentvalue is byte)
			{
				byte b2 = currentvalue as byte;
				return (T)((object)((int)b2 ^ salt));
			}
			if (currentvalue is short)
			{
				short num = currentvalue as short;
				return (T)((object)((int)num ^ salt));
			}
			if (currentvalue is ushort)
			{
				ushort num2 = currentvalue as ushort;
				return (T)((object)((int)num2 ^ salt));
			}
			if (currentvalue is int)
			{
				int num3 = currentvalue as int;
				return (T)((object)(num3 ^ salt));
			}
			if (currentvalue is uint)
			{
				uint num4 = currentvalue as uint;
				return (T)((object)((long)((ulong)num4 ^ (ulong)((long)salt))));
			}
			if (currentvalue is long)
			{
				long num5 = currentvalue as long;
				return (T)((object)(num5 ^ (long)salt + ((long)salt << 32)));
			}
			if (currentvalue is ulong)
			{
				ulong num6 = currentvalue as ulong;
				return (T)((object)(num6 ^ (ulong)((long)salt + ((long)salt << 32))));
			}
			if (currentvalue is float)
			{
				float f = currentvalue as float;
				return (T)((object)ObfuscatedValue<T>.XorF(f, salt));
			}
			if (currentvalue is double)
			{
				double d = currentvalue as double;
				return (T)((object)ObfuscatedValue<T>.XorD(d, (long)salt + ((long)salt << 32)));
			}
			if (currentvalue is decimal)
			{
				decimal m = currentvalue as decimal;
				return (T)((object)ObfuscatedValue<T>.XorM(m, salt));
			}
			if (type.BaseType == typeof(Enum))
			{
				return (T)((object)((int)Convert.ChangeType(currentvalue, typeof(int)) ^ salt));
			}
			throw new NotSupportedException();
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007B2C File Offset: 0x00005D2C
		private unsafe static float XorF(float f, int salt)
		{
			int num = *(int*)(&f) ^ salt;
			return *(float*)(&num);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007B44 File Offset: 0x00005D44
		private unsafe static double XorD(double d, long salt)
		{
			long num = *(long*)(&d) ^ salt;
			return *(double*)(&num);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007B5C File Offset: 0x00005D5C
		private static decimal XorM(decimal m, int salt)
		{
			int[] bits = decimal.GetBits(m);
			bits[0] ^= salt;
			bits[1] ^= salt;
			bits[2] ^= salt;
			bits[3] ^= (salt & -2145452032);
			return new decimal(bits);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007BAC File Offset: 0x00005DAC
		private static string XorS(string str, int salt)
		{
			StringBuilder stringBuilder = new StringBuilder(str.Length);
			foreach (char c in str)
			{
				stringBuilder.Append((char)((int)c ^ salt));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040000CA RID: 202
		private static readonly Random Rand = new Random();

		// Token: 0x040000CB RID: 203
		private readonly int _salt;

		// Token: 0x040000CC RID: 204
		private T _obf;
	}
}
