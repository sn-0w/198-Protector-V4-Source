using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DB RID: 219
	public abstract class iFunction
	{
		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060003A2 RID: 930
		public abstract ArithmeticTypes ArithmeticTypes { get; }

		// Token: 0x060003A3 RID: 931
		public abstract ArithmeticVT Arithmetic(Instruction instruction, ModuleDef module);
	}
}
