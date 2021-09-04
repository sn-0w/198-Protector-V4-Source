using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions
{
	// Token: 0x020000E3 RID: 227
	public class Sub : iFunction
	{
		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060003C1 RID: 961 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Sub;
			}
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00019AFC File Offset: 0x00017CFC
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
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Sub), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
