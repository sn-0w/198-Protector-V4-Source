using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002F RID: 47
	internal class DefAttributeRecord : SizedBamlRecord
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600014E RID: 334 RVA: 0x0000CBD8 File Offset: 0x0000ADD8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DefAttribute;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000CBEC File Offset: 0x0000ADEC
		// (set) Token: 0x06000150 RID: 336 RVA: 0x0000CBF4 File Offset: 0x0000ADF4
		public string Value { get; set; }

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000CBFD File Offset: 0x0000ADFD
		// (set) Token: 0x06000152 RID: 338 RVA: 0x0000CC05 File Offset: 0x0000AE05
		public ushort NameId { get; set; }

		// Token: 0x06000153 RID: 339 RVA: 0x0000CC0E File Offset: 0x0000AE0E
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.NameId = reader.ReadUInt16();
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000CC2B File Offset: 0x0000AE2B
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.NameId);
		}
	}
}
