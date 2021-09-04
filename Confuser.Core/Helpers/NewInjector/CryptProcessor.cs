using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000C1 RID: 193
	// (Invoke) Token: 0x0600045D RID: 1117
	public delegate IReadOnlyList<Instruction> CryptProcessor(ModuleDef module, MethodDef method, Local block, Local key);
}
