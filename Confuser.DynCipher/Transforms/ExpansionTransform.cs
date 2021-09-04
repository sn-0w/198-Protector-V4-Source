using System;
using System.Linq;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x02000008 RID: 8
	internal class ExpansionTransform
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002D20 File Offset: 0x00000F20
		private static bool ProcessStatement(Statement st, StatementBlock block)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assignmentStatement = (AssignmentStatement)st;
				bool flag2 = assignmentStatement.Value is BinOpExpression;
				if (flag2)
				{
					BinOpExpression binOpExpression = (BinOpExpression)assignmentStatement.Value;
					bool flag3 = (binOpExpression.Left is BinOpExpression || binOpExpression.Right is BinOpExpression) && binOpExpression.Left != assignmentStatement.Target;
					if (flag3)
					{
						block.Statements.Add(new AssignmentStatement
						{
							Target = assignmentStatement.Target,
							Value = binOpExpression.Left
						});
						block.Statements.Add(new AssignmentStatement
						{
							Target = assignmentStatement.Target,
							Value = new BinOpExpression
							{
								Left = assignmentStatement.Target,
								Operation = binOpExpression.Operation,
								Right = binOpExpression.Right
							}
						});
						return true;
					}
				}
			}
			block.Statements.Add(st);
			return false;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002E38 File Offset: 0x00001038
		public static void Run(StatementBlock block)
		{
			bool flag;
			do
			{
				flag = false;
				Statement[] array = block.Statements.ToArray<Statement>();
				block.Statements.Clear();
				foreach (Statement st in array)
				{
					flag |= ExpansionTransform.ProcessStatement(st, block);
				}
			}
			while (flag);
		}
	}
}
