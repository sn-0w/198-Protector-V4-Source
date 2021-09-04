using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000058 RID: 88
	internal class LinePositionRecord : BamlRecord
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000252 RID: 594 RVA: 0x0000DCA8 File Offset: 0x0000BEA8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.LinePosition;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000253 RID: 595 RVA: 0x0000DCBC File Offset: 0x0000BEBC
		// (set) Token: 0x06000254 RID: 596 RVA: 0x0000DCC4 File Offset: 0x0000BEC4
		public uint LinePosition { get; set; }

		// Token: 0x06000255 RID: 597 RVA: 0x0000DCCD File Offset: 0x0000BECD
		public override void Read(BamlBinaryReader reader)
		{
			this.LinePosition = reader.ReadUInt32();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x0000DCDD File Offset: 0x0000BEDD
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.LinePosition);
		}
	}
}
