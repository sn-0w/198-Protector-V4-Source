using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000034 RID: 52
	internal class StringInfoRecord : SizedBamlRecord
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000D0A4 File Offset: 0x0000B2A4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StringInfo;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06000182 RID: 386 RVA: 0x0000D0B8 File Offset: 0x0000B2B8
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000D0C0 File Offset: 0x0000B2C0
		public ushort StringId { get; set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000D0C9 File Offset: 0x0000B2C9
		// (set) Token: 0x06000185 RID: 389 RVA: 0x0000D0D1 File Offset: 0x0000B2D1
		public string Value { get; set; }

		// Token: 0x06000186 RID: 390 RVA: 0x0000D0DA File Offset: 0x0000B2DA
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.StringId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000D0F7 File Offset: 0x0000B2F7
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.StringId);
			writer.Write(this.Value);
		}
	}
}
