using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000057 RID: 87
	internal class LineNumberAndPositionRecord : BamlRecord
	{
		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x0600024A RID: 586 RVA: 0x0000DC38 File Offset: 0x0000BE38
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LineNumberAndPosition;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x0600024B RID: 587 RVA: 0x0000DC4C File Offset: 0x0000BE4C
		// (set) Token: 0x0600024C RID: 588 RVA: 0x0000DC54 File Offset: 0x0000BE54
		public uint LineNumber { get; set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600024D RID: 589 RVA: 0x0000DC5D File Offset: 0x0000BE5D
		// (set) Token: 0x0600024E RID: 590 RVA: 0x0000DC65 File Offset: 0x0000BE65
		public uint LinePosition { get; set; }

		// Token: 0x0600024F RID: 591 RVA: 0x0000DC6E File Offset: 0x0000BE6E
		public override void Read(BamlBinaryReader reader)
		{
			this.LineNumber = reader.ReadUInt32();
			this.LinePosition = reader.ReadUInt32();
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000DC8B File Offset: 0x0000BE8B
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LineNumber);
			writer.Write(this.LinePosition);
		}
	}
}
