using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009E RID: 158
	public class LiteralExpression : PatternExpression
	{
		// Token: 0x060003BD RID: 957 RVA: 0x0000390B File Offset: 0x00001B0B
		public LiteralExpression(object literal)
		{
			this.Literal = literal;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060003BE RID: 958 RVA: 0x0000391D File Offset: 0x00001B1D
		// (set) Token: 0x060003BF RID: 959 RVA: 0x00003925 File Offset: 0x00001B25
		public object Literal { get; private set; }

		// Token: 0x060003C0 RID: 960 RVA: 0x00016084 File Offset: 0x00014284
		public override object Evaluate(IDnlibDef definition)
		{
			return this.Literal;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001609C File Offset: 0x0001429C
		public override void Serialize(IList<PatternToken> tokens)
		{
			bool flag = this.Literal is bool;
			if (flag)
			{
				tokens.Add(new PatternToken(TokenType.Identifier, ((bool)this.Literal).ToString().ToLowerInvariant()));
			}
			else
			{
				tokens.Add(new PatternToken(TokenType.Literal, this.Literal.ToString()));
			}
		}
	}
}
