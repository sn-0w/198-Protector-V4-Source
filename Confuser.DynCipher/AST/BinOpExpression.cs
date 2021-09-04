using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000030 RID: 48
	public class BinOpExpression : Expression
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000F2 RID: 242 RVA: 0x000026EA File Offset: 0x000008EA
		// (set) Token: 0x060000F3 RID: 243 RVA: 0x000026F2 File Offset: 0x000008F2
		public Expression Left { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000026FB File Offset: 0x000008FB
		// (set) Token: 0x060000F5 RID: 245 RVA: 0x00002703 File Offset: 0x00000903
		public Expression Right { get; set; }

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x0000270C File Offset: 0x0000090C
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00002714 File Offset: 0x00000914
		public BinOps Operation { get; set; }

		// Token: 0x060000F8 RID: 248 RVA: 0x0000725C File Offset: 0x0000545C
		public override string ToString()
		{
			string arg;
			switch (this.Operation)
			{
			case BinOps.Add:
				arg = "+";
				break;
			case BinOps.Sub:
				arg = "-";
				break;
			case BinOps.Div:
				arg = "/";
				break;
			case BinOps.Mul:
				arg = "*";
				break;
			case BinOps.Or:
				arg = "|";
				break;
			case BinOps.And:
				arg = "&";
				break;
			case BinOps.Xor:
				arg = "^";
				break;
			case BinOps.Lsh:
				arg = "<<";
				break;
			case BinOps.Rsh:
				arg = ">>";
				break;
			default:
				throw new Exception();
			}
			return string.Format("({0} {1} {2})", this.Left, arg, this.Right);
		}
	}
}
