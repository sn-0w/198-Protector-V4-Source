using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200004B RID: 75
	internal class PropertyArrayStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000210 RID: 528 RVA: 0x0000D998 File Offset: 0x0000BB98
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyArrayStart;
			}
		}
	}
}
