using System;
using System.IO;

namespace Confuser.Runtime
{
	// Token: 0x0200002A RID: 42
	internal static class Lzma
	{
		// Token: 0x06000091 RID: 145 RVA: 0x0000603C File Offset: 0x0000423C
		public static byte[] Decompress(byte[] data)
		{
			MemoryStream memoryStream = new MemoryStream(data);
			Lzma.LzmaDecoder lzmaDecoder = new Lzma.LzmaDecoder();
			byte[] array = new byte[5];
			memoryStream.Read(array, 0, 5);
			lzmaDecoder.SetDecoderProperties(array);
			long num = 0L;
			for (int i = 0; i < 8; i++)
			{
				int num2 = memoryStream.ReadByte();
				num |= (long)((long)((ulong)((byte)num2)) << 8 * i);
			}
			byte[] array2 = new byte[(int)num];
			MemoryStream outStream = new MemoryStream(array2, true);
			long inSize = memoryStream.Length - 13L;
			lzmaDecoder.Code(memoryStream, outStream, inSize, num);
			return array2;
		}

		// Token: 0x0400007B RID: 123
		private const uint kNumStates = 12U;

		// Token: 0x0400007C RID: 124
		private const int kNumPosSlotBits = 6;

		// Token: 0x0400007D RID: 125
		private const uint kNumLenToPosStates = 4U;

		// Token: 0x0400007E RID: 126
		private const uint kMatchMinLen = 2U;

		// Token: 0x0400007F RID: 127
		private const int kNumAlignBits = 4;

		// Token: 0x04000080 RID: 128
		private const uint kAlignTableSize = 16U;

		// Token: 0x04000081 RID: 129
		private const uint kStartPosModelIndex = 4U;

		// Token: 0x04000082 RID: 130
		private const uint kEndPosModelIndex = 14U;

		// Token: 0x04000083 RID: 131
		private const uint kNumFullDistances = 128U;

		// Token: 0x04000084 RID: 132
		private const int kNumPosStatesBitsMax = 4;

		// Token: 0x04000085 RID: 133
		private const uint kNumPosStatesMax = 16U;

		// Token: 0x04000086 RID: 134
		private const int kNumLowLenBits = 3;

		// Token: 0x04000087 RID: 135
		private const int kNumMidLenBits = 3;

		// Token: 0x04000088 RID: 136
		private const int kNumHighLenBits = 8;

		// Token: 0x04000089 RID: 137
		private const uint kNumLowLenSymbols = 8U;

		// Token: 0x0400008A RID: 138
		private const uint kNumMidLenSymbols = 8U;

		// Token: 0x0200002B RID: 43
		private struct BitDecoder
		{
			// Token: 0x06000092 RID: 146 RVA: 0x0000245D File Offset: 0x0000065D
			public void Init()
			{
				this.Prob = 1024U;
			}

			// Token: 0x06000093 RID: 147 RVA: 0x000060C8 File Offset: 0x000042C8
			public uint Decode(Lzma.Decoder rangeDecoder)
			{
				uint num = (rangeDecoder.Range >> 11) * this.Prob;
				if (rangeDecoder.Code < num)
				{
					rangeDecoder.Range = num;
					this.Prob += 2048U - this.Prob >> 5;
					if (rangeDecoder.Range < 16777216U)
					{
						rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
						rangeDecoder.Range <<= 8;
					}
					return 0U;
				}
				rangeDecoder.Range -= num;
				rangeDecoder.Code -= num;
				this.Prob -= this.Prob >> 5;
				if (rangeDecoder.Range < 16777216U)
				{
					rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
					rangeDecoder.Range <<= 8;
				}
				return 1U;
			}

			// Token: 0x0400008B RID: 139
			public const int kNumBitModelTotalBits = 11;

			// Token: 0x0400008C RID: 140
			public const uint kBitModelTotal = 2048U;

			// Token: 0x0400008D RID: 141
			private const int kNumMoveBits = 5;

			// Token: 0x0400008E RID: 142
			private uint Prob;
		}

