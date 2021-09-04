using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000043 RID: 67
	internal class PropertyStringReferenceRecord : PropertyComplexStartRecord
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x0000D5D4 File Offset: 0x0000B7D4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyStringReference;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000D5E8 File Offset: 0x0000B7E8
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x0000D5F0 File Offset: 0x0000B7F0
		public ushort StringId { get; set; }

		// Token: 0x060001E8 RID: 488 RVA: 0x0000D5F9 File Offset: 0x0000B7F9
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.StringId = reader.ReadUInt16();
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000D611 File Offset: 0x0000B811
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(this.StringId);
		}
	}
}
