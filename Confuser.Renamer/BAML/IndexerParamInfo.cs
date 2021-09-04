using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006A RID: 106
	internal struct IndexerParamInfo
	{
		// Token: 0x0600029C RID: 668 RVA: 0x00024D4B File Offset: 0x00022F4B
		public IndexerParamInfo(string paren, string value)
		{
			this.parenString = paren;
			this.valueString = value;
		}

		// Token: 0x0400051C RID: 1308
		public string parenString;

		// Token: 0x0400051D RID: 1309
		public string valueString;
	}
}
