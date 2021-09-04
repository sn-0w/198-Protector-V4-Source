using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000033 RID: 51
	internal class AttributeInfoRecord : SizedBamlRecord
	{
		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000175 RID: 373 RVA: 0x0000CFDC File Offset: 0x0000B1DC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.AttributeInfo;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000176 RID: 374 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		// (set) Token: 0x06000177 RID: 375 RVA: 0x0000CFF8 File Offset: 0x0000B1F8
		public ushort AttributeId { get; set; }

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000D001 File Offset: 0x0000B201
		// (set) Token: 0x06000179 RID: 377 RVA: 0x0000D009 File Offset: 0x0000B209
		public ushort OwnerTypeId { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600017A RID: 378 RVA: 0x0000D012 File Offset: 0x0000B212
		// (set) Token: 0x0600017B RID: 379 RVA: 0x0000D01A File Offset: 0x0000B21A
		public byte AttributeUsage { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600017C RID: 380 RVA: 0x0000D023 File Offset: 0x0000B223
		// (set) Token: 0x0600017D RID: 381 RVA: 0x0000D02B File Offset: 0x0000B22B
		public string Name { get; set; }

		// Token: 0x0600017E RID: 382 RVA: 0x0000D034 File Offset: 0x0000B234
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.OwnerTypeId = reader.ReadUInt16();
			this.AttributeUsage = reader.ReadByte();
			this.Name = reader.ReadString();
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000D06B File Offset: 0x0000B26B
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.OwnerTypeId);
			writer.Write(this.AttributeUsage);
			writer.Write(this.Name);
		}
	}
}
