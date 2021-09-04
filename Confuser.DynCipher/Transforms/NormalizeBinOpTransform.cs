using System;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x0200000A RID: 10
	internal class NormalizeBinOpTransform
	{
		// Token: 0x0600001E RID: 30 RVA: 0x0000316C File Offset: 0x0000136C
		private static Expression ProcessExpression(Expression exp)
		{
			bool flag = exp is BinOpExpression;
			if (flag)
			{
				BinOpExpression binOpExpression = (BinOpExpression)exp;
				BinOpExpression binOpExpression2 = binOpExpression.Right as BinOpExpression;
				bool flag2 = binOpExpression2 != null && binOpExpression2.Operation == binOpExpression.Operation && (binOpExpression.Operation == BinOps.Add || binOpExpression.Operation == BinOps.Mul || binOpExpression.Operation == BinOps.Or || binOpExpression.Operation == BinOps.And || binOpExpression.Operation == BinOps.Xor);
				if (flag2)
				{
					binOpExpression.Left = new BinOpExpression
					{
						Left = binOpExpression.Left,
						Operation = binOpExpression.Operation,
						Right = binOpExpression2.Left
					};
					binOpExpression.Right = binOpExpression2.Right;
				}
				binOpExpression.Left = NormalizeBinOpTransform.ProcessExpression(binOpExpression.Left);
				binOpExpression.Right = NormalizeBinOpTransform.ProcessExpression(binOpExpression.Right);
				bool flag3 = binOpExpression.Right is LiteralExpression && ((LiteralExpression)binOpExpression.Right).Value == 0U && binOpExpression.Operation == BinOps.Add;
				if (flag3)
				{
					return binOpExpression.Left;
				}
			}
			else
			{
				bool flag4 = exp is ArrayIndexExpression;
				if (flag4)
				{
					((ArrayIndexExpression)exp).Array = NormalizeBinOpTransform.ProcessExpression(((ArrayIndexExpression)exp).Array);
				}
				else
				{
					bool flag5 = exp is UnaryOpExpression;
					if (flag5)
					{
						((UnaryOpExpression)exp).Value = NormalizeBinOpTransform.ProcessExpression(((UnaryOpExpression)exp).Value);
					}
				}
			}
			return exp;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000032EC File Offset: 0x000014EC
		private static void ProcessStatement(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assignmentStatement = (AssignmentStatement)st;
				assignmentStatement.Target = NormalizeBinOpTransform.ProcessExpression(assignmentStatement.Target);
				assignmentStatement.Value = NormalizeBinOpTransform.ProcessExpression(assignmentStatement.Value);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003334 File Offset: 0x00001534
		public static void Run(StatementBlock block)
		{
			foreach (Statement st in block.Statements)
			{
				NormalizeBinOpTransform.ProcessStatement(st);
			}
		}
	}
}
