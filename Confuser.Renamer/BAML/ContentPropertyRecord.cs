using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000045 RID: 69
	internal class ContentPropertyRecord : BamlRecord
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000D68C File Offset: 0x0000B88C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ContentProperty;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001F2 RID: 498 RVA: 0x0000D6A0 File Offset: 0x0000B8A0
		// (set) Token: 0x060001F3 RID: 499 RVA: 0x0000D6A8 File Offset: 0x0000B8A8
		public ushort AttributeId { get; set; }

		// Token: 0x060001F4 RID: 500 RVA: 0x0000D6B1 File Offset: 0x0000B8B1
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000D6C1 File Offset: 0x0000B8C1
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
		}
	}
}
