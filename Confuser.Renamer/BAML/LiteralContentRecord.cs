using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000038 RID: 56
	internal class LiteralContentRecord : SizedBamlRecord
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000D204 File Offset: 0x0000B404
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LiteralContent;
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000D218 File Offset: 0x0000B418
		// (set) Token: 0x0600019D RID: 413 RVA: 0x0000D220 File Offset: 0x0000B420
		public string Value { get; set; }

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600019E RID: 414 RVA: 0x0000D229 File Offset: 0x0000B429
		// (set) Token: 0x0600019F RID: 415 RVA: 0x0000D231 File Offset: 0x0000B431
		public uint Reserved0 { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001A0 RID: 416 RVA: 0x0000D23A File Offset: 0x0000B43A
		// (set) Token: 0x060001A1 RID: 417 RVA: 0x0000D242 File Offset: 0x0000B442
		public uint Reserved1 { get; set; }

		// Token: 0x060001A2 RID: 418 RVA: 0x0000D24B File Offset: 0x0000B44B
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.Reserved0 = reader.ReadUInt32();
			this.Reserved1 = reader.ReadUInt32();
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000D275 File Offset: 0x0000B475
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.Reserved0);
			writer.Write(this.Reserved1);
		}
	}
}
