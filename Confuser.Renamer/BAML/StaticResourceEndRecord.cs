using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000054 RID: 84
	internal class StaticResourceEndRecord : BamlRecord
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000238 RID: 568 RVA: 0x0000DB6C File Offset: 0x0000BD6C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceEnd;
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
