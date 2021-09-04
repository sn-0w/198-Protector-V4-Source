using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000056 RID: 86
	internal class OptimizedStaticResourceRecord : BamlRecord
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000242 RID: 578 RVA: 0x0000DBC8 File Offset: 0x0000BDC8
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.OptimizedStaticResource;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000243 RID: 579 RVA: 0x0000DBDC File Offset: 0x0000BDDC
		// (set) Token: 0x06000244 RID: 580 RVA: 0x0000DBE4 File Offset: 0x0000BDE4
		public byte Flags { get; set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000245 RID: 581 RVA: 0x0000DBED File Offset: 0x0000BDED
		// (set) Token: 0x06000246 RID: 582 RVA: 0x0000DBF5 File Offset: 0x0000BDF5
		public ushort ValueId { get; set; }

		// Token: 0x06000247 RID: 583 RVA: 0x0000DBFE File Offset: 0x0000BDFE
		public override void Read(BamlBinaryReader reader)
		{
			this.Flags = reader.ReadByte();
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000DC1B File Offset: 0x0000BE1B
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.Flags);
			writer.Write(this.ValueId);
		}
	}
}
