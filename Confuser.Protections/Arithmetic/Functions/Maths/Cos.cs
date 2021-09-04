using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions.Maths
{
	// Token: 0x020000E7 RID: 231
	public class Cos : iFunction
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060003CD RID: 973 RVA: 0x000034F0 File Offset: 0x000016F0
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Cos;
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00019BD8 File Offset: 0x00017DD8
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
				List<ArithmeticTypes> arithmetics = new List<ArithmeticTypes>
				{
					ArithmeticTypes.Add,
					ArithmeticTypes.Sub
				};
				ArithmeticEmulator arithmeticEmulator = new ArithmeticEmulator((double)instruction.GetLdcI4Value(), ArithmeticUtils.GetY((double)instruction.GetLdcI4Value()), this.ArithmeticTypes);
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(arithmetics), arithmeticEmulator.GetY()), new Token(ArithmeticUtils.GetOpCode(arithmeticEmulator.GetType), module.Import(ArithmeticUtils.GetMethod(this.ArithmeticTypes))), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
