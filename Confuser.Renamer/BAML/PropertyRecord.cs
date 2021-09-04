using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002C RID: 44
	internal class PropertyRecord : SizedBamlRecord
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000136 RID: 310 RVA: 0x0000CA44 File Offset: 0x0000AC44
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.Property;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000137 RID: 311 RVA: 0x0000CA57 File Offset: 0x0000AC57
		// (set) Token: 0x06000138 RID: 312 RVA: 0x0000CA5F File Offset: 0x0000AC5F
		public ushort AttributeId { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000139 RID: 313 RVA: 0x0000CA68 File Offset: 0x0000AC68
		// (set) Token: 0x0600013A RID: 314 RVA: 0x0000CA70 File Offset: 0x0000AC70
		public string Value { get; set; }

		// Token: 0x0600013B RID: 315 RVA: 0x0000CA79 File Offset: 0x0000AC79
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AttributeId = reader.ReadUInt16();
			this.Value = reader.ReadString();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000CA96 File Offset: 0x0000AC96
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.Value);
		}
	}
}
