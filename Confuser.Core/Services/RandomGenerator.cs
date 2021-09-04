using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using dnlib.DotNet.Writer;

namespace Confuser.Core.Services
{
	// Token: 0x0200007D RID: 125
	public class RandomGenerator
	{
		// Token: 0x060002FA RID: 762 RVA: 0x0000345E File Offset: 0x0000165E
		internal RandomGenerator(byte[] seed)
		{
			this.state = (byte[])seed.Clone();
			this.stateFilled = 32;
			this.mixIndex = 0;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0001347C File Offset: 0x0001167C
		internal static byte[] Seed(string seed)
		{
			bool flag = !string.IsNullOrEmpty(seed);
			byte[] array;
			if (flag)
			{
				array = Utils.SHA256(Encoding.UTF8.GetBytes(seed));
			}
			else
			{
				array = Utils.SHA256(Guid.NewGuid().ToByteArray());
			}
			for (int i = 0; i < 32; i++)
			{
				byte[] array2 = array;
				int num = i;
				array2[num] *= RandomGenerator.primes[i % RandomGenerator.primes.Length];
				array = Utils.SHA256(array);
			}
			return array;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x000134FC File Offset: 0x000116FC
		private void NextState()
		{
			for (int i = 0; i < 32; i++)
			{
				byte[] array = this.state;
				int num = i;
				array[num] ^= RandomGenerator.primes[this.mixIndex = (this.mixIndex + 1) % RandomGenerator.primes.Length];
			}
			this.state = this.sha256.ComputeHash(this.state);
			this.stateFilled = 32;
		}

		// Token: 0x060002FD RID: 765 RVA: 0x0001356C File Offset: 0x0001176C
		public void NextBytes(byte[] buffer, int offset, int length)
		{
			bool flag = buffer == null;
			if (flag)
			{
				throw new ArgumentNullException("buffer");
			}
			bool flag2 = offset < 0;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			bool flag3 = length < 0;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			bool flag4 = buffer.Length - offset < length;
			if (flag4)
			{
				throw new ArgumentException("Invalid offset or length.");
			}
			while (length > 0)
			{
				bool flag5 = length >= this.stateFilled;
				if (flag5)
				{
					Buffer.BlockCopy(this.state, 32 - this.stateFilled, buffer, offset, this.stateFilled);
					offset += this.stateFilled;
					length -= this.stateFilled;
					this.stateFilled = 0;
				}
				else
				{
					Buffer.BlockCopy(this.state, 32 - this.stateFilled, buffer, offset, length);
					this.stateFilled -= length;
					length = 0;
				}
				bool flag6 = this.stateFilled == 0;
				if (flag6)
				{
					this.NextState();
				}
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00013670 File Offset: 0x00011870
		public byte NextByte()
		{
			byte result = this.state[32 - this.stateFilled];
			this.stateFilled--;
			bool flag = this.stateFilled == 0;
			if (flag)
			{
				this.NextState();
			}
			return result;
		}

		// Token: 0x060002FF RID: 767 RVA: 0x000136B8 File Offset: 0x000118B8
		public byte[] NextBytes(int length)
		{
			byte[] array = new byte[length];
			this.NextBytes(array, 0, length);
			return array;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x000136DC File Offset: 0x000118DC
		public int NextInt32()
		{
			return BitConverter.ToInt32(this.NextBytes(4), 0);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x000136FC File Offset: 0x000118FC
		public int NextInt32(int max)
		{
			return (int)((ulong)this.NextUInt32() % (ulong)((long)max));
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0001371C File Offset: 0x0001191C
		public int NextInt32(int min, int max)
		{
			bool flag = max <= min;
			int result;
			if (flag)
			{
				result = min;
			}
			else
			{
				result = min + (int)((ulong)this.NextUInt32() % (ulong)((long)(max - min)));
			}
			return result;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0001374C File Offset: 0x0001194C
		public uint NextUInt32()
		{
			return BitConverter.ToUInt32(this.NextBytes(4), 0);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00003493 File Offset: 0x00001693
		public uint NextUInt32(uint max)
		{
			return this.NextUInt32() % max;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0001376C File Offset: 0x0001196C
		public double NextDouble()
		{
			return this.NextUInt32() / 4294967296.0;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00013790 File Offset: 0x00011990
		public bool NextBoolean()
		{
			byte b = this.state[32 - this.stateFilled];
			this.stateFilled--;
			bool flag = this.stateFilled == 0;
			if (flag)
			{
				this.NextState();
			}
			return b % 2 == 0;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x000137DC File Offset: 0x000119DC
		public void Shuffle<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 1; i--)
			{
				int index = this.NextInt32(i + 1);
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00013830 File Offset: 0x00011A30
		public void Shuffle<T>(MDTable<T> table) where T : struct
		{
			bool isEmpty = table.IsEmpty;
			if (!isEmpty)
			{
				for (uint num = (uint)table.Rows; num > 2U; num -= 1U)
				{
					uint rid = this.NextUInt32(num - 1U) + 1U;
					T value = table[rid];
					table[rid] = table[num];
					table[num] = value;
				}
			}
		}

		// Token: 0x04000234 RID: 564
		private static readonly byte[] primes = new byte[]
		{
			7,
			11,
			23,
			37,
			43,
			59,
			71
		};

		// Token: 0x04000235 RID: 565
		private readonly SHA256Managed sha256 = new SHA256Managed();

		// Token: 0x04000236 RID: 566
		private int mixIndex;

		// Token: 0x04000237 RID: 567
		private byte[] state;

		// Token: 0x04000238 RID: 568
		private int stateFilled;
	}
}
