using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200001A RID: 26
	public class x86CodeGen
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000052CC File Offset: 0x000034CC
		public IList<x86Instruction> Instructions
		{
			get
			{
				return this.instrs;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000086 RID: 134 RVA: 0x0000243B File Offset: 0x0000063B
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00002443 File Offset: 0x00000643
		public int MaxUsedRegister { get; private set; }

		// Token: 0x06000088 RID: 136 RVA: 0x000052E4 File Offset: 0x000034E4
		public x86Register? GenerateX86(Expression expression, Func<Variable, x86Register, IEnumerable<x86Instruction>> loadArg)
		{
			this.instrs = new List<x86Instruction>();
			this.usedRegs = new bool[8];
			this.MaxUsedRegister = -1;
			this.usedRegs[5] = true;
			this.usedRegs[4] = true;
			x86Register? result;
			try
			{
				result = new x86Register?(((x86RegisterOperand)this.Emit(expression, loadArg)).Register);
			}
			catch (Exception ex)
			{
				bool flag = ex.Message == "Register overflowed.";
				if (!flag)
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005374 File Offset: 0x00003574
		private x86Register GetFreeRegister()
		{
			for (int i = 0; i < 8; i++)
			{
				bool flag = !this.usedRegs[i];
				if (flag)
				{
					return (x86Register)i;
				}
			}
			throw new Exception("Register overflowed.");
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000053B4 File Offset: 0x000035B4
		private void TakeRegister(x86Register reg)
		{
			this.usedRegs[(int)reg] = true;
			bool flag = reg > (x86Register)this.MaxUsedRegister;
			if (flag)
			{
				this.MaxUsedRegister = (int)reg;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000244C File Offset: 0x0000064C
		private void ReleaseRegister(x86Register reg)
		{
			this.usedRegs[(int)reg] = false;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000053E0 File Offset: 0x000035E0
		private x86Register Normalize(x86Instruction instr)
		{
			bool flag = instr.Operands.Length == 2 && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86ImmediateOperand;
			x86Register result;
			if (flag)
			{
				x86Register freeRegister = this.GetFreeRegister();
				this.instrs.Add(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
				{
					new x86RegisterOperand(freeRegister),
					instr.Operands[0]
				}));
				instr.Operands[0] = new x86RegisterOperand(freeRegister);
				this.instrs.Add(instr);
				result = freeRegister;
			}
			else
			{
				bool flag2 = instr.Operands.Length == 1 && instr.Operands[0] is x86ImmediateOperand;
				if (flag2)
				{
					x86Register freeRegister2 = this.GetFreeRegister();
					this.instrs.Add(x86Instruction.Create(x86OpCode.MOV, new Ix86Operand[]
					{
						new x86RegisterOperand(freeRegister2),
						instr.Operands[0]
					}));
					instr.Operands[0] = new x86RegisterOperand(freeRegister2);
					this.instrs.Add(instr);
					result = freeRegister2;
				}
				else
				{
					bool flag3 = instr.OpCode == x86OpCode.SUB && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86RegisterOperand;
					if (flag3)
					{
						x86Register register = ((x86RegisterOperand)instr.Operands[1]).Register;
						this.instrs.Add(x86Instruction.Create(x86OpCode.NEG, new Ix86Operand[]
						{
							new x86RegisterOperand(register)
						}));
						instr.OpCode = x86OpCode.ADD;
						instr.Operands[1] = instr.Operands[0];
						instr.Operands[0] = new x86RegisterOperand(register);
						this.instrs.Add(instr);
						result = register;
					}
					else
					{
						bool flag4 = instr.Operands.Length == 2 && instr.Operands[0] is x86ImmediateOperand && instr.Operands[1] is x86RegisterOperand;
						if (flag4)
						{
							x86Register register2 = ((x86RegisterOperand)instr.Operands[1]).Register;
							instr.Operands[1] = instr.Operands[0];
							instr.Operands[0] = new x86RegisterOperand(register2);
							this.instrs.Add(instr);
							result = register2;
						}
						else
						{
							Debug.Assert(instr.Operands.Length != 0);
							Debug.Assert(instr.Operands[0] is x86RegisterOperand);
							bool flag5 = instr.Operands.Length == 2 && instr.Operands[1] is x86RegisterOperand;
							if (flag5)
							{
								this.ReleaseRegister(((x86RegisterOperand)instr.Operands[1]).Register);
							}
							this.instrs.Add(instr);
							result = ((x86RegisterOperand)instr.Operands[0]).Register;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005698 File Offset: 0x00003898
		private Ix86Operand Emit(Expression exp, Func<Variable, x86Register, IEnumerable<x86Instruction>> loadArg)
		{
			bool flag = exp is BinOpExpression;
			Ix86Operand result;
			if (flag)
			{
				BinOpExpression binOpExpression = (BinOpExpression)exp;
				x86Register reg;
				switch (binOpExpression.Operation)
				{
				case BinOps.Add:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.ADD, new Ix86Operand[]
					{
						this.Emit(binOpExpression.Left, loadArg),
						this.Emit(binOpExpression.Right, loadArg)
					}));
					goto IL_128;
				case BinOps.Sub:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.SUB, new Ix86Operand[]
					{
						this.Emit(binOpExpression.Left, loadArg),
						this.Emit(binOpExpression.Right, loadArg)
					}));
					goto IL_128;
				case BinOps.Mul:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.IMUL, new Ix86Operand[]
					{
						this.Emit(binOpExpression.Left, loadArg),
						this.Emit(binOpExpression.Right, loadArg)
					}));
					goto IL_128;
				case BinOps.Xor:
					reg = this.Normalize(x86Instruction.Create(x86OpCode.XOR, new Ix86Operand[]
					{
						this.Emit(binOpExpression.Left, loadArg),
						this.Emit(binOpExpression.Right, loadArg)
					}));
					goto IL_128;
				}
				throw new NotSupportedException();
				IL_128:
				this.TakeRegister(reg);
				result = new x86RegisterOperand(reg);
			}
			else
			{
				bool flag2 = exp is UnaryOpExpression;
				if (flag2)
				{
					UnaryOpExpression unaryOpExpression = (UnaryOpExpression)exp;
					UnaryOps operation = unaryOpExpression.Operation;
					UnaryOps unaryOps = operation;
					x86Register reg2;
					if (unaryOps != UnaryOps.Not)
					{
						if (unaryOps != UnaryOps.Negate)
						{
							throw new NotSupportedException();
						}
						reg2 = this.Normalize(x86Instruction.Create(x86OpCode.NEG, new Ix86Operand[]
						{
							this.Emit(unaryOpExpression.Value, loadArg)
						}));
					}
					else
					{
						reg2 = this.Normalize(x86Instruction.Create(x86OpCode.NOT, new Ix86Operand[]
						{
							this.Emit(unaryOpExpression.Value, loadArg)
						}));
					}
					this.TakeRegister(reg2);
					result = new x86RegisterOperand(reg2);
				}
				else
				{
					bool flag3 = exp is LiteralExpression;
					if (flag3)
					{
						result = new x86ImmediateOperand((int)((LiteralExpression)exp).Value);
					}
					else
					{
						bool flag4 = exp is VariableExpression;
						if (!flag4)
						{
							throw new NotSupportedException();
						}
						x86Register freeRegister = this.GetFreeRegister();
						this.TakeRegister(freeRegister);
						this.instrs.AddRange(loadArg(((VariableExpression)exp).Variable, freeRegister));
						result = new x86RegisterOperand(freeRegister);
					}
				}
			}
			return result;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000058F8 File Offset: 0x00003AF8
		public override string ToString()
		{
			return string.Join("\r\n", (from instr in this.instrs
			select instr.ToString()).ToArray<string>());
		}

		// Token: 0x04000049 RID: 73
		private List<x86Instruction> instrs;

		// Token: 0x0400004A RID: 74
		private bool[] usedRegs;
	}
}
