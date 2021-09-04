using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000055 RID: 85
	internal class StaticResourceIdRecord : BamlRecord
	{
		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600023C RID: 572 RVA: 0x0000DB80 File Offset: 0x0000BD80
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceId;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600023D RID: 573 RVA: 0x0000DB94 File Offset: 0x0000BD94
		// (set) Token: 0x0600023E RID: 574 RVA: 0x0000DB9C File Offset: 0x0000BD9C
		public ushort StaticResourceId { get; set; }

		// Token: 0x0600023F RID: 575 RVA: 0x0000DBA5 File Offset: 0x0000BDA5
		public override void Read(BamlBinaryReader reader)
		{
			this.StaticResourceId = reader.ReadUInt16();
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000DBB5 File Offset: 0x0000BDB5
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.StaticResourceId);
		}
	}
}
