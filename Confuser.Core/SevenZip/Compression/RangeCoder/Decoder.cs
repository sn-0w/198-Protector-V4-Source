using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000C RID: 12
	internal class Decoder
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00004038 File Offset: 0x00002238
		public void Init(Stream stream)
		{
			this.Stream = stream;
			this.Code = 0U;
			this.Range = uint.MaxValue;
			for (int i = 0; i < 5; i++)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000021BD File Offset: 0x000003BD
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000021C7 File Offset: 0x000003C7
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00004088 File Offset: 0x00002288
		public void Normalize()
		{
			while (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000040D4 File Offset: 0x000022D4
		public void Normalize2()
		{
			bool flag = this.Range < 16777216U;
			if (flag)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00004120 File Offset: 0x00002320
		public uint GetThreshold(uint total)
		{
			return this.Code / (this.Range /= total);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000021D6 File Offset: 0x000003D6
		public void Decode(uint start, uint size, uint total)
		{
			this.Code -= start * this.Range;
			this.Range *= size;
			this.Normalize();
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000414C File Offset: 0x0000234C
		public uint DecodeDirectBits(int numTotalBits)
		{
			uint num = this.Range;
			uint num2 = this.Code;
			uint num3 = 0U;
			for (int i = numTotalBits; i > 0; i--)
			{
				num >>= 1;
				uint num4 = num2 - num >> 31;
				num2 -= (num & num4 - 1U);
				num3 = (num3 << 1 | 1U - num4);
				bool flag = num < 16777216U;
				if (flag)
				{
					num2 = (num2 << 8 | (uint)((byte)this.Stream.ReadByte()));
					num <<= 8;
				}
			}
			this.Range = num;
			this.Code = num2;
			return num3;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000041D8 File Offset: 0x000023D8
		public uint DecodeBit(uint size0, int numTotalBits)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			bool flag = this.Code < num;
			uint result;
			if (flag)
			{
				result = 0U;
				this.Range = num;
			}
			else
			{
				result = 1U;
				this.Code -= num;
				this.Range -= num;
			}
			this.Normalize();
			return result;
		}

		// Token: 0x0400001A RID: 26
		public const uint kTopValue = 16777216U;

		// Token: 0x0400001B RID: 27
		public uint Code;

		// Token: 0x0400001C RID: 28
		public uint Range;

		// Token: 0x0400001D RID: 29
		public Stream Stream;
	}
}
