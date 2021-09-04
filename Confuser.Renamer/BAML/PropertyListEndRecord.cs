using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000048 RID: 72
	internal class PropertyListEndRecord : BamlRecord
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000206 RID: 518 RVA: 0x0000D95C File Offset: 0x0000BB5C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyListEnd;
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
