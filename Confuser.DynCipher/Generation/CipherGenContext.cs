using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000012 RID: 18
	internal class CipherGenContext
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00003D50 File Offset: 0x00001F50
		public CipherGenContext(RandomGenerator random, int dataVarCount)
		{
			this.random = random;
			this.Block = new StatementBlock();
			this.dataVars = new Variable[dataVarCount];
			for (int i = 0; i < dataVarCount; i++)
			{
				this.dataVars[i] = new Variable("v" + i)
				{
					Tag = i
				};
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002321 File Offset: 0x00000521
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002329 File Offset: 0x00000529
		public StatementBlock Block { get; private set; }

		// Token: 0x06000059 RID: 89 RVA: 0x00003DDC File Offset: 0x00001FDC
		public Expression GetDataExpression(int index)
		{
			return new VariableExpression
			{
				Variable = this.dataVars[index]
			};
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003E04 File Offset: 0x00002004
		public Expression GetKeyExpression(int index)
		{
			return new ArrayIndexExpression
			{
				Array = new VariableExpression
				{
					Variable = this.keyVar
				},
				Index = index
			};
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003E3C File Offset: 0x0000203C
		public CipherGenContext Emit(Statement statement)
		{
			this.Block.Statements.Add(statement);
			return this;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003E64 File Offset: 0x00002064
		public IDisposable AcquireTempVar(out VariableExpression exp)
		{
			bool flag = this.tempVars.Count == 0;
			Variable variable;
			if (flag)
			{
				object arg = "t";
				int num = this.tempVarCounter;
				this.tempVarCounter = num + 1;
				variable = new Variable(arg + num);
			}
			else
			{
				variable = this.tempVars[this.random.NextInt32(this.tempVars.Count)];
				this.tempVars.Remove(variable);
			}
			exp = new VariableExpression
			{
				Variable = variable
			};
			return new CipherGenContext.TempVarHolder(this, variable);
		}

		// Token: 0x0400002A RID: 42
		private readonly Variable[] dataVars;

		// Token: 0x0400002B RID: 43
		private readonly Variable keyVar = new Variable("{KEY}");

		// Token: 0x0400002C RID: 44
		private readonly RandomGenerator random;

		// Token: 0x0400002D RID: 45
		private readonly List<Variable> tempVars = new List<Variable>();

		// Token: 0x0400002E RID: 46
		private int tempVarCounter;

		// Token: 0x02000013 RID: 19
		private struct TempVarHolder : IDisposable
		{
			// Token: 0x0600005D RID: 93 RVA: 0x00002332 File Offset: 0x00000532
			public TempVarHolder(CipherGenContext p, Variable v)
			{
				this.parent = p;
				this.tempVar = v;
			}

			// Token: 0x0600005E RID: 94 RVA: 0x00002343 File Offset: 0x00000543
			public void Dispose()
			{
				this.parent.tempVars.Add(this.tempVar);
			}

			// Token: 0x04000030 RID: 48
			private readonly CipherGenContext parent;

			// Token: 0x04000031 RID: 49
			private readonly Variable tempVar;
		}
	}
}
