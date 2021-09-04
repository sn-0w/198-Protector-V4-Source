using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000052 RID: 82
	internal class DeferableContentStartRecord : BamlRecord, IBamlDeferRecord
	{
		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600022E RID: 558 RVA: 0x0000DA88 File Offset: 0x0000BC88
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DeferableContentStart;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600022F RID: 559 RVA: 0x0000DA9C File Offset: 0x0000BC9C
		// (set) Token: 0x06000230 RID: 560 RVA: 0x0000DAA4 File Offset: 0x0000BCA4
		public BamlRecord Record { get; set; }

		// Token: 0x06000231 RID: 561 RVA: 0x0000DAAD File Offset: 0x0000BCAD
		public void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve)
		{
			this.Record = resolve(this.pos + (long)((ulong)this.size));
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000DACB File Offset: 0x0000BCCB
		public void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr)
		{
			wtr.BaseStream.Seek(this.pos, SeekOrigin.Begin);
			wtr.Write((uint)(this.Record.Position - (this.pos + 4L)));
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000DAFE File Offset: 0x0000BCFE
		public override void Read(BamlBinaryReader reader)
		{
			this.size = reader.ReadUInt32();
			this.pos = reader.BaseStream.Position;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000DB1E File Offset: 0x0000BD1E
		public override void Write(BamlBinaryWriter writer)
		{
			this.pos = writer.BaseStream.Position;
			writer.Write(0U);
		}

		// Token: 0x040000F6 RID: 246
		private long pos;

		// Token: 0x040000F7 RID: 247
		internal uint size = uint.MaxValue;
	}
}
