using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003B RID: 59
	internal class DocumentEndRecord : BamlRecord
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x0000D3C0 File Offset: 0x0000B5C0
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.DocumentEnd;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