		// Token: 0x0200002C RID: 44
		private struct BitTreeDecoder
		{
			// Token: 0x06000094 RID: 148 RVA: 0x0000246A File Offset: 0x0000066A
			public BitTreeDecoder(int numBitLevels)
			{
				this.NumBitLevels = numBitLevels;
				this.Models = new Lzma.BitDecoder[1 << numBitLevels];
			}

			// Token: 0x06000095 RID: 149 RVA: 0x000061B4 File Offset: 0x000043B4
			public void Init()
			{
				uint num = 1U;
				while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
				{
					this.Models[(int)num].Init();
					num += 1U;
				}
			}

			// Token: 0x06000096 RID: 150 RVA: 0x000061EC File Offset: 0x000043EC
			public uint Decode(Lzma.Decoder rangeDecoder)
			{
				uint num = 1U;
				for (int i = this.NumBitLevels; i > 0; i--)
				{
					num = (num << 1) + this.Models[(int)num].Decode(rangeDecoder);
				}
				return num - (1U << this.NumBitLevels);
			}

			// Token: 0x06000097 RID: 151 RVA: 0x00006230 File Offset: 0x00004430
			public uint ReverseDecode(Lzma.Decoder rangeDecoder)
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

			// Token: 0x06000098 RID: 152 RVA: 0x00006278 File Offset: 0x00004478
			public static uint ReverseDecode(Lzma.BitDecoder[] Models, uint startIndex, Lzma.Decoder rangeDecoder, int NumBitLevels)
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

			// Token: 0x0400008F RID: 143
			private readonly Lzma.BitDecoder[] Models;

			// Token: 0x04000090 RID: 144
			private readonly int NumBitLevels;
		}

		// Token: 0x0200002D RID: 45
		private class Decoder
		{
			// Token: 0x06000099 RID: 153 RVA: 0x000062B8 File Offset: 0x000044B8
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

			// Token: 0x0600009A RID: 154 RVA: 0x00002484 File Offset: 0x00000684
			public void ReleaseStream()
			{
				this.Stream = null;
			}

			// Token: 0x0600009B RID: 155 RVA: 0x0000248D File Offset: 0x0000068D
			public void Normalize()
			{
				while (this.Range < 16777216U)
				{
					this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
					this.Range <<= 8;
				}
			}

			// Token: 0x0600009C RID: 156 RVA: 0x00006304 File Offset: 0x00004504
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
					if (num < 16777216U)
					{
						num2 = (num2 << 8 | (uint)((byte)this.Stream.ReadByte()));
						num <<= 8;
					}
				}
				this.Range = num;
				this.Code = num2;
				return num3;
			}

			// Token: 0x04000091 RID: 145
			public const uint kTopValue = 16777216U;

			// Token: 0x04000092 RID: 146
			public uint Code;

			// Token: 0x04000093 RID: 147
			public uint Range;

