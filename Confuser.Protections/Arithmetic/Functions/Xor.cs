using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic.Functions
{
	// Token: 0x020000E4 RID: 228
	public class Xor : iFunction
	{
		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ArithmeticTypes ArithmeticTypes
		{
			get
			{
				return ArithmeticTypes.Xor;
			}
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x00019B6C File Offset: 0x00017D6C
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
				ArithmeticEmulator arithmeticEmulator = new ArithmeticEmulator((double)instruction.GetLdcI4Value(), (double)generator.Next(), this.ArithmeticTypes);
				result = new ArithmeticVT(new Value(arithmeticEmulator.GetValue(), arithmeticEmulator.GetY()), new Token(OpCodes.Xor), this.ArithmeticTypes);
			}
			return result;
		}
	}
}
