using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000034 RID: 52
	public abstract class Statement
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000113 RID: 275 RVA: 0x0000276A File Offset: 0x0000096A
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00002772 File Offset: 0x00000972
		public object Tag { get; set; }

		// Token: 0x06000115 RID: 277
		public abstract override string ToString();
	}
}
