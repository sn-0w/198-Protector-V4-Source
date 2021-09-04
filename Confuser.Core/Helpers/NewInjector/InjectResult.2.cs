using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using dnlib.DotNet;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000BC RID: 188
	public sealed class InjectResult<T> : IEnumerable<ValueTuple<IMemberDef, IMemberDef>>, IEnumerable where T : IMemberDef
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00003B4F File Offset: 0x00001D4F
		[TupleElementNames(new string[]
		{
			"Source",
			"Mapped"
		})]
		public ValueTuple<T, T> Requested { [return: TupleElementNames(new string[]
		{
			"Source",
			"Mapped"
		})] get; }

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00003B57 File Offset: 0x00001D57
		[TupleElementNames(new string[]
		{
			"Source",
			"Mapped"
		})]
		public IReadOnlyCollection<ValueTuple<IMemberDef, IMemberDef>> InjectedDependencies { [return: TupleElementNames(new string[]
		{
			"Source",
			"Mapped"
		})] get; }

		// Token: 0x06000447 RID: 1095 RVA: 0x00003B5F File Offset: 0x00001D5F
		internal InjectResult(T source, T mapped, IReadOnlyCollection<ValueTuple<IMemberDef, IMemberDef>> dependencies)
		{
			this.Requested = new ValueTuple<T, T>(source, mapped);
			this.InjectedDependencies = dependencies;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00003B7D File Offset: 0x00001D7D
		private IEnumerable<ValueTuple<IMemberDef, IMemberDef>> GetAllMembers()
		{
			ValueTuple<T, T> requested = this.Requested;
			yield return new ValueTuple<IMemberDef, IMemberDef>(requested.Item1, requested.Item2);
			foreach (ValueTuple<IMemberDef, IMemberDef> dep in this.InjectedDependencies)
			{
				yield return dep;
				dep = default(ValueTuple<IMemberDef, IMemberDef>);
			}
			IEnumerator<ValueTuple<IMemberDef, IMemberDef>> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x00003B8D File Offset: 0x00001D8D
		[return: TupleElementNames(new string[]
		{
			"Source",
			"Mapped"
		})]
		IEnumerator<ValueTuple<IMemberDef, IMemberDef>> IEnumerable<ValueTuple<IMemberDef, IMemberDef>>.GetEnumerator()
		{
			return this.GetAllMembers().GetEnumerator();
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00003B8D File Offset: 0x00001D8D
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetAllMembers().GetEnumerator();
		}
	}
}
