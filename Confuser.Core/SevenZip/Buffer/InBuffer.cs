using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x02000023 RID: 35
	internal class InBuffer
	{
		// Token: 0x060000C0 RID: 192 RVA: 0x000024BB File Offset: 0x000006BB
		public InBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000024D8 File Offset: 0x000006D8
		public void Init(Stream stream)
		{
			this.m_Stream = stream;
			this.m_ProcessedSize = 0UL;
			this.m_Limit = 0U;
			this.m_Pos = 0U;
			this.m_StreamWasExhausted = false;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000961C File Offset: 0x0000781C
		public bool ReadBlock()
		{
			bool streamWasExhausted = this.m_StreamWasExhausted;
			bool result;
			if (streamWasExhausted)
			{
				result = false;
			}
			else
			{
				this.m_ProcessedSize += (ulong)this.m_Pos;
				int num = this.m_Stream.Read(this.m_Buffer, 0, (int)this.m_BufferSize);
				this.m_Pos = 0U;
				this.m_Limit = (uint)num;
				this.m_StreamWasExhausted = (num == 0);
				result = !this.m_StreamWasExhausted;
			}
			return result;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000024FF File Offset: 0x000006FF
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000968C File Offset: 0x0000788C
		public bool ReadByte(byte b)
		{
			bool flag = this.m_Pos >= this.m_Limit;
			if (flag)
			{
				bool flag2 = !this.ReadBlock();
				if (flag2)
				{
					return false;
				}
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			b = buffer[(int)pos];
			return true;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000096E0 File Offset: 0x000078E0
		public byte ReadByte()
		{
			bool flag = this.m_Pos >= this.m_Limit;
			if (flag)
			{
				bool flag2 = !this.ReadBlock();
				if (flag2)
				{
					return byte.MaxValue;
				}
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			return buffer[(int)pos];
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00009734 File Offset: 0x00007934
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x040000E0 RID: 224
		private readonly byte[] m_Buffer;

		// Token: 0x040000E1 RID: 225
		private readonly uint m_BufferSize;

		// Token: 0x040000E2 RID: 226
		private uint m_Limit;

		// Token: 0x040000E3 RID: 227
		private uint m_Pos;

		// Token: 0x040000E4 RID: 228
		private ulong m_ProcessedSize;

		// Token: 0x040000E5 RID: 229
		private Stream m_Stream;

		// Token: 0x040000E6 RID: 230
		private bool m_StreamWasExhausted;
	}
}
