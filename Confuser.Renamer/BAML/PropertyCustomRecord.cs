using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002E RID: 46
	internal class PropertyCustomRecord : SizedBamlRecord
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000144 RID: 324 RVA: 0x0000CB14 File Offset: 0x0000AD14
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyCustom;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000CB27 File Offset: 0x0000AD27
		// (set) Token: 0x06000146 RID: 326 RVA: 0x0000CB2F File Offset: 0x0000AD2F
		public ushort AttributeId { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000147 RID: 327 RVA: 0x0000CB38 File Offset: 0x0000AD38
		// (set) Token: 0x06000148 RID: 328 RVA: 0x0000CB40 File Offset: 0x0000AD40
		public ushort SerializerTypeId { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000149 RID: 329 RVA: 0x0000CB49 File Offset: 0x0000AD49
		// (set) Token: 0x0600014A RID: 330 RVA: 0x0000CB51 File Offset: 0x0000AD51
		public byte[] Data { get; set; }

		// Token: 0x0600014B RID: 331 RVA: 0x0000CB5C File Offset: 0x0000AD5C
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			long position = reader.BaseStream.Position;
			this.AttributeId = reader.ReadUInt16();
			this.SerializerTypeId = reader.ReadUInt16();
			this.Data = reader.ReadBytes(size - (int)(reader.BaseStream.Position - position));
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000CBAD File Offset: 0x0000ADAD
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
			writer.Write(this.SerializerTypeId);
			writer.Write(this.Data);
		}
	}
}
