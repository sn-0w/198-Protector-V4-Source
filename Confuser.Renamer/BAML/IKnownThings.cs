using System;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000063 RID: 99
	internal interface IKnownThings
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000276 RID: 630
		Func<KnownTypes, TypeDef> Types { get; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000277 RID: 631
		Func<KnownProperties, Tuple<KnownTypes, PropertyDef, TypeDef>> Properties { get; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000278 RID: 632
		AssemblyDef FrameworkAssembly { get; }
	}
}
