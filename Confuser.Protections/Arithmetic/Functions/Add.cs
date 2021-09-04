using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions
{
	// Token: 0x020000E0 RID: 224
	public class Add : iFunction
	{
		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x0000257A File Offset: 0x0000077A
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Add;
			}
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x000199AC File Offset: 0x00017BAC
		public override ArithmeticVT Arithmetic(Instruction instruction, ModuleDef module)
		{
			Generator generator = new Generator();
			bool flag = !ArithmeticUtils.CheckArithmetic(instruction);
			ArithmeticVT result;
			if (flag)
			{
				result = null;
			}
			else
			{
				ArithmeticEmulator arithmeticEmulator = new ArithmeticEmulator((double)instruction.GetLdcI4Value(), ArithmeticUtils.GetY((double)instruction.GetLdcI4Value()), this.ArithmeticTypes);
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Add), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
