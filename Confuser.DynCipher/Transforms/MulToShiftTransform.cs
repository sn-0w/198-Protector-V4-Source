using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x02000009 RID: 9
	internal class MulToShiftTransform
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002E8C File Offset: 0x0000108C
		private static uint NumberOfSetBits(uint i)
		{
			i -= (i >> 1 & 1431655765U);
			i = (i & 858993459U) + (i >> 2 & 858993459U);
			return (i + (i >> 4) & 252645135U) * 16843009U >> 24;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002ED4 File Offset: 0x000010D4
		private static Expression ProcessExpression(Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binOpExpression = (BinOpExpression)exp;
				bool flag2 = binOpExpression.Operation == BinOps.Mul && binOpExpression.Right is LiteralExpression;
				if (flag2)
				{
					uint num = ((LiteralExpression)binOpExpression.Right).Value;
					bool flag3 = num == 0U;
					if (flag3)
					{
						return 0U;
					}
					bool flag4 = num == 1U;
					if (flag4)
					{
						return binOpExpression.Left;
					}
					uint num2 = MulToShiftTransform.NumberOfSetBits(num);
					bool flag5 = num2 <= 2U;
					if (flag5)
					{
						List<Expression> list = new List<Expression>();
						int num3 = 0;
						while (num > 0U)
						{
							bool flag6 = (num & 1U) > 0U;
							if (flag6)
							{
								bool flag7 = num3 == 0;
								if (flag7)
								{
									list.Add(binOpExpression.Left);
								}
								else
								{
									list.Add(binOpExpression.Left << num3);
								}
							}
							num >>= 1;
							num3++;
						}
						BinOpExpression binOpExpression2 = list.OfType<BinOpExpression>().First<BinOpExpression>();
						foreach (Expression b in list.Except(new BinOpExpression[]
						{
							binOpExpression2
						}))
						{
							binOpExpression2 += b;
						}
						return binOpExpression2;
					}
				}
				else
				{
					binOpExpression.Left = MulToShiftTransform.ProcessExpression(binOpExpression.Left);
					binOpExpression.Right = MulToShiftTransform.ProcessExpression(binOpExpression.Right);
				}
			}
			else
			{
				bool flag8 = exp is ArrayIndexExpression;
				if (flag8)
				{
					((ArrayIndexExpression)exp).Array = MulToShiftTransform.ProcessExpression(((ArrayIndexExpression)exp).Array);
				}
				else
				{
					bool flag9 = exp is UnaryOpExpression;
					if (flag9)
					{
						((UnaryOpExpression)exp).Value = MulToShiftTransform.ProcessExpression(((UnaryOpExpression)exp).Value);
					}
				}
			}
			return exp;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000030D4 File Offset: 0x000012D4
		private static void ProcessStatement(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assignmentStatement = (AssignmentStatement)st;
				assignmentStatement.Target = MulToShiftTransform.ProcessExpression(assignmentStatement.Target);
				assignmentStatement.Value = MulToShiftTransform.ProcessExpression(assignmentStatement.Value);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000311C File Offset: 0x0000131C
		public static void Run(StatementBlock block)
		{
			foreach (Statement st in block.Statements)
			{
				MulToShiftTransform.ProcessStatement(st);
			}
		}
	}
}
