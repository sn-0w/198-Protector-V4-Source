using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000093 RID: 147
	internal interface IPredicate
	{
		// Token: 0x06000296 RID: 662
		void Init(CilBody body);

		// Token: 0x06000297 RID: 663
		void EmitSwitchLoad(IList<Instruction> instrs);

		// Token: 0x06000298 RID: 664
		int GetSwitchKey(int key);
	}
}
