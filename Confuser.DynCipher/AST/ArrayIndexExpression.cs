using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x0200002D RID: 45
	public class ArrayIndexExpression : Expression
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00002694 File Offset: 0x00000894
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x0000269C File Offset: 0x0000089C
		public Expression Array { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000026A5 File Offset: 0x000008A5
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x000026AD File Offset: 0x000008AD
		public int Index { get; set; }

		// Token: 0x060000EA RID: 234 RVA: 0x00007204 File Offset: 0x00005404
		public override string ToString()
		{
			return string.Format("{0}[{1}]", this.Array, this.Index);
		}
	}
}
