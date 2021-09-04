using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000028 RID: 40
	internal class XmlnsPropertyRecord : SizedBamlRecord
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000112 RID: 274 RVA: 0x0000C7BC File Offset: 0x0000A9BC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.XmlnsProperty;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000113 RID: 275 RVA: 0x0000C7D0 File Offset: 0x0000A9D0
		// (set) Token: 0x06000114 RID: 276 RVA: 0x0000C7D8 File Offset: 0x0000A9D8
		public string Prefix { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000115 RID: 277 RVA: 0x0000C7E1 File Offset: 0x0000A9E1
		// (set) Token: 0x06000116 RID: 278 RVA: 0x0000C7E9 File Offset: 0x0000A9E9
		public string XmlNamespace { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000117 RID: 279 RVA: 0x0000C7F2 File Offset: 0x0000A9F2
		// (set) Token: 0x06000118 RID: 280 RVA: 0x0000C7FA File Offset: 0x0000A9FA
		public ushort[] AssemblyIds { get; set; }

		// Token: 0x06000119 RID: 281 RVA: 0x0000C804 File Offset: 0x0000AA04
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.Prefix = reader.ReadString();
			this.XmlNamespace = reader.ReadString();
			this.AssemblyIds = new ushort[(int)reader.ReadUInt16()];
			for (int i = 0; i < this.AssemblyIds.Length; i++)
			{
				this.AssemblyIds[i] = reader.ReadUInt16();
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000C864 File Offset: 0x0000AA64
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.Prefix);
			writer.Write(this.XmlNamespace);
			writer.Write((ushort)this.AssemblyIds.Length);
			foreach (ushort value in this.AssemblyIds)
			{
				writer.Write(value);
			}
		}
	}
}
