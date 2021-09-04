using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000039 RID: 57
	public class VariableExpression : Expression
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000027F8 File Offset: 0x000009F8
		// (set) Token: 0x06000128 RID: 296 RVA: 0x00002800 File Offset: 0x00000A00
		public Variable Variable { get; set; }

		// Token: 0x06000129 RID: 297 RVA: 0x00007664 File Offset: 0x00005864
		public override string ToString()
		{
			return this.Variable.Name;
		}
	}
}
