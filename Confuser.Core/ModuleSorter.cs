using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x02000029 RID: 41
	internal class ModuleSorter
	{
		// Token: 0x060000E1 RID: 225 RVA: 0x000025ED File Offset: 0x000007ED
		public ModuleSorter(IEnumerable<ModuleDefMD> modules)
		{
			this.modules = modules.ToList<ModuleDefMD>();
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00009C5C File Offset: 0x00007E5C
		public IList<ModuleDefMD> Sort()
		{
			List<ModuleSorter.DependencyGraphEdge> list = new List<ModuleSorter.DependencyGraphEdge>();
			HashSet<ModuleDefMD> hashSet = new HashSet<ModuleDefMD>(this.modules);
			Dictionary<IAssembly, List<ModuleDefMD>> dictionary = this.modules.GroupBy((ModuleDefMD module) => module.Assembly.ToAssemblyRef(), AssemblyNameComparer.CompareAll).ToDictionary((IGrouping<IAssembly, ModuleDefMD> gp) => gp.Key, (IGrouping<IAssembly, ModuleDefMD> gp) => gp.ToList<ModuleDefMD>(), AssemblyNameComparer.CompareAll);
			foreach (ModuleDefMD moduleDefMD in this.modules)
			{
				foreach (AssemblyRef key in moduleDefMD.GetAssemblyRefs())
				{
					bool flag = !dictionary.ContainsKey(key);
					if (!flag)
					{
						foreach (ModuleDefMD from in dictionary[key])
						{
							list.Add(new ModuleSorter.DependencyGraphEdge(from, moduleDefMD));
						}
						hashSet.Remove(moduleDefMD);
					}
				}
			}
			List<ModuleDefMD> list2 = this.SortGraph(hashSet, list).ToList<ModuleDefMD>();
			Debug.Assert(list2.Count == this.modules.Count);
			return list2;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00002603 File Offset: 0x00000803
		private IEnumerable<ModuleDefMD> SortGraph(IEnumerable<ModuleDefMD> roots, IList<ModuleSorter.DependencyGraphEdge> edges)
		{
			HashSet<ModuleDefMD> visited = new HashSet<ModuleDefMD>();
			Queue<ModuleDefMD> queue = new Queue<ModuleDefMD>(roots);
			do
			{
				while (queue.Count > 0)
				{
					ModuleSorter.<>c__DisplayClass3_0 CS$<>8__locals1 = new ModuleSorter.<>c__DisplayClass3_0();
					CS$<>8__locals1.node = queue.Dequeue();
					visited.Add(CS$<>8__locals1.node);
					Debug.Assert(!(from edge in edges
					where edge.To == CS$<>8__locals1.node
					select edge).Any<ModuleSorter.DependencyGraphEdge>());
					yield return CS$<>8__locals1.node;
					Func<ModuleSorter.DependencyGraphEdge, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__1) == null)
					{
						predicate = (CS$<>8__locals1.<>9__1 = ((ModuleSorter.DependencyGraphEdge edge) => edge.From == CS$<>8__locals1.node));
					}
					using (List<ModuleSorter.DependencyGraphEdge>.Enumerator enumerator = edges.Where(predicate).ToList<ModuleSorter.DependencyGraphEdge>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ModuleSorter.<>c__DisplayClass3_1 CS$<>8__locals2 = new ModuleSorter.<>c__DisplayClass3_1();
							CS$<>8__locals2.edge = enumerator.Current;
							edges.Remove(CS$<>8__locals2.edge);
							bool flag = !edges.Any((ModuleSorter.DependencyGraphEdge e) => e.To == CS$<>8__locals2.edge.To);
							if (flag)
							{
								queue.Enqueue(CS$<>8__locals2.edge.To);
							}
							CS$<>8__locals2 = null;
						}
					}
					List<ModuleSorter.DependencyGraphEdge>.Enumerator enumerator = default(List<ModuleSorter.DependencyGraphEdge>.Enumerator);
					CS$<>8__locals1 = null;
				}
				bool flag2 = edges.Count > 0;
				if (flag2)
				{
					foreach (ModuleSorter.DependencyGraphEdge edge2 in edges)
					{
						bool flag3 = !visited.Contains(edge2.From);
						if (flag3)
						{
							queue.Enqueue(edge2.From);
							break;
						}
						edge2 = null;
					}
					IEnumerator<ModuleSorter.DependencyGraphEdge> enumerator2 = null;
				}
			}
			while (edges.Count > 0);
			yield break;
		}

		// Token: 0x040000F2 RID: 242
		private readonly List<ModuleDefMD> modules;

		// Token: 0x0200002A RID: 42
		private class DependencyGraphEdge
		{
			// Token: 0x060000E4 RID: 228 RVA: 0x00002621 File Offset: 0x00000821
			public DependencyGraphEdge(ModuleDefMD from, ModuleDefMD to)
			{
				this.From = from;
				this.To = to;
			}

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000263B File Offset: 0x0000083B
			// (set) Token: 0x060000E6 RID: 230 RVA: 0x00002643 File Offset: 0x00000843
			public ModuleDefMD From { get; private set; }

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x060000E7 RID: 231 RVA: 0x0000264C File Offset: 0x0000084C
			// (set) Token: 0x060000E8 RID: 232 RVA: 0x00002654 File Offset: 0x00000854
			public ModuleDefMD To { get; private set; }
		}
	}
}
