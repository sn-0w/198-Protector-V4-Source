using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004C RID: 76
	internal class PropertyArrayEndRecord : BamlRecord
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000212 RID: 530 RVA: 0x0000D9AC File Offset: 0x0000BBAC
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyArrayEnd;
			}
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
