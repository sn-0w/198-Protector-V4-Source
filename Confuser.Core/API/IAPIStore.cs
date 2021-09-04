using System;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000C4 RID: 196
	public interface IAPIStore
	{
		// Token: 0x06000474 RID: 1140
		void AddStore(IDataStore dataStore);

		// Token: 0x06000475 RID: 1141
		IDataStore GetStore(MethodDef method);

		// Token: 0x06000476 RID: 1142
		void AddPredicate(IOpaquePredicateDescriptor predicate);

		// Token: 0x06000477 RID: 1143
		IOpaquePredicateDescriptor GetPredicate(MethodDef method, OpaquePredicateType? type, params int[] argCount);
	}
}
