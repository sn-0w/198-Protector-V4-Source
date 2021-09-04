using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000047 RID: 71
	internal class PropertyListStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000204 RID: 516 RVA: 0x0000D948 File Offset: 0x0000BB48
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyListStart;
			}
		}
	}
}
