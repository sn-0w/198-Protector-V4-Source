using System;
using System.Collections.Generic;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x020000A1 RID: 161
	public abstract class PatternFunction : PatternExpression
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060003C9 RID: 969
		public abstract string Name { get; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060003CA RID: 970
		public abstract int ArgumentCount { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060003CB RID: 971 RVA: 0x0000392E File Offset: 0x00001B2E
		// (set) Token: 0x060003CC RID: 972 RVA: 0x00003936 File Offset: 0x00001B36
		public IList<PatternExpression> Arguments { get; set; }

		// Token: 0x060003CD RID: 973 RVA: 0x00016154 File Offset: 0x00014354
		public override void Serialize(IList<PatternToken> tokens)
		{
			tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
			tokens.Add(new PatternToken(TokenType.LParens));
			for (int i = 0; i < this.Arguments.Count; i++)
			{
				bool flag = i != 0;
				if (flag)
				{
					tokens.Add(new PatternToken(TokenType.Comma));
				}
				this.Arguments[i].Serialize(tokens);
			}
			tokens.Add(new PatternToken(TokenType.RParens));
		}
	}
}
