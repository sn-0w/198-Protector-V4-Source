using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x02000024 RID: 36
	internal class OutBuffer
	{
		// Token: 0x060000C7 RID: 199 RVA: 0x00002509 File Offset: 0x00000709
		public OutBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002526 File Offset: 0x00000726
		public void SetStream(Stream stream)
		{
			this.m_Stream = stream;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00002530 File Offset: 0x00000730
		public void FlushStream()
		{
			this.m_Stream.Flush();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000253F File Offset: 0x0000073F
		public void CloseStream()
		{
			this.m_Stream.Close();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000254E File Offset: 0x0000074E
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00002558 File Offset: 0x00000758
		public void Init()
		{
			this.m_ProcessedSize = 0UL;
			this.m_Pos = 0U;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009754 File Offset: 0x00007954
		public void WriteByte(byte b)
		{
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			buffer[(int)pos] = b;
			bool flag = this.m_Pos >= this.m_BufferSize;
			if (flag)
			{
				this.FlushData();
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009798 File Offset: 0x00007998
		public void FlushData()
		{
			bool flag = this.m_Pos == 0U;
			if (!flag)
			{
				this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
				this.m_Pos = 0U;
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000097D8 File Offset: 0x000079D8
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x040000E7 RID: 231
		private readonly byte[] m_Buffer;

		// Token: 0x040000E8 RID: 232
		private readonly uint m_BufferSize;

		// Token: 0x040000E9 RID: 233
		private uint m_Pos;

		// Token: 0x040000EA RID: 234
		private ulong m_ProcessedSize;

		// Token: 0x040000EB RID: 235
		private Stream m_Stream;
	}
}
