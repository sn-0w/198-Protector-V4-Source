using System;
using System.Text;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000033 RID: 51
	public class LoopStatement : StatementBlock
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600010D RID: 269 RVA: 0x0000273F File Offset: 0x0000093F
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00002747 File Offset: 0x00000947
		public int Begin { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00002750 File Offset: 0x00000950
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00002758 File Offset: 0x00000958
		public int Limit { get; set; }

		// Token: 0x06000111 RID: 273 RVA: 0x00007530 File Offset: 0x00005730
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("for (int i = {0}; i < {1}; i++)", this.Begin, this.Limit);
			stringBuilder.AppendLine();
			stringBuilder.Append(base.ToString());
			return stringBuilder.ToString();
		}
	}
}
