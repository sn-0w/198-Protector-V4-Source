using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000036 RID: 54
	internal class TextWithConverterRecord : TextRecord
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000D15C File Offset: 0x0000B35C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.TextWithConverter;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000D170 File Offset: 0x0000B370
		// (set) Token: 0x06000191 RID: 401 RVA: 0x0000D178 File Offset: 0x0000B378
		public ushort ConverterTypeId { get; set; }

		// Token: 0x06000192 RID: 402 RVA: 0x0000D181 File Offset: 0x0000B381
		protected override void ReadData(BamlBinaryReader reader, int size)
		{
			base.ReadData(reader, size);
			this.ConverterTypeId = reader.ReadUInt16();
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000D19A File Offset: 0x0000B39A
		protected override void WriteData(BamlBinaryWriter writer)
		{
			base.WriteData(writer);
			writer.Write(this.ConverterTypeId);
		}
	}
}
