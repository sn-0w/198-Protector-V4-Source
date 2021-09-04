using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000041 RID: 65
	internal class PropertyWithExtensionRecord : BamlRecord
	{
		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x0000D4D8 File Offset: 0x0000B6D8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyWithExtension;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000D4EC File Offset: 0x0000B6EC
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000D4F4 File Offset: 0x0000B6F4
		public ushort AttributeId { get; set; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000D4FD File Offset: 0x0000B6FD
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000D505 File Offset: 0x0000B705
		public ushort Flags { get; set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060001DA RID: 474 RVA: 0x0000D50E File Offset: 0x0000B70E
		// (set) Token: 0x060001DB RID: 475 RVA: 0x0000D516 File Offset: 0x0000B716
		public ushort ValueId { get; set; }

		// Token: 0x060001DC RID: 476 RVA: 0x0000D51F File Offset: 0x0000B71F
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Flags = reader.ReadUInt16();
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000D549 File Offset: 0x0000B749
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.Flags);
			writer.Write(this.ValueId);
		}
	}
}
