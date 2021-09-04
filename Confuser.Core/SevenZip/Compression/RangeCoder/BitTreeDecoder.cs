using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x02000010 RID: 16
	internal struct BitTreeDecoder
	{
		// Token: 0x06000038 RID: 56 RVA: 0x0000223A File Offset: 0x0000043A
		public BitTreeDecoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitDecoder[1 << numBitLevels];
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000478C File Offset: 0x0000298C
		public void Init()
		{
			uint num = 1U;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)num].Init();
				num += 1U;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000047C8 File Offset: 0x000029C8
		public uint Decode(Decoder rangeDecoder)
		{
			uint num = 1U;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				num = (num << 1) + this.Models[(int)num].Decode(rangeDecoder);
			}
			return num - (1U << this.NumBitLevels);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004818 File Offset: 0x00002A18
		public uint ReverseDecode(Decoder rangeDecoder)
		{
			uint num = 1U;
			uint num2 = 0U;
			for (int i = 0; i < this.NumBitLevels; i++)
			{
				uint num3 = this.Models[(int)num].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00004870 File Offset: 0x00002A70
		public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
		{
			uint num = 1U;
			uint num2 = 0U;
			for (int i = 0; i < NumBitLevels; i++)
			{
				uint num3 = Models[(int)(startIndex + num)].Decode(rangeDecoder);
				num <<= 1;
				num += num3;
				num2 |= num3 << i;
			}
			return num2;
		}

		// Token: 0x0400002B RID: 43
		private readonly BitDecoder[] Models;

		// Token: 0x0400002C RID: 44
		private readonly int NumBitLevels;
	}
}
