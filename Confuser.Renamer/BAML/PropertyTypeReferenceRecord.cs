using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000042 RID: 66
	internal class PropertyTypeReferenceRecord : PropertyComplexStartRecord
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060001DF RID: 479 RVA: 0x0000D574 File Offset: 0x0000B774
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyTypeReference;
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000D588 File Offset: 0x0000B788
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x0000D590 File Offset: 0x0000B790
		public ushort TypeId { get; set; }

		// Token: 0x060001E2 RID: 482 RVA: 0x0000D599 File Offset: 0x0000B799
		public override void Read(BamlBinaryReader reader)
		{
			base.Read(reader);
			this.TypeId = reader.ReadUInt16();
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000D5B1 File Offset: 0x0000B7B1
		public override void Write(BamlBinaryWriter writer)
		{
			base.Write(writer);
			writer.Write(this.TypeId);
		}
	}
}
