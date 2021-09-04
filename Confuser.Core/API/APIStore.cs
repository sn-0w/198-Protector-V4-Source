using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using dnlib.DotNet;

namespace Confuser.Core.API
{
	// Token: 0x020000C3 RID: 195
	internal class APIStore : IAPIStore
	{
		// Token: 0x0600046F RID: 1135 RVA: 0x0001A6B4 File Offset: 0x000188B4
		public APIStore(ConfuserContext context)
		{
			this.context = context;
			this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("APIStore");
			this.dataStores = new SortedList<int, List<IDataStore>>();
			this.predicates = new List<IOpaquePredicateDescriptor>();
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00003CA5 File Offset: 0x00001EA5
		public void AddStore(IDataStore dataStore)
		{
			this.dataStores.AddListEntry(dataStore.Priority, dataStore);
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x00003CBB File Offset: 0x00001EBB
		public void AddPredicate(IOpaquePredicateDescriptor predicate)
		{
			this.predicates.Add(predicate);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001A704 File Offset: 0x00018904
		public IDataStore GetStore(MethodDef method)
		{
			for (int i = this.dataStores.Count - 1; i >= 0; i--)
			{
				List<IDataStore> list = this.dataStores[i];
				for (int j = list.Count - 1; j >= 0; j--)
				{
					bool flag = list[j].IsUsable(method);
					if (flag)
					{
						return list[j];
					}
				}
			}
			return null;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001A784 File Offset: 0x00018984
		public IOpaquePredicateDescriptor GetPredicate(MethodDef method, OpaquePredicateType? type, params int[] argCount)
		{
			IOpaquePredicateDescriptor[] array = this.predicates.ToArray();
			this.random.Shuffle<IOpaquePredicateDescriptor>(array);
			foreach (IOpaquePredicateDescriptor opaquePredicateDescriptor in array)
			{
				bool flag = opaquePredicateDescriptor.IsUsable(method) && (type == null || opaquePredicateDescriptor.Type == type.Value) && (argCount == null || Array.IndexOf<int>(argCount, opaquePredicateDescriptor.ArgumentCount) != -1);
				if (flag)
				{
					return opaquePredicateDescriptor;
				}
			}
			return null;
		}

		// Token: 0x040002D2 RID: 722
		private readonly ConfuserContext context;

		// Token: 0x040002D3 RID: 723
		private readonly RandomGenerator random;

		// Token: 0x040002D4 RID: 724
		private readonly SortedList<int, List<IDataStore>> dataStores;

		// Token: 0x040002D5 RID: 725
		private readonly List<IOpaquePredicateDescriptor> predicates;
	}
}
