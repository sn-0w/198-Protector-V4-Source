using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000038 RID: 56
	public class Variable
	{
		// Token: 0x06000121 RID: 289 RVA: 0x000027C4 File Offset: 0x000009C4
		public Variable(string name)
		{
			this.Name = name;
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000122 RID: 290 RVA: 0x000027D6 File Offset: 0x000009D6
		// (set) Token: 0x06000123 RID: 291 RVA: 0x000027DE File Offset: 0x000009DE
		public string Name { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000027E7 File Offset: 0x000009E7
		// (set) Token: 0x06000125 RID: 293 RVA: 0x000027EF File Offset: 0x000009EF
		public object Tag { get; set; }

		// Token: 0x06000126 RID: 294 RVA: 0x0000764C File Offset: 0x0000584C
		public override string ToString()
		{
			return this.Name;
		}
	}
}
