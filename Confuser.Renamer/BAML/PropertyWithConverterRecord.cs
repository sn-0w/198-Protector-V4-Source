using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002D RID: 45
	internal class PropertyWithConverterRecord : PropertyRecord
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600013E RID: 318 RVA: 0x0000CAB4 File Offset: 0x0000ACB4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithConverter;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000CAC8 File Offset: 0x0000ACC8
		// (set) Token: 0x06000140 RID: 320 RVA: 0x0000CAD0 File Offset: 0x0000ACD0
		public ushort ConverterTypeId { get; set; }

		// Token: 0x06000141 RID: 321 RVA: 0x0000CAD9 File Offset: 0x0000ACD9
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.ConverterTypeId = reader.ReadUInt16();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000CAF2 File Offset: 0x0000ACF2
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.ConverterTypeId);
		}
	}
}
