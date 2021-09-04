using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000016 RID: 22
	public class DMCodeGen
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004740 File Offset: 0x00002940
		public DMCodeGen(Type returnType, Tuple<string, Type>[] parameters)
		{
			this.dm = new DynamicMethod("", returnType, (from param in parameters
			select param.Item2).ToArray<Type>(), true);
			this.paramMap = new Dictionary<string, int>();
			for (int i = 0; i < parameters.Length; i++)
			{
				this.paramMap.Add(parameters[i].Item1, i);
			}
			this.ilGen = this.dm.GetILGenerator();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000047E0 File Offset: 0x000029E0
		protected virtual LocalBuilder Var(Variable var)
		{
			LocalBuilder localBuilder;
			bool flag = !this.localMap.TryGetValue(var.Name, out localBuilder);
			if (flag)
			{
				localBuilder = this.ilGen.DeclareLocal(typeof(int));
				this.localMap[var.Name] = localBuilder;
			}
			return localBuilder;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004838 File Offset: 0x00002A38
		protected virtual void LoadVar(Variable var)
		{
			bool flag = this.paramMap.ContainsKey(var.Name);
			if (flag)
			{
				this.ilGen.Emit(OpCodes.Ldarg, this.paramMap[var.Name]);
			}
			else
			{
				this.ilGen.Emit(OpCodes.Ldloc, this.Var(var));
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004898 File Offset: 0x00002A98
		protected virtual void StoreVar(Variable var)
		{
			bool flag = this.paramMap.ContainsKey(var.Name);
			if (flag)
			{
				this.ilGen.Emit(OpCodes.Starg, this.paramMap[var.Name]);
			}
			else
			{
				this.ilGen.Emit(OpCodes.Stloc, this.Var(var));
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000048F8 File Offset: 0x00002AF8
		public T Compile<T>()
		{
			this.ilGen.Emit(OpCodes.Ret);
			return (T)((object)this.dm.CreateDelegate(typeof(T)));
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004938 File Offset: 0x00002B38
		public DMCodeGen GenerateCIL(Expression expression)
		{
			this.EmitLoad(expression);
			return this;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004954 File Offset: 0x00002B54
		public DMCodeGen GenerateCIL(Statement statement)
		{
			this.EmitStatement(statement);
			return this;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004970 File Offset: 0x00002B70
		private void EmitLoad(Expression exp)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrayIndexExpression = (ArrayIndexExpression)exp;
				this.EmitLoad(arrayIndexExpression.Array);
				this.ilGen.Emit(OpCodes.Ldc_I4, arrayIndexExpression.Index);
				this.ilGen.Emit(OpCodes.Ldelem_U4);
			}
			else
			{
				bool flag2 = exp is BinOpExpression;
				if (flag2)
				{
					BinOpExpression binOpExpression = (BinOpExpression)exp;
					this.EmitLoad(binOpExpression.Left);
					this.EmitLoad(binOpExpression.Right);
					OpCode opcode;
					switch (binOpExpression.Operation)
					{
					case BinOps.Add:
						opcode = OpCodes.Add;
						break;
					case BinOps.Sub:
						opcode = OpCodes.Sub;
						break;
					case BinOps.Div:
						opcode = OpCodes.Div;
						break;
					case BinOps.Mul:
						opcode = OpCodes.Mul;
						break;
					case BinOps.Or:
						opcode = OpCodes.Or;
						break;
					case BinOps.And:
						opcode = OpCodes.And;
						break;
					case BinOps.Xor:
						opcode = OpCodes.Xor;
						break;
					case BinOps.Lsh:
						opcode = OpCodes.Shl;
						break;
					case BinOps.Rsh:
						opcode = OpCodes.Shr_Un;
						break;
					default:
						throw new NotSupportedException();
					}
					this.ilGen.Emit(opcode);
				}
				else
				{
					bool flag3 = exp is UnaryOpExpression;
					if (flag3)
					{
						UnaryOpExpression unaryOpExpression = (UnaryOpExpression)exp;
						this.EmitLoad(unaryOpExpression.Value);
						UnaryOps operation = unaryOpExpression.Operation;
						UnaryOps unaryOps = operation;
						OpCode opcode2;
						if (unaryOps != UnaryOps.Not)
						{
							if (unaryOps != UnaryOps.Negate)
							{
								throw new NotSupportedException();
							}
							opcode2 = OpCodes.Neg;
						}
						else
						{
							opcode2 = OpCodes.Not;
						}
						this.ilGen.Emit(opcode2);
					}
					else
					{
						bool flag4 = exp is LiteralExpression;
						if (flag4)
						{
							LiteralExpression literalExpression = (LiteralExpression)exp;
							this.ilGen.Emit(OpCodes.Ldc_I4, (int)literalExpression.Value);
						}
						else
						{
							bool flag5 = exp is VariableExpression;
							if (!flag5)
							{
								throw new NotSupportedException();
							}
							VariableExpression variableExpression = (VariableExpression)exp;
							this.LoadVar(variableExpression.Variable);
						}
					}
				}
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004B70 File Offset: 0x00002D70
		private void EmitStore(Expression exp, Expression value)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrayIndexExpression = (ArrayIndexExpression)exp;
				this.EmitLoad(arrayIndexExpression.Array);
				this.ilGen.Emit(OpCodes.Ldc_I4, arrayIndexExpression.Index);
				this.EmitLoad(value);
				this.ilGen.Emit(OpCodes.Stelem_I4);
			}
			else
			{
				bool flag2 = exp is VariableExpression;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				VariableExpression variableExpression = (VariableExpression)exp;
				this.EmitLoad(value);
				this.StoreVar(variableExpression.Variable);
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004C08 File Offset: 0x00002E08
		private void EmitStatement(Statement statement)
		{
			bool flag = statement is AssignmentStatement;
			if (flag)
			{
				AssignmentStatement assignmentStatement = (AssignmentStatement)statement;
				this.EmitStore(assignmentStatement.Target, assignmentStatement.Value);
			}
			else
			{
				bool flag2 = statement is LoopStatement;
				if (flag2)
				{
					LoopStatement loopStatement = (LoopStatement)statement;
					Label label = this.ilGen.DefineLabel();
					Label label2 = this.ilGen.DefineLabel();
					this.ilGen.Emit(OpCodes.Ldc_I4, loopStatement.Begin);
					this.ilGen.Emit(OpCodes.Br, label2);
					this.ilGen.Emit(OpCodes.Ldc_I4, loopStatement.Begin);
					this.ilGen.MarkLabel(label);
					foreach (Statement statement2 in loopStatement.Statements)
					{
						this.EmitStatement(statement2);
					}
					this.ilGen.Emit(OpCodes.Ldc_I4_1);
					this.ilGen.Emit(OpCodes.Add);
					this.ilGen.MarkLabel(label2);
					this.ilGen.Emit(OpCodes.Dup);
					this.ilGen.Emit(OpCodes.Ldc_I4, loopStatement.Limit);
					this.ilGen.Emit(OpCodes.Blt, label);
					this.ilGen.Emit(OpCodes.Pop);
				}
				else
				{
					bool flag3 = statement is StatementBlock;
					if (!flag3)
					{
						throw new NotSupportedException();
					}
					foreach (Statement statement3 in ((StatementBlock)statement).Statements)
					{
						this.EmitStatement(statement3);
					}
				}
			}
		}

		// Token: 0x0400003C RID: 60
		private readonly DynamicMethod dm;

		// Token: 0x0400003D RID: 61
		private readonly ILGenerator ilGen;

		// Token: 0x0400003E RID: 62
		private readonly Dictionary<string, LocalBuilder> localMap = new Dictionary<string, LocalBuilder>();

		// Token: 0x0400003F RID: 63
		private readonly Dictionary<string, int> paramMap;
	}
}