			// Token: 0x04000094 RID: 148
			public Stream Stream;
		}

		// Token: 0x0200002E RID: 46
		private class LzmaDecoder
		{
			// Token: 0x0600009E RID: 158 RVA: 0x00006378 File Offset: 0x00004578
			public LzmaDecoder()
			{
				this.m_DictionarySize = uint.MaxValue;
				int num = 0;
				while ((long)num < 4L)
				{
					this.m_PosSlotDecoder[num] = new Lzma.BitTreeDecoder(6);
					num++;
				}
			}

			// Token: 0x0600009F RID: 159 RVA: 0x00006464 File Offset: 0x00004664
			private void SetDictionarySize(uint dictionarySize)
			{
				if (this.m_DictionarySize != dictionarySize)
				{
					this.m_DictionarySize = dictionarySize;
					this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1U);
					uint windowSize = Math.Max(this.m_DictionarySizeCheck, 4096U);
					this.m_OutWindow.Create(windowSize);
				}
			}

			// Token: 0x060000A0 RID: 160 RVA: 0x000024C7 File Offset: 0x000006C7
			private void SetLiteralProperties(int lp, int lc)
			{
				this.m_LiteralDecoder.Create(lp, lc);
			}

			// Token: 0x060000A1 RID: 161 RVA: 0x000064B0 File Offset: 0x000046B0
			private void SetPosBitsProperties(int pb)
			{
				uint num = 1U << pb;
				this.m_LenDecoder.Create(num);
				this.m_RepLenDecoder.Create(num);
				this.m_PosStateMask = num - 1U;
			}

			// Token: 0x060000A2 RID: 162 RVA: 0x000064E8 File Offset: 0x000046E8
			private void Init(Stream inStream, Stream outStream)
			{
				this.m_RangeDecoder.Init(inStream);
				this.m_OutWindow.Init(outStream, this._solid);
				for (uint num = 0U; num < 12U; num += 1U)
				{
					for (uint num2 = 0U; num2 <= this.m_PosStateMask; num2 += 1U)
					{
						uint num3 = (num << 4) + num2;
						this.m_IsMatchDecoders[(int)num3].Init();
						this.m_IsRep0LongDecoders[(int)num3].Init();
					}
					this.m_IsRepDecoders[(int)num].Init();
					this.m_IsRepG0Decoders[(int)num].Init();
					this.m_IsRepG1Decoders[(int)num].Init();
					this.m_IsRepG2Decoders[(int)num].Init();
				}
				this.m_LiteralDecoder.Init();
				for (uint num = 0U; num < 4U; num += 1U)
				{
					this.m_PosSlotDecoder[(int)num].Init();
				}
				for (uint num = 0U; num < 114U; num += 1U)
				{
					this.m_PosDecoders[(int)num].Init();
				}
				this.m_LenDecoder.Init();
				this.m_RepLenDecoder.Init();
				this.m_PosAlignDecoder.Init();
			}

			// Token: 0x060000A3 RID: 163 RVA: 0x0000660C File Offset: 0x0000480C
			public void Code(Stream inStream, Stream outStream, long inSize, long outSize)
			{
				this.Init(inStream, outStream);
				Lzma.State state = default(Lzma.State);
				state.Init();
				uint num = 0U;
				uint num2 = 0U;
				uint num3 = 0U;
				uint num4 = 0U;
				ulong num5 = 0UL;
				if (num5 < (ulong)outSize)
				{
					this.m_IsMatchDecoders[(int)((int)state.Index << 4)].Decode(this.m_RangeDecoder);
					state.UpdateChar();
					byte b = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0U, 0);
					this.m_OutWindow.PutByte(b);
					num5 += 1UL;
				}
				while (num5 < (ulong)outSize)
				{
					uint num6 = (uint)num5 & this.m_PosStateMask;
					if (this.m_IsMatchDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
					{
						byte @byte = this.m_OutWindow.GetByte(0U);
						byte b2;
						if (!state.IsCharState())
						{
							b2 = this.m_LiteralDecoder.DecodeWithMatchByte(this.m_RangeDecoder, (uint)num5, @byte, this.m_OutWindow.GetByte(num));
						}
						else
						{
							b2 = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, (uint)num5, @byte);
						}
						this.m_OutWindow.PutByte(b2);
						state.UpdateChar();
						num5 += 1UL;
					}
					else
					{
						uint num8;
						if (this.m_IsRepDecoders[(int)state.Index].Decode(this.m_RangeDecoder) == 1U)
						{
							if (this.m_IsRepG0Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
							{
								if (this.m_IsRep0LongDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
								{
									state.UpdateShortRep();
									this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(num));
									num5 += 1UL;
									continue;
								}
							}
							else
							{
								uint num7;
								if (this.m_IsRepG1Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
								{
									num7 = num2;
								}
								else
								{
									if (this.m_IsRepG2Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
									{
										num7 = num3;
									}
									else
									{
										num7 = num4;
										num4 = num3;
									}
									num3 = num2;
								}
								num2 = num;
								num = num7;
							}
							num8 = this.m_RepLenDecoder.Decode(this.m_RangeDecoder, num6) + 2U;
							state.UpdateRep();
						}
						else
						{
							num4 = num3;
							num3 = num2;
							num2 = num;
							num8 = 2U + this.m_LenDecoder.Decode(this.m_RangeDecoder, num6);
							state.UpdateMatch();
							uint num9 = this.m_PosSlotDecoder[(int)Lzma.LzmaDecoder.GetLenToPosState(num8)].Decode(this.m_RangeDecoder);
							if (num9 >= 4U)
							{
								int num10 = (int)((num9 >> 1) - 1U);
								num = (2U | (num9 & 1U)) << num10;
								if (num9 < 14U)
								{
									num += Lzma.BitTreeDecoder.ReverseDecode(this.m_PosDecoders, num - num9 - 1U, this.m_RangeDecoder, num10);
								}
								else
								{
									num += this.m_RangeDecoder.DecodeDirectBits(num10 - 4) << 4;
									num += this.m_PosAlignDecoder.ReverseDecode(this.m_RangeDecoder);
								}
							}
							else
							{
								num = num9;
							}
						}
						if (((ulong)num >= num5 || num >= this.m_DictionarySizeCheck) && num == 4294967295U)
						{
							break;
						}
						this.m_OutWindow.CopyBlock(num, num8);
						num5 += (ulong)num8;
					}
				}
				this.m_OutWindow.Flush();
				this.m_OutWindow.ReleaseStream();
				this.m_RangeDecoder.ReleaseStream();
			}

			// Token: 0x060000A4 RID: 164 RVA: 0x00006948 File Offset: 0x00004B48
			public void SetDecoderProperties(byte[] properties)
			{
				int lc = (int)(properties[0] % 9);
				int num = (int)(properties[0] / 9);
				int lp = num % 5;
				int posBitsProperties = num / 5;
				uint num2 = 0U;
				for (int i = 0; i < 4; i++)
				{
					num2 += (uint)((uint)properties[1 + i] << i * 8);
				}
				this.SetDictionarySize(num2);
				this.SetLiteralProperties(lp, lc);
				this.SetPosBitsProperties(posBitsProperties);
			}

			// Token: 0x060000A5 RID: 165 RVA: 0x000024D6 File Offset: 0x000006D6
			private static uint GetLenToPosState(uint len)
			{
				len -= 2U;
				if (len < 4U)
				{
					return len;
				}
				return 3U;
			}

			// Token: 0x04000095 RID: 149
			private readonly Lzma.BitDecoder[] m_IsMatchDecoders = new Lzma.BitDecoder[192];

			// Token: 0x04000096 RID: 150
			private readonly Lzma.BitDecoder[] m_IsRep0LongDecoders = new Lzma.BitDecoder[192];

			// Token: 0x04000097 RID: 151
			private readonly Lzma.BitDecoder[] m_IsRepDecoders = new Lzma.BitDecoder[12];

			// Token: 0x04000098 RID: 152
			private readonly Lzma.BitDecoder[] m_IsRepG0Decoders = new Lzma.BitDecoder[12];

			// Token: 0x04000099 RID: 153
			private readonly Lzma.BitDecoder[] m_IsRepG1Decoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009A RID: 154
			private readonly Lzma.BitDecoder[] m_IsRepG2Decoders = new Lzma.BitDecoder[12];

			// Token: 0x0400009B RID: 155
			private readonly Lzma.LzmaDecoder.LenDecoder m_LenDecoder = new Lzma.LzmaDecoder.LenDecoder();

			// Token: 0x0400009C RID: 156
			private readonly Lzma.LzmaDecoder.LiteralDecoder m_LiteralDecoder = new Lzma.LzmaDecoder.LiteralDecoder();

			// Token: 0x0400009D RID: 157
			private readonly Lzma.OutWindow m_OutWindow = new Lzma.OutWindow();

			// Token: 0x0400009E RID: 158
			private readonly Lzma.BitDecoder[] m_PosDecoders = new Lzma.BitDecoder[114];

			// Token: 0x0400009F RID: 159
			private readonly Lzma.BitTreeDecoder[] m_PosSlotDecoder = new Lzma.BitTreeDecoder[4];

			// Token: 0x040000A0 RID: 160
			private readonly Lzma.Decoder m_RangeDecoder = new Lzma.Decoder();

			// Token: 0x040000A1 RID: 161
			private readonly Lzma.LzmaDecoder.LenDecoder m_RepLenDecoder = new Lzma.LzmaDecoder.LenDecoder();

			// Token: 0x040000A2 RID: 162
			private bool _solid;

			// Token: 0x040000A3 RID: 163
			private uint m_DictionarySize;

			// Token: 0x040000A4 RID: 164
			private uint m_DictionarySizeCheck;

			// Token: 0x040000A5 RID: 165
			private Lzma.BitTreeDecoder m_PosAlignDecoder = new Lzma.BitTreeDecoder(4);

			// Token: 0x040000A6 RID: 166
			private uint m_PosStateMask;

			// Token: 0x0200002F RID: 47
			private class LenDecoder
			{
				// Token: 0x060000A6 RID: 166 RVA: 0x000069A8 File Offset: 0x00004BA8
				public void Create(uint numPosStates)
				{
					for (uint num = this.m_NumPosStates; num < numPosStates; num += 1U)
					{
						this.m_LowCoder[(int)num] = new Lzma.BitTreeDecoder(3);
						this.m_MidCoder[(int)num] = new Lzma.BitTreeDecoder(3);
					}
					this.m_NumPosStates = numPosStates;
				}

				// Token: 0x060000A7 RID: 167 RVA: 0x000069F4 File Offset: 0x00004BF4
				public void Init()
				{
					this.m_Choice.Init();
					for (uint num = 0U; num < this.m_NumPosStates; num += 1U)
					{
						this.m_LowCoder[(int)num].Init();
						this.m_MidCoder[(int)num].Init();
					}
					this.m_Choice2.Init();
					this.m_HighCoder.Init();
				}

				// Token: 0x060000A8 RID: 168 RVA: 0x00006A58 File Offset: 0x00004C58
				public uint Decode(Lzma.Decoder rangeDecoder, uint posState)
				{
					if (this.m_Choice.Decode(rangeDecoder) == 0U)
					{
						return this.m_LowCoder[(int)posState].Decode(rangeDecoder);
					}
					uint num = 8U;
					if (this.m_Choice2.Decode(rangeDecoder) == 0U)
					{
						num += this.m_MidCoder[(int)posState].Decode(rangeDecoder);
					}
					else
					{
						num += 8U;
						num += this.m_HighCoder.Decode(rangeDecoder);
					}
					return num;
				}

				// Token: 0x040000A7 RID: 167
				private readonly Lzma.BitTreeDecoder[] m_LowCoder = new Lzma.BitTreeDecoder[16];

				// Token: 0x040000A8 RID: 168
				private readonly Lzma.BitTreeDecoder[] m_MidCoder = new Lzma.BitTreeDecoder[16];

				// Token: 0x040000A9 RID: 169
				private Lzma.BitDecoder m_Choice;

				// Token: 0x040000AA RID: 170
				private Lzma.BitDecoder m_Choice2;

				// Token: 0x040000AB RID: 171
				private Lzma.BitTreeDecoder m_HighCoder = new Lzma.BitTreeDecoder(8);

				// Token: 0x040000AC RID: 172
				private uint m_NumPosStates;
			}

			// Token: 0x02000030 RID: 48
			private class LiteralDecoder
			{
				// Token: 0x060000AA RID: 170 RVA: 0x00006AC4 File Offset: 0x00004CC4
				public void Create(int numPosBits, int numPrevBits)
				{
					if (this.m_Coders != null && this.m_NumPrevBits == numPrevBits && this.m_NumPosBits == numPosBits)
					{
						return;
					}
					this.m_NumPosBits = numPosBits;
					this.m_PosMask = (1U << numPosBits) - 1U;
					this.m_NumPrevBits = numPrevBits;
					uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					this.m_Coders = new Lzma.LzmaDecoder.LiteralDecoder.Decoder2[num];
					for (uint num2 = 0U; num2 < num; num2 += 1U)
					{
						this.m_Coders[(int)num2].Create();
					}
				}

				// Token: 0x060000AB RID: 171 RVA: 0x00006B44 File Offset: 0x00004D44
				public void Init()
				{
					uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
					for (uint num2 = 0U; num2 < num; num2 += 1U)
					{
						this.m_Coders[(int)num2].Init();
					}
				}

				// Token: 0x060000AC RID: 172 RVA: 0x00002512 File Offset: 0x00000712
				private uint GetState(uint pos, byte prevByte)
				{
					return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
				}

				// Token: 0x060000AD RID: 173 RVA: 0x00002534 File Offset: 0x00000734
				public byte DecodeNormal(Lzma.Decoder rangeDecoder, uint pos, byte prevByte)
				{
					return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeNormal(rangeDecoder);
				}

				// Token: 0x060000AE RID: 174 RVA: 0x0000254F File Offset: 0x0000074F
				public byte DecodeWithMatchByte(Lzma.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
				{
					return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);
				}

				// Token: 0x040000AD RID: 173
				private Lzma.LzmaDecoder.LiteralDecoder.Decoder2[] m_Coders;

				// Token: 0x040000AE RID: 174
				private int m_NumPosBits;

				// Token: 0x040000AF RID: 175
				private int m_NumPrevBits;

				// Token: 0x040000B0 RID: 176
				private uint m_PosMask;

				// Token: 0x02000031 RID: 49
				private struct Decoder2
				{
					// Token: 0x060000B0 RID: 176 RVA: 0x0000256C File Offset: 0x0000076C
					public void Create()
					{
						this.m_Decoders = new Lzma.BitDecoder[768];
					}

					// Token: 0x060000B1 RID: 177 RVA: 0x00006B84 File Offset: 0x00004D84
					public void Init()
					{
						for (int i = 0; i < 768; i++)
						{
							this.m_Decoders[i].Init();
						}
					}

					// Token: 0x060000B2 RID: 178 RVA: 0x00006BB4 File Offset: 0x00004DB4
					public byte DecodeNormal(Lzma.Decoder rangeDecoder)
					{
						uint num = 1U;
						do
						{
							num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
						}
						while (num < 256U);
						return (byte)num;
					}

					// Token: 0x060000B3 RID: 179 RVA: 0x00006BE4 File Offset: 0x00004DE4
					public byte DecodeWithMatchByte(Lzma.Decoder rangeDecoder, byte matchByte)
					{
						uint num = 1U;
						for (;;)
						{
							uint num2 = (uint)(matchByte >> 7 & 1);
							matchByte = (byte)(matchByte << 1);
							uint num3 = this.m_Decoders[(int)((1U + num2 << 8) + num)].Decode(rangeDecoder);
							num = (num << 1 | num3);
							if (num2 != num3)
							{
								break;
							}
							if (num >= 256U)
							{
								goto IL_5C;
							}
						}
						while (num < 256U)
						{
							num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
						}
						IL_5C:
						return (byte)num;
					}

					// Token: 0x040000B1 RID: 177
					private Lzma.BitDecoder[] m_Decoders;
				}
			}
		}

		// Token: 0x02000032 RID: 50
		private class OutWindow
		{
			// Token: 0x060000B4 RID: 180 RVA: 0x0000257E File Offset: 0x0000077E
			public void Create(uint windowSize)
			{
				if (this._windowSize != windowSize)
				{
					this._buffer = new byte[windowSize];
				}
				this._windowSize = windowSize;
				this._pos = 0U;
				this._streamPos = 0U;
			}

			// Token: 0x060000B5 RID: 181 RVA: 0x000025AA File Offset: 0x000007AA
			public void Init(Stream stream, bool solid)
			{
				this.ReleaseStream();
				this._stream = stream;
				if (!solid)
				{
					this._streamPos = 0U;
					this._pos = 0U;
				}
			}

			// Token: 0x060000B6 RID: 182 RVA: 0x000025CA File Offset: 0x000007CA
			public void ReleaseStream()
			{
				this.Flush();
				this._stream = null;
				Buffer.BlockCopy(new byte[this._buffer.Length], 0, this._buffer, 0, this._buffer.Length);
			}

			// Token: 0x060000B7 RID: 183 RVA: 0x00006C50 File Offset: 0x00004E50
			public void Flush()
			{
				uint num = this._pos - this._streamPos;
				if (num == 0U)
				{
					return;
				}
				this._stream.Write(this._buffer, (int)this._streamPos, (int)num);
				if (this._pos >= this._windowSize)
				{
					this._pos = 0U;
				}
				this._streamPos = this._pos;
			}

			// Token: 0x060000B8 RID: 184 RVA: 0x00006CA8 File Offset: 0x00004EA8
			public void CopyBlock(uint distance, uint len)
			{
				uint num = this._pos - distance - 1U;
				if (num >= this._windowSize)
				{
					num += this._windowSize;
				}
				while (len > 0U)
				{
					if (num >= this._windowSize)
					{
						num = 0U;
					}
					byte[] buffer = this._buffer;
					uint pos = this._pos;
					this._pos = pos + 1U;
					buffer[(int)pos] = this._buffer[(int)num++];
					if (this._pos >= this._windowSize)
					{
						this.Flush();
					}
					len -= 1U;
				}
			}

			// Token: 0x060000B9 RID: 185 RVA: 0x00006D20 File Offset: 0x00004F20
			public void PutByte(byte b)
			{
				byte[] buffer = this._buffer;
				uint pos = this._pos;
				this._pos = pos + 1U;
				buffer[(int)pos] = b;
				if (this._pos >= this._windowSize)
				{
					this.Flush();
				}
			}

			// Token: 0x060000BA RID: 186 RVA: 0x00006D5C File Offset: 0x00004F5C
			public byte GetByte(uint distance)
			{
				uint num = this._pos - distance - 1U;
				if (num >= this._windowSize)
				{
					num += this._windowSize;
				}
				return this._buffer[(int)num];
			}

			// Token: 0x040000B2 RID: 178
			private byte[] _buffer;

			// Token: 0x040000B3 RID: 179
			private uint _pos;

			// Token: 0x040000B4 RID: 180
			private Stream _stream;

			// Token: 0x040000B5 RID: 181
			private uint _streamPos;

			// Token: 0x040000B6 RID: 182
			private uint _windowSize;
		}

		// Token: 0x02000033 RID: 51
		private struct State
		{
			// Token: 0x060000BC RID: 188 RVA: 0x000025FB File Offset: 0x000007FB
			public void Init()
			{
				this.Index = 0U;
			}

			// Token: 0x060000BD RID: 189 RVA: 0x00002604 File Offset: 0x00000804
			public void UpdateChar()
			{
				if (this.Index < 4U)
				{
					this.Index = 0U;
					return;
				}
				if (this.Index < 10U)
				{
					this.Index -= 3U;
					return;
				}
				this.Index -= 6U;
			}

			// Token: 0x060000BE RID: 190 RVA: 0x0000263E File Offset: 0x0000083E
			public void UpdateMatch()
			{
				this.Index = ((this.Index < 7U) ? 7U : 10U);
			}

			// Token: 0x060000BF RID: 191 RVA: 0x00002654 File Offset: 0x00000854
			public void UpdateRep()
			{
				this.Index = ((this.Index < 7U) ? 8U : 11U);
			}

			// Token: 0x060000C0 RID: 192 RVA: 0x0000266A File Offset: 0x0000086A
			public void UpdateShortRep()
			{
				this.Index = ((this.Index < 7U) ? 9U : 11U);
			}

			// Token: 0x060000C1 RID: 193 RVA: 0x00002681 File Offset: 0x00000881
			public bool IsCharState()
			{
				return this.Index < 7U;
			}

			// Token: 0x040000B7 RID: 183
			public uint Index;
		}
	}
}
