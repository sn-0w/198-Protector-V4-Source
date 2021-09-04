using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions.Maths
{
	// Token: 0x020000EC RID: 236
	public class Sin : iFunction
	{
		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060003DC RID: 988 RVA: 0x00003502 File Offset: 0x00001702
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Sin;
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00019BD8 File Offset: 0x00017DD8
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
