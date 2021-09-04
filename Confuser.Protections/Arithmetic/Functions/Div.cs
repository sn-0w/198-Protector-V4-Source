using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions
{
	// Token: 0x020000E1 RID: 225
	public class Div : iFunction
	{
		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060003BB RID: 955 RVA: 0x000021DB File Offset: 0x000003DB
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Div;
			}
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00019A1C File Offset: 0x00017C1C
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
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Div), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
