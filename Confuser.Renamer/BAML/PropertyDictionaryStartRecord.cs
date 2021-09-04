using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000049 RID: 73
	internal class PropertyDictionaryStartRecord : PropertyComplexStartRecord
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000D970 File Offset: 0x0000BB70
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.PropertyDictionaryStart;
			}
		}
	}
}
