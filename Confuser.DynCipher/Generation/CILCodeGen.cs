using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000015 RID: 21
	public class CILCodeGen
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00002384 File Offset: 0x00000584
		public CILCodeGen(MethodDef method, IList<Instruction> instrs)
		{
			this.Method = method;
			this.Instructions = instrs;
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000023A9 File Offset: 0x000005A9
		// (set) Token: 0x06000065 RID: 101 RVA: 0x000023B1 File Offset: 0x000005B1
		public MethodDef Method { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000023BA File Offset: 0x000005BA
		// (set) Token: 0x06000067 RID: 103 RVA: 0x000023C2 File Offset: 0x000005C2
		public IList<Instruction> Instructions { get; private set; }

		// Token: 0x06000068 RID: 104 RVA: 0x000023CB File Offset: 0x000005CB
		protected void Emit(Instruction instr)
		{
			this.Instructions.Add(instr);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004220 File Offset: 0x00002420
		protected virtual Local Var(Variable var)
		{
			Local local;
			bool flag = !this.localMap.TryGetValue(var.Name, out local);
			if (flag)
			{
				local = new Local(this.Method.Module.CorLibTypes.UInt32);
				local.Name = var.Name;
				this.localMap[var.Name] = local;
			}
			return local;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000023DB File Offset: 0x000005DB
		protected virtual void LoadVar(Variable var)
		{
			this.Emit(Instruction.Create(OpCodes.Ldloc, this.Var(var)));
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000023F6 File Offset: 0x000005F6
		protected virtual void StoreVar(Variable var)
		{
			this.Emit(Instruction.Create(OpCodes.Stloc, this.Var(var)));
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000428C File Offset: 0x0000248C
		public void Commit(CilBody body)
		{
			foreach (Local local in this.localMap.Values)
			{
				body.InitLocals = true;
				body.Variables.Add(local);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002411 File Offset: 0x00000611
		public void GenerateCIL(Expression expression)
		{
			this.EmitLoad(expression);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000241C File Offset: 0x0000061C
		public void GenerateCIL(Statement statement)
		{
			this.EmitStatement(statement);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000042F8 File Offset: 0x000024F8
		private void EmitLoad(Expression exp)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrayIndexExpression = (ArrayIndexExpression)exp;
				this.EmitLoad(arrayIndexExpression.Array);
				this.Emit(Instruction.CreateLdcI4(arrayIndexExpression.Index));
				this.Emit(Instruction.Create(OpCodes.Ldelem_U4));
			}
			else
			{
				bool flag2 = exp is BinOpExpression;
				if (flag2)
				{
					BinOpExpression binOpExpression = (BinOpExpression)exp;
					this.EmitLoad(binOpExpression.Left);
					this.EmitLoad(binOpExpression.Right);
					OpCode opCode;
					switch (binOpExpression.Operation)
					{
					case BinOps.Add:
						opCode = OpCodes.Add;
						break;
					case BinOps.Sub:
						opCode = OpCodes.Sub;
						break;
					case BinOps.Div:
						opCode = OpCodes.Div;
						break;
					case BinOps.Mul:
						opCode = OpCodes.Mul;
						break;
					case BinOps.Or:
						opCode = OpCodes.Or;
						break;
					case BinOps.And:
						opCode = OpCodes.And;
						break;
					case BinOps.Xor:
						opCode = OpCodes.Xor;
						break;
					case BinOps.Lsh:
						opCode = OpCodes.Shl;
						break;
					case BinOps.Rsh:
						opCode = OpCodes.Shr_Un;
						break;
					default:
						throw new NotSupportedException();
					}
					this.Emit(Instruction.Create(opCode));
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
						OpCode opCode2;
						if (unaryOps != UnaryOps.Not)
						{
							if (unaryOps != UnaryOps.Negate)
							{
								throw new NotSupportedException();
							}
							opCode2 = OpCodes.Neg;
						}
						else
						{
							opCode2 = OpCodes.Not;
						}
						this.Emit(Instruction.Create(opCode2));
					}
					else
					{
						bool flag4 = exp is LiteralExpression;
						if (flag4)
						{
							LiteralExpression literalExpression = (LiteralExpression)exp;
							this.Emit(Instruction.CreateLdcI4((int)literalExpression.Value));
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

		// Token: 0x06000070 RID: 112 RVA: 0x000044F0 File Offset: 0x000026F0
		private void EmitStore(Expression exp, Expression value)
		{
			bool flag = exp is ArrayIndexExpression;
			if (flag)
			{
				ArrayIndexExpression arrayIndexExpression = (ArrayIndexExpression)exp;
				this.EmitLoad(arrayIndexExpression.Array);
				this.Emit(Instruction.CreateLdcI4(arrayIndexExpression.Index));
				this.EmitLoad(value);
				this.Emit(Instruction.Create(OpCodes.Stelem_I4));
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

		// Token: 0x06000071 RID: 113 RVA: 0x00004584 File Offset: 0x00002784
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
					Instruction instruction = Instruction.Create(OpCodes.Nop);
					Instruction instruction2 = Instruction.Create(OpCodes.Dup);
					this.Emit(Instruction.CreateLdcI4(loopStatement.Begin));
					this.Emit(Instruction.Create(OpCodes.Br, instruction2));
					this.Emit(Instruction.CreateLdcI4(loopStatement.Begin));
					this.Emit(instruction);
					foreach (Statement statement2 in loopStatement.Statements)
					{
						this.EmitStatement(statement2);
					}
					this.Emit(Instruction.CreateLdcI4(1));
					this.Emit(Instruction.Create(OpCodes.Add));
					this.Emit(instruction2);
					this.Emit(Instruction.CreateLdcI4(loopStatement.Limit));
					this.Emit(Instruction.Create(OpCodes.Blt, instruction));
					this.Emit(Instruction.Create(OpCodes.Pop));
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

		// Token: 0x04000039 RID: 57
		private readonly Dictionary<string, Local> localMap = new Dictionary<string, Local>();
	}
}
