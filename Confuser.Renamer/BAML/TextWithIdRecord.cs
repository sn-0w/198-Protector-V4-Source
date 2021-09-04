using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000037 RID: 55
	internal class TextWithIdRecord : TextRecord
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000195 RID: 405 RVA: 0x0000D1BC File Offset: 0x0000B3BC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TextWithId;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000196 RID: 406 RVA: 0x0000D1D0 File Offset: 0x0000B3D0
		// (set) Token: 0x06000197 RID: 407 RVA: 0x0000D1D8 File Offset: 0x0000B3D8
		public ushort ValueId { get; set; }

		// Token: 0x06000198 RID: 408 RVA: 0x0000D1E1 File Offset: 0x0000B3E1
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			this.ValueId = reader.ReadUInt16();
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000D1F1 File Offset: 0x0000B3F1
		protected override void WriteData(BamlBinaryWriter writer)
		{
			writer.Write(this.ValueId);
		}
	}
}
