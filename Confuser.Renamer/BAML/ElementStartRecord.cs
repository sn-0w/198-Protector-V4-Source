using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003C RID: 60
	internal class ElementStartRecord : BamlRecord
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000D3D8 File Offset: 0x0000B5D8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ElementStart;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001BE RID: 446 RVA: 0x0000D3EB File Offset: 0x0000B5EB
		// (set) Token: 0x060001BF RID: 447 RVA: 0x0000D3F3 File Offset: 0x0000B5F3
		public ushort TypeId { get; set; }

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000D3FC File Offset: 0x0000B5FC
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x0000D404 File Offset: 0x0000B604
		public byte Flags { get; set; }

		// Token: 0x060001C2 RID: 450 RVA: 0x0000D40D File Offset: 0x0000B60D
		public override void Read(BamlBinaryReader reader)
		{
			this.TypeId = reader.ReadUInt16();
			this.Flags = reader.ReadByte();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000D42A File Offset: 0x0000B62A
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
			writer.Write(this.Flags);
		}
	}
}
