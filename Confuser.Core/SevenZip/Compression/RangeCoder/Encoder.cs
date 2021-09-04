using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x0200000B RID: 11
	internal class Encoder
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00002151 File Offset: 0x00000351
		public void SetStream(Stream stream)
		{
			this.Stream = stream;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000215B File Offset: 0x0000035B
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002165 File Offset: 0x00000365
		public void Init()
		{
			this.StartPosition = this.Stream.Position;
			this.Low = 0UL;
			this.Range = uint.MaxValue;
			this._cacheSize = 1U;
			this._cache = 0;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003DDC File Offset: 0x00001FDC
		public void FlushData()
		{
			for (int i = 0; i < 5; i++)
			{
				this.ShiftLow();
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002196 File Offset: 0x00000396
		public void FlushStream()
		{
			this.Stream.Flush();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000021A5 File Offset: 0x000003A5
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003E04 File Offset: 0x00002004
		public void Encode(uint start, uint size, uint total)
		{
			this.Low += (ulong)(start * (this.Range /= total));
			this.Range *= size;
			while (this.Range < 16777216U)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00003E6C File Offset: 0x0000206C
		public void ShiftLow()
		{
			bool flag = (uint)this.Low < 4278190080U || (uint)(this.Low >> 32) == 1U;
			if (flag)
			{
				byte b = this._cache;
				uint num;
				do
				{
					this.Stream.WriteByte((byte)((ulong)b + (this.Low >> 32)));
					b = byte.MaxValue;
					num = this._cacheSize - 1U;
					this._cacheSize = num;
				}
				while (num > 0U);
				this._cache = (byte)((uint)this.Low >> 24);
			}
			this._cacheSize += 1U;
			this.Low = (ulong)((ulong)((uint)this.Low) << 8);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003F10 File Offset: 0x00002110
		public void EncodeDirectBits(uint v, int numTotalBits)
		{
			for (int i = numTotalBits - 1; i >= 0; i--)
			{
				this.Range >>= 1;
				bool flag = (v >> i & 1U) == 1U;
				if (flag)
				{
					this.Low += (ulong)this.Range;
				}
				bool flag2 = this.Range < 16777216U;
				if (flag2)
				{
					this.Range <<= 8;
					this.ShiftLow();
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003F90 File Offset: 0x00002190
		public void EncodeBit(uint size0, int numTotalBits, uint symbol)
		{
			uint num = (this.Range >> numTotalBits) * size0;
			bool flag = symbol == 0U;
			if (flag)
			{
				this.Range = num;
			}
			else
			{
				this.Low += (ulong)num;
				this.Range -= num;
			}
			while (this.Range < 16777216U)
			{
				this.Range <<= 8;
				this.ShiftLow();
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00004008 File Offset: 0x00002208
		public long GetProcessedSizeAdd()
		{
			return (long)((ulong)this._cacheSize + (ulong)this.Stream.Position - (ulong)this.StartPosition + 4UL);
		}

		// Token: 0x04000013 RID: 19
		public const uint kTopValue = 16777216U;

		// Token: 0x04000014 RID: 20
		public ulong Low;

		// Token: 0x04000015 RID: 21
		public uint Range;

		// Token: 0x04000016 RID: 22
		private long StartPosition;

		// Token: 0x04000017 RID: 23
		private Stream Stream;

		// Token: 0x04000018 RID: 24
		private byte _cache;

		// Token: 0x04000019 RID: 25
		private uint _cacheSize;
	}
}
