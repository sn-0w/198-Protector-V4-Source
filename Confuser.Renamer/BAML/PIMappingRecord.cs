using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002A RID: 42
	internal class PIMappingRecord : SizedBamlRecord
	{
		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000124 RID: 292 RVA: 0x0000C938 File Offset: 0x0000AB38
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PIMapping;
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000C94C File Offset: 0x0000AB4C
		// (set) Token: 0x06000126 RID: 294 RVA: 0x0000C954 File Offset: 0x0000AB54
		public string XmlNamespace { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000127 RID: 295 RVA: 0x0000C95D File Offset: 0x0000AB5D
		// (set) Token: 0x06000128 RID: 296 RVA: 0x0000C965 File Offset: 0x0000AB65
		public string ClrNamespace { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000C96E File Offset: 0x0000AB6E
		// (set) Token: 0x0600012A RID: 298 RVA: 0x0000C976 File Offset: 0x0000AB76
		public ushort AssemblyId { get; set; }

		// Token: 0x0600012B RID: 299 RVA: 0x0000C97F File Offset: 0x0000AB7F
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.XmlNamespace = reader.ReadString();
			this.ClrNamespace = reader.ReadString();
			this.AssemblyId = reader.ReadUInt16();
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000C9A9 File Offset: 0x0000ABA9
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.XmlNamespace);
			writer.Write(this.ClrNamespace);
			writer.Write(this.AssemblyId);
		}
	}
}
