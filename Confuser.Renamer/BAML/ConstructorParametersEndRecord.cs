using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000050 RID: 80
	internal class ConstructorParametersEndRecord : BamlRecord
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000224 RID: 548 RVA: 0x0000DA2C File Offset: 0x0000BC2C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.ConstructorParametersEnd;
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Read(BamlBinaryReader reader)
		{
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public override void Write(BamlBinaryWriter writer)
		{
		}
	}
}
