using System;
using System.Collections.Generic;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000C0 RID: 192
	// (Invoke) Token: 0x06000459 RID: 1113
	public delegate IReadOnlyList<Instruction> PlaceholderProcessor(ModuleDef module, MethodDef method, IReadOnlyList<Instruction> arguments);
}
