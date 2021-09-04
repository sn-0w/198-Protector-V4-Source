using System;
using dnlib.DotNet.Emit;

namespace Confuser.Core.API
{
	// Token: 0x020000C8 RID: 200
	public interface IOpaquePredicate
	{
		// Token: 0x06000481 RID: 1153
		Instruction[] Emit(Func<Instruction[]> loadArg);

		// Token: 0x06000482 RID: 1154
		uint GetValue(uint[] arg);
	}
}
