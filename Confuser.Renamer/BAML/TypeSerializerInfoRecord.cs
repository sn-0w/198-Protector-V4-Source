using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000032 RID: 50
	internal class TypeSerializerInfoRecord : TypeInfoRecord
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600016F RID: 367 RVA: 0x0000CF7C File Offset: 0x0000B17C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TypeSerializerInfo;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000170 RID: 368 RVA: 0x0000CF90 File Offset: 0x0000B190
		// (set) Token: 0x06000171 RID: 369 RVA: 0x0000CF98 File Offset: 0x0000B198
		public ushort SerializerTypeId { get; set; }

		// Token: 0x06000172 RID: 370 RVA: 0x0000CFA1 File Offset: 0x0000B1A1
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.SerializerTypeId = reader.ReadUInt16();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000CFBA File Offset: 0x0000B1BA
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.SerializerTypeId);
		}
	}
}
