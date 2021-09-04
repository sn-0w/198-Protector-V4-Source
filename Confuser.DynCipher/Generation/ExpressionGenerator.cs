using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000018 RID: 24
	internal class ExpressionGenerator
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00004DF4 File Offset: 0x00002FF4
		private static Expression GenerateExpression(RandomGenerator random, Expression current, int currentDepth, int targetDepth)
		{
			bool flag = currentDepth == targetDepth || (currentDepth > targetDepth / 3 && random.NextInt32(100) > 85);
			Expression result;
			if (flag)
			{
				result = current;
			}
			else
			{
				switch (random.NextInt32(6))
				{
				case 0:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) + ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth);
					break;
				case 1:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) - ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth);
					break;
				case 2:
					result = ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) * (random.NextUInt32() | 1U);
					break;
				case 3:
					result = (ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth) ^ ExpressionGenerator.GenerateExpression(random, random.NextUInt32(), currentDepth + 1, targetDepth));
					break;
				case 4:
					result = ~ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth);
					break;
				case 5:
					result = -ExpressionGenerator.GenerateExpression(random, current, currentDepth + 1, targetDepth);
					break;
				default:
					throw new UnreachableException();
				}
			}
			return result;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004F20 File Offset: 0x00003120
		private static void SwapOperands(RandomGenerator random, Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binOpExpression = (BinOpExpression)exp;
				bool flag2 = random.NextBoolean();
				if (flag2)
				{
					Expression left = binOpExpression.Left;
					binOpExpression.Left = binOpExpression.Right;
					binOpExpression.Right = left;
				}
				ExpressionGenerator.SwapOperands(random, binOpExpression.Left);
				ExpressionGenerator.SwapOperands(random, binOpExpression.Right);
			}
			else
			{
				bool flag3 = exp is UnaryOpExpression;
				if (flag3)
				{
					ExpressionGenerator.SwapOperands(random, ((UnaryOpExpression)exp).Value);
				}
				else
				{
					bool flag4 = exp is LiteralExpression || exp is VariableExpression;
					if (!flag4)
					{
						throw new UnreachableException();
					}
				}
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004FD0 File Offset: 0x000031D0
		private static bool HasVariable(Expression exp, Dictionary<Expression, bool> hasVar)
		{
			bool flag2;
			bool flag = !hasVar.TryGetValue(exp, out flag2);
			if (flag)
			{
				bool flag3 = exp is VariableExpression;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = exp is LiteralExpression;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = exp is BinOpExpression;
						if (flag5)
						{
							BinOpExpression binOpExpression = (BinOpExpression)exp;
							flag2 = (ExpressionGenerator.HasVariable(binOpExpression.Left, hasVar) || ExpressionGenerator.HasVariable(binOpExpression.Right, hasVar));
						}
						else
						{
							bool flag6 = exp is UnaryOpExpression;
							if (!flag6)
							{
								throw new UnreachableException();
							}
							flag2 = ExpressionGenerator.HasVariable(((UnaryOpExpression)exp).Value, hasVar);
						}
					}
				}
				hasVar[exp] = flag2;
			}
			return flag2;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000508C File Offset: 0x0000328C
		private static Expression GenerateInverse(Expression exp, Expression var, Dictionary<Expression, bool> hasVar)
		{
			Expression expression = var;
			while (!(exp is VariableExpression))
			{
				Debug.Assert(hasVar[exp]);
				bool flag = exp is UnaryOpExpression;
				if (flag)
				{
					UnaryOpExpression unaryOpExpression = (UnaryOpExpression)exp;
					expression = new UnaryOpExpression
					{
						Operation = unaryOpExpression.Operation,
						Value = expression
					};
					exp = unaryOpExpression.Value;
				}
				else
				{
					bool flag2 = exp is BinOpExpression;
					if (flag2)
					{
						BinOpExpression binOpExpression = (BinOpExpression)exp;
						bool flag3 = hasVar[binOpExpression.Left];
						Expression expression2 = flag3 ? binOpExpression.Left : binOpExpression.Right;
						Expression expression3 = flag3 ? binOpExpression.Right : binOpExpression.Left;
						bool flag4 = binOpExpression.Operation == BinOps.Add;
						if (flag4)
						{
							expression = new BinOpExpression
							{
								Operation = BinOps.Sub,
								Left = expression,
								Right = expression3
							};
						}
						else
						{
							bool flag5 = binOpExpression.Operation == BinOps.Sub;
							if (flag5)
							{
								bool flag6 = flag3;
								if (flag6)
								{
									expression = new BinOpExpression
									{
										Operation = BinOps.Add,
										Left = expression,
										Right = expression3
									};
								}
								else
								{
									expression = new BinOpExpression
									{
										Operation = BinOps.Sub,
										Left = expression3,
										Right = expression
									};
								}
							}
							else
							{
								bool flag7 = binOpExpression.Operation == BinOps.Mul;
								if (flag7)
								{
									Debug.Assert(expression3 is LiteralExpression);
									uint num = ((LiteralExpression)expression3).Value;
									num = MathsUtils.modInv(num);
									expression = new BinOpExpression
									{
										Operation = BinOps.Mul,
										Left = expression,
										Right = num
									};
								}
								else
								{
									bool flag8 = binOpExpression.Operation == BinOps.Xor;
									if (flag8)
									{
										expression = new BinOpExpression
										{
											Operation = BinOps.Xor,
											Left = expression,
											Right = expression3
										};
									}
								}
							}
						}
						exp = expression2;
					}
				}
			}
			return expression;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005288 File Offset: 0x00003488
		public static void GeneratePair(RandomGenerator random, Expression var, Expression result, int depth, out Expression expression, out Expression inverse)
		{
			expression = ExpressionGenerator.GenerateExpression(random, var, 0, depth);
			ExpressionGenerator.SwapOperands(random, expression);
			Dictionary<Expression, bool> hasVar = new Dictionary<Expression, bool>();
			ExpressionGenerator.HasVariable(expression, hasVar);
			inverse = ExpressionGenerator.GenerateInverse(expression, result, hasVar);
		}

		// Token: 0x02000019 RID: 25
		private enum ExpressionOps
		{
			// Token: 0x04000043 RID: 67
			Add,
			// Token: 0x04000044 RID: 68
			Sub,
			// Token: 0x04000045 RID: 69
			Mul,
			// Token: 0x04000046 RID: 70
			Xor,
			// Token: 0x04000047 RID: 71
			Not,
			// Token: 0x04000048 RID: 72
			Neg
		}
	}
}
