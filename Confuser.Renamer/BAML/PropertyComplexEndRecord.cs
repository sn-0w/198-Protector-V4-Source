using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004E RID: 78
	internal class PropertyComplexEndRecord : BamlRecord
	{
		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000DA04 File Offset: 0x0000BC04
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyComplexEnd;
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
