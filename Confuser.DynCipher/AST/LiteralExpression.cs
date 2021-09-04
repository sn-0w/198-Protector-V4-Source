using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000032 RID: 50
	public class LiteralExpression : Expression
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000108 RID: 264 RVA: 0x0000272E File Offset: 0x0000092E
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00002736 File Offset: 0x00000936
		public uint Value { get; set; }

		// Token: 0x0600010A RID: 266 RVA: 0x000074E0 File Offset: 0x000056E0
		public static implicit operator LiteralExpression(uint val)
		{
			return new LiteralExpression
			{
				Value = val
			};
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00007500 File Offset: 0x00005700
		public override string ToString()
		{
			return this.Value.ToString("x8") + "h";
		}
	}
}
