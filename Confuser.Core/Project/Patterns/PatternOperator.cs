using System;
using System.Collections.Generic;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x020000A2 RID: 162
	public abstract class PatternOperator : PatternExpression
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060003CF RID: 975
		public abstract string Name { get; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060003D0 RID: 976
		public abstract bool IsUnary { get; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00003948 File Offset: 0x00001B48
		// (set) Token: 0x060003D2 RID: 978 RVA: 0x00003950 File Offset: 0x00001B50
		public PatternExpression OperandA { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060003D3 RID: 979 RVA: 0x00003959 File Offset: 0x00001B59
		// (set) Token: 0x060003D4 RID: 980 RVA: 0x00003961 File Offset: 0x00001B61
		public PatternExpression OperandB { get; set; }

		// Token: 0x060003D5 RID: 981 RVA: 0x000161D4 File Offset: 0x000143D4
		public override void Serialize(IList<PatternToken> tokens)
		{
			bool isUnary = this.IsUnary;
			if (isUnary)
			{
				tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
				this.OperandA.Serialize(tokens);
			}
			else
			{
				this.OperandA.Serialize(tokens);
				tokens.Add(new PatternToken(TokenType.Identifier, this.Name));
				this.OperandB.Serialize(tokens);
			}
		}
	}
}
