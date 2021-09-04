using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions
{
	// Token: 0x020000E2 RID: 226
	public class Mul : iFunction
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Mul;
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00019A8C File Offset: 0x00017C8C
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
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Mul), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
