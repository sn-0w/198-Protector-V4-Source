using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004A RID: 74
	internal class PropertyDictionaryEndRecord : BamlRecord
	{
		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000D984 File Offset: 0x0000BB84
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyDictionaryEnd;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
