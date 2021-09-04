using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000051 RID: 81
	internal class ConstructorParameterTypeRecord : BamlRecord
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000DA40 File Offset: 0x0000BC40
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParameterType;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000DA54 File Offset: 0x0000BC54
		// (set) Token: 0x0600022A RID: 554 RVA: 0x0000DA5C File Offset: 0x0000BC5C
		public ushort TypeId { get; set; }

		// Token: 0x0600022B RID: 555 RVA: 0x0000DA65 File Offset: 0x0000BC65
		public override void Read(BamlBinaryReader reader)
		{
			this.TypeId = reader.ReadUInt16();
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000DA75 File Offset: 0x0000BC75
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.TypeId);
		}
	}
}
