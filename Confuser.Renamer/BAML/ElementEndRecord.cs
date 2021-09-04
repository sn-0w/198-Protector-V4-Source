using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003D RID: 61
	internal class ElementEndRecord : BamlRecord
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x0000D448 File Offset: 0x0000B648
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ElementEnd;
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
