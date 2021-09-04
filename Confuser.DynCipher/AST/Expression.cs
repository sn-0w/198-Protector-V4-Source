using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x02000031 RID: 49
	public abstract class Expression
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000271D File Offset: 0x0000091D
		// (set) Token: 0x060000FB RID: 251 RVA: 0x00002725 File Offset: 0x00000925
		public object Tag { get; set; }

		// Token: 0x060000FC RID: 252
		public abstract override string ToString();

		// Token: 0x060000FD RID: 253 RVA: 0x00007308 File Offset: 0x00005508
		public static BinOpExpression operator +(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Add
			};
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00007338 File Offset: 0x00005538
		public static BinOpExpression operator -(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Sub
			};
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00007368 File Offset: 0x00005568
		public static BinOpExpression operator *(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Mul
			};
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00007398 File Offset: 0x00005598
		public static BinOpExpression operator >>(Expression a, int b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = (uint)b,
				Operation = BinOps.Rsh
			};
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000073CC File Offset: 0x000055CC
		public static BinOpExpression operator <<(Expression a, int b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = (uint)b,
				Operation = BinOps.Lsh
			};
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00007400 File Offset: 0x00005600
		public static BinOpExpression operator |(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Or
			};
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00007430 File Offset: 0x00005630
		public static BinOpExpression operator &(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.And
			};
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00007460 File Offset: 0x00005660
		public static BinOpExpression operator ^(Expression a, Expression b)
		{
			return new BinOpExpression
			{
				Left = a,
				Right = b,
				Operation = BinOps.Xor
			};
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00007490 File Offset: 0x00005690
		public static UnaryOpExpression operator ~(Expression val)
		{
			return new UnaryOpExpression
			{
				Value = val,
				Operation = UnaryOps.Not
			};
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000074B8 File Offset: 0x000056B8
		public static UnaryOpExpression operator -(Expression val)
		{
			return new UnaryOpExpression
			{
				Value = val,
				Operation = UnaryOps.Negate
			};
		}
	}
}
