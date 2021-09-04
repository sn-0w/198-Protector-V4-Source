using System;
using System.Text;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000021 RID: 33
	public class x86Instruction
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000024B2 File Offset: 0x000006B2
		// (set) Token: 0x0600009C RID: 156 RVA: 0x000024BA File Offset: 0x000006BA
		public x86OpCode OpCode { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000024C3 File Offset: 0x000006C3
		// (set) Token: 0x0600009E RID: 158 RVA: 0x000024CB File Offset: 0x000006CB
		public Ix86Operand[] Operands { get; set; }

		// Token: 0x0600009F RID: 159 RVA: 0x0000599C File Offset: 0x00003B9C
		public static x86Instruction Create(x86OpCode opCode, params Ix86Operand[] operands)
		{
			return new x86Instruction
			{
				OpCode = opCode,
				Operands = operands
			};
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x000059C8 File Offset: 0x00003BC8
		public byte[] Assemble()
		{
			switch (this.OpCode)
			{
			case x86OpCode.MOV:
			{
				bool flag = this.Operands.Length != 2;
				if (flag)
				{
					throw new InvalidOperationException();
				}
				bool flag2 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag2)
				{
					byte[] array = new byte[]
					{
						137,
						192
					};
					byte[] array2 = array;
					int num = 1;
					array2[num] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array3 = array;
					int num2 = 1;
					array3[num2] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array;
				}
				bool flag3 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag3)
				{
					byte[] array4 = new byte[5];
					array4[0] = 184;
					byte[] array5 = array4;
					int num3 = 0;
					array5[num3] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, array4, 1, 4);
					return array4;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.ADD:
			{
				bool flag4 = this.Operands.Length != 2;
				if (flag4)
				{
					throw new InvalidOperationException();
				}
				bool flag5 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag5)
				{
					byte[] array6 = new byte[]
					{
						1,
						192
					};
					byte[] array7 = array6;
					int num4 = 1;
					array7[num4] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array8 = array6;
					int num5 = 1;
					array8[num5] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array6;
				}
				bool flag6 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag6)
				{
					byte[] array9 = new byte[6];
					array9[0] = 129;
					array9[1] = 192;
					byte[] array10 = array9;
					int num6 = 1;
					array10[num6] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, array9, 2, 4);
					return array9;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.SUB:
			{
				bool flag7 = this.Operands.Length != 2;
				if (flag7)
				{
					throw new InvalidOperationException();
				}
				bool flag8 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag8)
				{
					byte[] array11 = new byte[]
					{
						41,
						192
					};
					byte[] array12 = array11;
					int num7 = 1;
					array12[num7] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array13 = array11;
					int num8 = 1;
					array13[num8] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array11;
				}
				bool flag9 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag9)
				{
					byte[] array14 = new byte[6];
					array14[0] = 129;
					array14[1] = 232;
					byte[] array15 = array14;
					int num9 = 1;
					array15[num9] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, array14, 2, 4);
					return array14;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.IMUL:
			{
				bool flag10 = this.Operands.Length != 2;
				if (flag10)
				{
					throw new InvalidOperationException();
				}
				bool flag11 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag11)
				{
					byte[] array16 = new byte[3];
					array16[0] = 15;
					array16[1] = 175;
					array16[1] = 192;
					byte[] array17 = array16;
					int num10 = 1;
					array17[num10] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array18 = array16;
					int num11 = 1;
					array18[num11] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array16;
				}
				bool flag12 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag12)
				{
					byte[] array19 = new byte[6];
					array19[0] = 105;
					array19[1] = 192;
					byte[] array20 = array19;
					int num12 = 1;
					array20[num12] |= (byte)((this.Operands[0] as x86RegisterOperand).Register << 3);
					byte[] array21 = array19;
					int num13 = 1;
					array21[num13] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, array19, 2, 4);
					return array19;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.NEG:
			{
				bool flag13 = this.Operands.Length != 1;
				if (flag13)
				{
					throw new InvalidOperationException();
				}
				bool flag14 = this.Operands[0] is x86RegisterOperand;
				if (flag14)
				{
					byte[] array22 = new byte[]
					{
						247,
						216
					};
					byte[] array23 = array22;
					int num14 = 1;
					array23[num14] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array22;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.NOT:
			{
				bool flag15 = this.Operands.Length != 1;
				if (flag15)
				{
					throw new InvalidOperationException();
				}
				bool flag16 = this.Operands[0] is x86RegisterOperand;
				if (flag16)
				{
					byte[] array24 = new byte[]
					{
						247,
						208
					};
					byte[] array25 = array24;
					int num15 = 1;
					array25[num15] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array24;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.XOR:
			{
				bool flag17 = this.Operands.Length != 2;
				if (flag17)
				{
					throw new InvalidOperationException();
				}
				bool flag18 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86RegisterOperand;
				if (flag18)
				{
					byte[] array26 = new byte[]
					{
						49,
						192
					};
					byte[] array27 = array26;
					int num16 = 1;
					array27[num16] |= (byte)((this.Operands[1] as x86RegisterOperand).Register << 3);
					byte[] array28 = array26;
					int num17 = 1;
					array28[num17] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array26;
				}
				bool flag19 = this.Operands[0] is x86RegisterOperand && this.Operands[1] is x86ImmediateOperand;
				if (flag19)
				{
					byte[] array29 = new byte[6];
					array29[0] = 129;
					array29[1] = 240;
					byte[] array30 = array29;
					int num18 = 1;
					array30[num18] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					Buffer.BlockCopy(BitConverter.GetBytes((this.Operands[1] as x86ImmediateOperand).Immediate), 0, array29, 2, 4);
					return array29;
				}
				throw new NotSupportedException();
			}
			case x86OpCode.POP:
			{
				bool flag20 = this.Operands.Length != 1;
				if (flag20)
				{
					throw new InvalidOperationException();
				}
				bool flag21 = this.Operands[0] is x86RegisterOperand;
				if (flag21)
				{
					byte[] array31 = new byte[]
					{
						88
					};
					byte[] array32 = array31;
					int num19 = 0;
					array32[num19] |= (byte)(this.Operands[0] as x86RegisterOperand).Register;
					return array31;
				}
				throw new NotSupportedException();
			}
			}
			throw new NotSupportedException();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000061BC File Offset: 0x000043BC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.OpCode);
			for (int i = 0; i < this.Operands.Length; i++)
			{
				stringBuilder.AppendFormat("{0}{1}", (i == 0) ? " " : ", ", this.Operands[i]);
			}
			return stringBuilder.ToString();
		}
	}
}
