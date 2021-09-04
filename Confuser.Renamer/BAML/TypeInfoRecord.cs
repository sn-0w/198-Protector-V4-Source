using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000031 RID: 49
	internal class TypeInfoRecord : SizedBamlRecord
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000165 RID: 357 RVA: 0x0000CEE0 File Offset: 0x0000B0E0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TypeInfo;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000166 RID: 358 RVA: 0x0000CEF4 File Offset: 0x0000B0F4
		// (set) Token: 0x06000167 RID: 359 RVA: 0x0000CEFC File Offset: 0x0000B0FC
		public ushort TypeId { get; set; }

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000168 RID: 360 RVA: 0x0000CF05 File Offset: 0x0000B105
		// (set) Token: 0x06000169 RID: 361 RVA: 0x0000CF0D File Offset: 0x0000B10D
		public ushort AssemblyId { get; set; }

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600016A RID: 362 RVA: 0x0000CF16 File Offset: 0x0000B116
		// (set) Token: 0x0600016B RID: 363 RVA: 0x0000CF1E File Offset: 0x0000B11E
		public string TypeFullName { get; set; }

		// Token: 0x0600016C RID: 364 RVA: 0x0000CF27 File Offset: 0x0000B127
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.TypeId = reader.ReadUInt16();
			this.AssemblyId = reader.ReadUInt16();
			this.TypeFullName = reader.ReadString();
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000CF51 File Offset: 0x0000B151
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
			writer.Write(this.AssemblyId);
			writer.Write(this.TypeFullName);
		}
	}
}
