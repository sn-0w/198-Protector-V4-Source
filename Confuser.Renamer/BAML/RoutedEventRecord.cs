using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000039 RID: 57
	internal class RoutedEventRecord : SizedBamlRecord
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000D2A0 File Offset: 0x0000B4A0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.RoutedEvent;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x0000D2BC File Offset: 0x0000B4BC
		public string Value { get; set; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x0000D2C5 File Offset: 0x0000B4C5
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x0000D2CD File Offset: 0x0000B4CD
		public ushort AttributeId { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000D2D6 File Offset: 0x0000B4D6
		// (set) Token: 0x060001AB RID: 427 RVA: 0x0000D2DE File Offset: 0x0000B4DE
		public uint Reserved1 { get; set; }

		// Token: 0x060001AC RID: 428 RVA: 0x0000D2E7 File Offset: 0x0000B4E7
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000D304 File Offset: 0x0000B504
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.AttributeId);
		}
	}
}
