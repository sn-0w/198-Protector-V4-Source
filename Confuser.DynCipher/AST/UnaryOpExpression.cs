using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000037 RID: 55
	public class UnaryOpExpression : Expression
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600011B RID: 283 RVA: 0x000027A2 File Offset: 0x000009A2
		// (set) Token: 0x0600011C RID: 284 RVA: 0x000027AA File Offset: 0x000009AA
		public Expression Value { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600011D RID: 285 RVA: 0x000027B3 File Offset: 0x000009B3
		// (set) Token: 0x0600011E RID: 286 RVA: 0x000027BB File Offset: 0x000009BB
		public UnaryOps Operation { get; set; }

		// Token: 0x0600011F RID: 287 RVA: 0x00007604 File Offset: 0x00005804
		public override string ToString()
		{
			UnaryOps operation = this.Operation;
			UnaryOps unaryOps = operation;
			string arg;
			if (unaryOps != UnaryOps.Not)
			{
				if (unaryOps != UnaryOps.Negate)
				{
					throw new Exception();
				}
				arg = "-";
			}
			else
			{
				arg = "~";
			}
			return arg + this.Value;
		}
	}
}
