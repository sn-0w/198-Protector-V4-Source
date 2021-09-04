using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000040 RID: 64
	internal class ConnectionIdRecord : BamlRecord
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000D490 File Offset: 0x0000B690
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConnectionId;
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000D4A4 File Offset: 0x0000B6A4
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x0000D4AC File Offset: 0x0000B6AC
		public uint ConnectionId { get; set; }

		// Token: 0x060001D2 RID: 466 RVA: 0x0000D4B5 File Offset: 0x0000B6B5
		public override void Read(BamlBinaryReader reader)
		{
			this.ConnectionId = reader.ReadUInt32();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000D4C5 File Offset: 0x0000B6C5
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.ConnectionId);
		}
	}
}
