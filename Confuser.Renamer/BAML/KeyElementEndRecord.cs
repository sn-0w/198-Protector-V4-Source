using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200003F RID: 63
	internal class KeyElementEndRecord : BamlRecord
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001CB RID: 459 RVA: 0x0000D47C File Offset: 0x0000B67C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.KeyElementEnd;
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x060001CD RID: 461 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
