using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000035 RID: 53
	internal class TextRecord : SizedBamlRecord
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000D114 File Offset: 0x0000B314
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.Text;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0000D128 File Offset: 0x0000B328
		// (set) Token: 0x0600018B RID: 395 RVA: 0x0000D130 File Offset: 0x0000B330
		public string Value { get; set; }

		// Token: 0x0600018C RID: 396 RVA: 0x0000D139 File Offset: 0x0000B339
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Value = reader.ReadString();
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000D149 File Offset: 0x0000B349
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Value);
		}
	}
}
