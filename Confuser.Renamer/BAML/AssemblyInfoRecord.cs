using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200002B RID: 43
	internal class AssemblyInfoRecord : SizedBamlRecord
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000C9D4 File Offset: 0x0000ABD4
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.AssemblyInfo;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600012F RID: 303 RVA: 0x0000C9E8 File Offset: 0x0000ABE8
		// (set) Token: 0x06000130 RID: 304 RVA: 0x0000C9F0 File Offset: 0x0000ABF0
		public ushort AssemblyId { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000131 RID: 305 RVA: 0x0000C9F9 File Offset: 0x0000ABF9
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000CA01 File Offset: 0x0000AC01
		public string AssemblyFullName { get; set; }

		// Token: 0x06000133 RID: 307 RVA: 0x0000CA0A File Offset: 0x0000AC0A
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.AssemblyId = reader.ReadUInt16();
			this.AssemblyFullName = reader.ReadString();
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000CA27 File Offset: 0x0000AC27
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.AssemblyId);
			writer.Write(this.AssemblyFullName);
		}
	}
}
