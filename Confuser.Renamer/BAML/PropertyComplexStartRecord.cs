using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004D RID: 77
	internal class PropertyComplexStartRecord : BamlRecord
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000216 RID: 534 RVA: 0x0000D9C0 File Offset: 0x0000BBC0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyComplexStart;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000217 RID: 535 RVA: 0x0000D9D3 File Offset: 0x0000BBD3
		// (set) Token: 0x06000218 RID: 536 RVA: 0x0000D9DB File Offset: 0x0000BBDB
		public ushort AttributeId { get; set; }

		// Token: 0x06000219 RID: 537 RVA: 0x0000D9E4 File Offset: 0x0000BBE4
		public override void Read(BamlBinaryReader reader)
		{
			this.AttributeId = reader.ReadUInt16();
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000D9F4 File Offset: 0x0000BBF4
		public override void Write(BamlBinaryWriter writer)
		{
			writer.Write(this.AttributeId);
		}
	}
}
