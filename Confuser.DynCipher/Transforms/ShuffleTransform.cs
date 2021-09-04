using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Transforms
{
	// Token: 0x0200000B RID: 11
	internal class ShuffleTransform
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00002172 File Offset: 0x00000372
		private static IEnumerable<Variable> GetVariableUsage(Expression exp)
		{
			bool flag = exp is VariableExpression;
			if (flag)
			{
				yield return ((VariableExpression)exp).Variable;
			}
			else
			{
				bool flag2 = exp is ArrayIndexExpression;
				if (flag2)
				{
					foreach (Variable i in ShuffleTransform.GetVariableUsage(((ArrayIndexExpression)exp).Array))
					{
						yield return i;
						i = null;
					}
					IEnumerator<Variable> enumerator = null;
				}
				else
				{
					bool flag3 = exp is BinOpExpression;
					if (flag3)
					{
						foreach (Variable j in ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Left).Concat(ShuffleTransform.GetVariableUsage(((BinOpExpression)exp).Right)))
						{
							yield return j;
							j = null;
						}
						IEnumerator<Variable> enumerator2 = null;
					}
					else
					{
						bool flag4 = exp is UnaryOpExpression;
						if (flag4)
						{
							foreach (Variable k in ShuffleTransform.GetVariableUsage(((UnaryOpExpression)exp).Value))
							{
								yield return k;
								k = null;
							}
							IEnumerator<Variable> enumerator3 = null;
						}
					}
				}
			}
			yield break;
			yield break;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002182 File Offset: 0x00000382
		private static IEnumerable<Variable> GetVariableUsage(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				foreach (Variable i in ShuffleTransform.GetVariableUsage(((AssignmentStatement)st).Value))
				{
					yield return i;
					i = null;
				}
				IEnumerator<Variable> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002192 File Offset: 0x00000392
		private static IEnumerable<Variable> GetVariableDefinition(Expression exp)
		{
			bool flag = exp is VariableExpression;
			if (flag)
			{
				yield return ((VariableExpression)exp).Variable;
			}
			yield break;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000021A2 File Offset: 0x000003A2
		private static IEnumerable<Variable> GetVariableDefinition(Statement st)
		{
			bool flag = st is AssignmentStatement;
			if (flag)
			{
				foreach (Variable i in ShuffleTransform.GetVariableDefinition(((AssignmentStatement)st).Target))
				{
					yield return i;
					i = null;
				}
				IEnumerator<Variable> enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00003384 File Offset: 0x00001584
		private static int SearchUpwardKill(ShuffleTransform.TransformContext context, Statement st, StatementBlock block, int startIndex)
		{
			Variable[] second = context.Usages[st];
			Variable[] second2 = context.Definitions[st];
			for (int i = startIndex - 1; i >= 0; i--)
			{
				bool flag = context.Usages[block.Statements[i]].Intersect(second2).Count<Variable>() > 0 || context.Definitions[block.Statements[i]].Intersect(second).Count<Variable>() > 0;
				if (flag)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003424 File Offset: 0x00001624
		private static int SearchDownwardKill(ShuffleTransform.TransformContext context, Statement st, StatementBlock block, int startIndex)
		{
			Variable[] second = context.Usages[st];
			Variable[] second2 = context.Definitions[st];
			for (int i = startIndex + 1; i < block.Statements.Count; i++)
			{
				bool flag = context.Usages[block.Statements[i]].Intersect(second2).Count<Variable>() > 0 || context.Definitions[block.Statements[i]].Intersect(second).Count<Variable>() > 0;
				if (flag)
				{
					return i;
				}
			}
			return block.Statements.Count - 1;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000034D8 File Offset: 0x000016D8
		public static void Run(StatementBlock block, RandomGenerator random)
		{
			ShuffleTransform.TransformContext transformContext = new ShuffleTransform.TransformContext();
			transformContext.Statements = block.Statements.ToArray<Statement>();
			transformContext.Usages = block.Statements.ToDictionary((Statement s) => s, (Statement s) => ShuffleTransform.GetVariableUsage(s).ToArray<Variable>());
			transformContext.Definitions = block.Statements.ToDictionary((Statement s) => s, (Statement s) => ShuffleTransform.GetVariableDefinition(s).ToArray<Variable>());
			ShuffleTransform.TransformContext transformContext2 = transformContext;
			for (int i = 0; i < 20; i++)
			{
				foreach (Statement statement in transformContext2.Statements)
				{
					int num = block.Statements.IndexOf(statement);
					Variable[] array = ShuffleTransform.GetVariableUsage(statement).Concat(ShuffleTransform.GetVariableDefinition(statement)).ToArray<Variable>();
					int num2 = ShuffleTransform.SearchUpwardKill(transformContext2, statement, block, num);
					int num3 = ShuffleTransform.SearchDownwardKill(transformContext2, statement, block, num);
					int num4 = num2 + random.NextInt32(1, num3 - num2);
					bool flag = num4 > num;
					if (flag)
					{
						num4--;
					}
					block.Statements.RemoveAt(num);
					block.Statements.Insert(num4, statement);
				}
			}
		}

		// Token: 0x04000003 RID: 3
		private const int ITERATION = 20;

		// Token: 0x0200000C RID: 12
		private class TransformContext
		{
			// Token: 0x04000004 RID: 4
			public Dictionary<Statement, Variable[]> Definitions;

			// Token: 0x04000005 RID: 5
			public Statement[] Statements;

			// Token: 0x04000006 RID: 6
			public Dictionary<Statement, Variable[]> Usages;
		}
	}
}
