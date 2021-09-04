using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000091 RID: 145
	internal class ExpressionPredicate : IPredicate
	{
		// Token: 0x0600028F RID: 655 RVA: 0x00002F7B File Offset: 0x0000117B
		public ExpressionPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00010664 File Offset: 0x0000E864
		public void Init(CilBody body)
		{
			if (!this.inited)
			{
				this.stateVar = new Local(this.ctx.Method.Module.CorLibTypes.Int32);
				body.Variables.Add(this.stateVar);
				body.InitLocals = true;
				this.Compile(body);
				this.inited = true;
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x000106C8 File Offset: 0x0000E8C8
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Stloc, this.stateVar));
			foreach (Instruction instruction in this.invCompiled)
			{
				instrs.Add(instruction.Clone());
			}
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00002F8A File Offset: 0x0000118A
		public int GetSwitchKey(int key)
		{
			return this.expCompiled(key);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00010738 File Offset: 0x0000E938
		private void Compile(CilBody body)
		{
			Variable variable = new Variable("{VAR}");
			Variable variable2 = new Variable("{RESULT}");
			this.ctx.DynCipher.GenerateExpressionPair(this.ctx.Random, new VariableExpression
			{
				Variable = variable
			}, new VariableExpression
			{
				Variable = variable2
			}, this.ctx.Depth, out this.expression, out this.inverse);
			this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(this.expression).Compile<Func<int, int>>();
			this.invCompiled = new List<Instruction>();
			new ExpressionPredicate.CodeGen(this.stateVar, this.ctx, this.invCompiled).GenerateCIL(this.inverse);
			body.MaxStack += (ushort)this.ctx.Depth;
		}

		// Token: 0x04000101 RID: 257
		private readonly CFContext ctx;

		// Token: 0x04000102 RID: 258
		private Func<int, int> expCompiled;

		// Token: 0x04000103 RID: 259
		private Expression expression;

		// Token: 0x04000104 RID: 260
		private bool inited;

		// Token: 0x04000105 RID: 261
		private List<Instruction> invCompiled;

		// Token: 0x04000106 RID: 262
		private Expression inverse;

		// Token: 0x04000107 RID: 263
		private Local stateVar;

		// Token: 0x02000092 RID: 146
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000294 RID: 660 RVA: 0x00002F98 File Offset: 0x00001198
			public CodeGen(Local state, CFContext ctx, IList<Instruction> instrs) : base(ctx.Method, instrs)
			{
				this.state = state;
			}

			// Token: 0x06000295 RID: 661 RVA: 0x00002FAE File Offset: 0x000011AE
			protected override Local Var(Variable var)
			{
				if (!(var.Name != "{RESULT}"))
				{
					return this.state;
				}
				return base.Var(var);
			}

			// Token: 0x04000108 RID: 264
			private readonly Local state;
		}
	}
}
