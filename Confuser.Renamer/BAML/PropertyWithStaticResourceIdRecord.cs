using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000044 RID: 68
	internal class PropertyWithStaticResourceIdRecord : StaticResourceIdRecord
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001EB RID: 491 RVA: 0x0000D62C File Offset: 0x0000B82C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithStaticResourceId;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001EC RID: 492 RVA: 0x0000D640 File Offset: 0x0000B840
		// (set) Token: 0x060001ED RID: 493 RVA: 0x0000D648 File Offset: 0x0000B848
		public ushort AttributeId { get; set; }

		// Token: 0x060001EE RID: 494 RVA: 0x0000D651 File Offset: 0x0000B851
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
			base.Read(reader);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000D669 File Offset: 0x0000B869
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			base.Write(writer);
		}
	}
}
