using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confuser.Core
{
	// Token: 0x0200003D RID: 61
	internal class DependencyResolver
	{
		// Token: 0x0600014F RID: 335 RVA: 0x000028EE File Offset: 0x00000AEE
		public DependencyResolver(IEnumerable<Protection> protections)
		{
			this.protections = (from prot in protections
			orderby prot.FullId
			select prot).ToList<Protection>();
		}

		// Token: 0x06000150 RID: 336 RVA: 0x0000C4B4 File Offset: 0x0000A6B4
		public IList<Protection> SortDependency()
		{
			List<DependencyResolver.DependencyGraphEdge> list = new List<DependencyResolver.DependencyGraphEdge>();
			HashSet<Protection> hashSet = new HashSet<Protection>(this.protections);
			Dictionary<string, Protection> id2prot = this.protections.ToDictionary((Protection prot) => prot.FullId, (Protection prot) => prot);
			Func<string, Protection> <>9__2;
			Func<string, Protection> <>9__3;
			foreach (Protection protection in this.protections)
			{
				Type type = protection.GetType();
				BeforeProtectionAttribute beforeProtectionAttribute = type.GetCustomAttributes(typeof(BeforeProtectionAttribute), false).Cast<BeforeProtectionAttribute>().SingleOrDefault<BeforeProtectionAttribute>();
				bool flag = beforeProtectionAttribute != null;
				if (flag)
				{
					IEnumerable<string> ids = beforeProtectionAttribute.Ids;
					Func<string, Protection> selector;
					if ((selector = <>9__2) == null)
					{
						selector = (<>9__2 = ((string id) => id2prot[id]));
					}
					IEnumerable<Protection> enumerable = ids.Select(selector);
					foreach (Protection protection2 in enumerable)
					{
						list.Add(new DependencyResolver.DependencyGraphEdge(protection, protection2));
						hashSet.Remove(protection2);
					}
				}
				AfterProtectionAttribute afterProtectionAttribute = type.GetCustomAttributes(typeof(AfterProtectionAttribute), false).Cast<AfterProtectionAttribute>().SingleOrDefault<AfterProtectionAttribute>();
				bool flag2 = afterProtectionAttribute != null;
				if (flag2)
				{
					IEnumerable<string> ids2 = afterProtectionAttribute.Ids;
					Func<string, Protection> selector2;
					if ((selector2 = <>9__3) == null)
					{
						selector2 = (<>9__3 = ((string id) => id2prot[id]));
					}
					IEnumerable<Protection> enumerable2 = ids2.Select(selector2);
					foreach (Protection from in enumerable2)
					{
						list.Add(new DependencyResolver.DependencyGraphEdge(from, protection));
						hashSet.Remove(protection);
					}
				}
			}
			IEnumerable<Protection> source = this.SortGraph(hashSet, list);
			return source.ToList<Protection>();
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00002928 File Offset: 0x00000B28
		private IEnumerable<Protection> SortGraph(IEnumerable<Protection> roots, IList<DependencyResolver.DependencyGraphEdge> edges)
		{
			Queue<Protection> queue = new Queue<Protection>(from prot in roots
			orderby prot.FullId
			select prot);
			while (queue.Count > 0)
			{
				DependencyResolver.<>c__DisplayClass3_0 CS$<>8__locals1 = new DependencyResolver.<>c__DisplayClass3_0();
				CS$<>8__locals1.root = queue.Dequeue();
				Debug.Assert(!(from edge in edges
				where edge.To == CS$<>8__locals1.root
				select edge).Any<DependencyResolver.DependencyGraphEdge>());
				yield return CS$<>8__locals1.root;
				Func<DependencyResolver.DependencyGraphEdge, bool> predicate;
				if ((predicate = CS$<>8__locals1.<>9__2) == null)
				{
					predicate = (CS$<>8__locals1.<>9__2 = ((DependencyResolver.DependencyGraphEdge edge) => edge.From == CS$<>8__locals1.root));
				}
				using (List<DependencyResolver.DependencyGraphEdge>.Enumerator enumerator = edges.Where(predicate).ToList<DependencyResolver.DependencyGraphEdge>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DependencyResolver.<>c__DisplayClass3_1 CS$<>8__locals2 = new DependencyResolver.<>c__DisplayClass3_1();
						CS$<>8__locals2.edge = enumerator.Current;
						edges.Remove(CS$<>8__locals2.edge);
						bool flag = !edges.Any((DependencyResolver.DependencyGraphEdge e) => e.To == CS$<>8__locals2.edge.To);
						if (flag)
						{
							queue.Enqueue(CS$<>8__locals2.edge.To);
						}
						CS$<>8__locals2 = null;
					}
				}
				List<DependencyResolver.DependencyGraphEdge>.Enumerator enumerator = default(List<DependencyResolver.DependencyGraphEdge>.Enumerator);
				CS$<>8__locals1 = null;
			}
			bool flag2 = edges.Count != 0;
			if (flag2)
			{
				throw new CircularDependencyException(edges[0].From, edges[0].To);
			}
			yield break;
		}

		// Token: 0x04000142 RID: 322
		private readonly List<Protection> protections;

		// Token: 0x0200003E RID: 62
		private class DependencyGraphEdge
		{
			// Token: 0x06000152 RID: 338 RVA: 0x00002946 File Offset: 0x00000B46
			public DependencyGraphEdge(Protection from, Protection to)
			{
				this.From = from;
				this.To = to;
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000153 RID: 339 RVA: 0x00002960 File Offset: 0x00000B60
			// (set) Token: 0x06000154 RID: 340 RVA: 0x00002968 File Offset: 0x00000B68
			public Protection From { get; private set; }

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000155 RID: 341 RVA: 0x00002971 File Offset: 0x00000B71
			// (set) Token: 0x06000156 RID: 342 RVA: 0x00002979 File Offset: 0x00000B79
			public Protection To { get; private set; }
		}
	}
}
