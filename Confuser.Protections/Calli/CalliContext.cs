using System;
using dnlib.DotNet;

namespace Confuser.Protections.Calli
{
	// Token: 0x020000CF RID: 207
	internal class CalliContext
	{
		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000032D3 File Offset: 0x000014D3
		// (set) Token: 0x06000373 RID: 883 RVA: 0x000032DB File Offset: 0x000014DB
		internal MethodDef CalliMethod { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000374 RID: 884 RVA: 0x000032E4 File Offset: 0x000014E4
		// (set) Token: 0x06000375 RID: 885 RVA: 0x000032EC File Offset: 0x000014EC
		internal CalliContext.CalliMode Mode { get; set; }

		// Token: 0x040001B6 RID: 438
		internal string[] DontObf = new string[]
		{
			"ToArray",
			"set_foregroundcolor",
			"get_conte",
			"GetTypeFromHandle",
			"TypeFromHandle",
			"GetFunctionPointer",
			"get_value",
			"GetIndex",
			"set_IgnoreProtocal",
			"Split",
			"WithAuthor",
			"Match",
			"ClearAllHeaders",
			"Post",
			"set_IgnoreProtocal",
			"GetChannel",
			"op_Implicit",
			"invoke",
			"get_Task",
			"get_ContentType",
			"ADD",
			"op_Equality",
			"op_Inequality",
			"Contains",
			"FreeHGlobal",
			"get_Module",
			"ResolveMethod",
			".ctor",
			"ReadLine",
			"Dispose",
			"Next",
			"Async",
			"GetAwaiter",
			"SetException",
			"Exception",
			"Enter",
			"ReadLines",
			"UnaryOperation",
			"BinaryOperation",
			"Close",
			"WithTitle",
			"Format",
			"get_Memeber",
			"set_IgnoreProtocallErrors",
			"MoveNext",
			"Getinstances",
			"Build",
			"Serialize",
			"Exists",
			"UseCommandsNext",
			"Delay"
		};

		// Token: 0x020000D0 RID: 208
		internal enum CalliMode
		{
			// Token: 0x040001B8 RID: 440
			Normal,
			// Token: 0x040001B9 RID: 441
			Ldftn
		}
	}
}
