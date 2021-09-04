using System;
using System.Collections.Generic;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006C RID: 108
	internal struct SourceValueInfo
	{
		// Token: 0x060002A6 RID: 678 RVA: 0x00025477 File Offset: 0x00023677
		public SourceValueInfo(SourceValueType t, DrillIn d, string n)
		{
			this.type = t;
			this.drillIn = d;
			this.name = n;
			this.paramList = null;
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x00025496 File Offset: 0x00023696
		public SourceValueInfo(SourceValueType t, DrillIn d, IReadOnlyList<IndexerParamInfo> list)
		{
			this.type = t;
			this.drillIn = d;
			this.name = null;
			this.paramList = list;
		}

		// Token: 0x04000529 RID: 1321
		public SourceValueType type;

		// Token: 0x0400052A RID: 1322
		public DrillIn drillIn;

		// Token: 0x0400052B RID: 1323
		public string name;

		// Token: 0x0400052C RID: 1324
		public IReadOnlyList<IndexerParamInfo> paramList;
	}
}
