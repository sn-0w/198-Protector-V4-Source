using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000029 RID: 41
	internal class PresentationOptionsAttributeRecord : SizedBamlRecord
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600011C RID: 284 RVA: 0x0000C8C8 File Offset: 0x0000AAC8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PresentationOptionsAttribute;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0000C8DC File Offset: 0x0000AADC
		// (set) Token: 0x0600011E RID: 286 RVA: 0x0000C8E4 File Offset: 0x0000AAE4
		public string Value { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600011F RID: 287 RVA: 0x0000C8ED File Offset: 0x0000AAED
		// (set) Token: 0x06000120 RID: 288 RVA: 0x0000C8F5 File Offset: 0x0000AAF5
		public ushort NameId { get; set; }

		// Token: 0x06000121 RID: 289 RVA: 0x0000C8FE File Offset: 0x0000AAFE
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
			this.NameId = reader.ReadUInt16();
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000C91B File Offset: 0x0000AB1B
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
			writer.Write(this.NameId);
		}
	}
}
