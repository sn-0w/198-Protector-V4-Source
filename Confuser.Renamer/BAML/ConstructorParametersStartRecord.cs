using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004F RID: 79
	internal class ConstructorParametersStartRecord : BamlRecord
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000DA18 File Offset: 0x0000BC18
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParametersStart;
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
