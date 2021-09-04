using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions.Maths
{
	// Token: 0x020000EE RID: 238
	public class Tan : iFunction
	{
		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060003E2 RID: 994 RVA: 0x00003509 File Offset: 0x00001709
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Tan;
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00019BD8 File Offset: 0x00017DD8
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
