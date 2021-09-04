using System;
using System.Collections.Generic;
using System.Text;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000035 RID: 53
	public class StatementBlock : Statement
	{
		// Token: 0x06000117 RID: 279 RVA: 0x0000277B File Offset: 0x0000097B
		public StatementBlock()
		{
			this.Statements = new List<Statement>();
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000118 RID: 280 RVA: 0x00002791 File Offset: 0x00000991
		// (set) Token: 0x06000119 RID: 281 RVA: 0x00002799 File Offset: 0x00000999
		public IList<Statement> Statements { get; private set; }

		// Token: 0x0600011A RID: 282 RVA: 0x00007584 File Offset: 0x00005784
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("{");
			foreach (Statement statement in this.Statements)
			{
				stringBuilder.AppendLine(statement.ToString());
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}
	}
}
