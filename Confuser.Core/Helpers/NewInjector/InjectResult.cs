using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000BA RID: 186
	internal static class InjectResult
	{
		// Token: 0x06000440 RID: 1088 RVA: 0x00003B1B File Offset: 0x00001D1B
		internal static InjectResult<T> Create<T>(T source, T mapped) where T : IMemberDef
		{
			return new InjectResult<T>(source, mapped, ImmutableArray.Create<ValueTuple<IMemberDef, IMemberDef>>());
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00019D8C File Offset: 0x00017F8C
		internal static InjectResult<T> Create<T>(T source, T mapped, IEnumerable<KeyValuePair<IMemberDef, IMemberDef>> dependencies) where T : IMemberDef
		{
			return new InjectResult<T>(source, mapped, ImmutableList.ToImmutableList<ValueTuple<IMemberDef, IMemberDef>>(from kvp in dependencies
			select new ValueTuple<IMemberDef, IMemberDef>(kvp.Key, kvp.Value)));
		}
	}
}
