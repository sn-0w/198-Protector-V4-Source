using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Confuser.Core
{
	// Token: 0x02000025 RID: 37
	public class Annotations
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x000097F8 File Offset: 0x000079F8
		public TValue Get<TValue>(object obj, object key, TValue defValue = default(TValue))
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary listDictionary;
			bool flag3 = !this.annotations.TryGetValue(obj, out listDictionary);
			TValue result;
			if (flag3)
			{
				result = defValue;
			}
			else
			{
				bool flag4 = !listDictionary.Contains(key);
				if (flag4)
				{
					result = defValue;
				}
				else
				{
					Type typeFromHandle = typeof(TValue);
					bool isValueType = typeFromHandle.IsValueType;
					if (isValueType)
					{
						result = (TValue)((object)Convert.ChangeType(listDictionary[key], typeof(TValue)));
					}
					else
					{
						result = (TValue)((object)listDictionary[key]);
					}
				}
			}
			return result;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000098A8 File Offset: 0x00007AA8
		public TValue GetLazy<TValue>(object obj, object key, Func<object, TValue> defValueFactory)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary listDictionary;
			bool flag3 = !this.annotations.TryGetValue(obj, out listDictionary);
			TValue result;
			if (flag3)
			{
				result = defValueFactory(key);
			}
			else
			{
				bool flag4 = !listDictionary.Contains(key);
				if (flag4)
				{
					result = defValueFactory(key);
				}
				else
				{
					Type typeFromHandle = typeof(TValue);
					bool isValueType = typeFromHandle.IsValueType;
					if (isValueType)
					{
						result = (TValue)((object)Convert.ChangeType(listDictionary[key], typeof(TValue)));
					}
					else
					{
						result = (TValue)((object)listDictionary[key]);
					}
				}
			}
			return result;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00009964 File Offset: 0x00007B64
		public TValue GetOrCreate<TValue>(object obj, object key, Func<object, TValue> factory)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary listDictionary;
			bool flag3 = !this.annotations.TryGetValue(obj, out listDictionary);
			if (flag3)
			{
				listDictionary = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			bool flag4 = listDictionary.Contains(key);
			TValue result;
			if (flag4)
			{
				Type typeFromHandle = typeof(TValue);
				bool isValueType = typeFromHandle.IsValueType;
				if (isValueType)
				{
					result = (TValue)((object)Convert.ChangeType(listDictionary[key], typeof(TValue)));
				}
				else
				{
					result = (TValue)((object)listDictionary[key]);
				}
			}
			else
			{
				TValue tvalue;
				listDictionary[key] = (tvalue = factory(key));
				result = tvalue;
			}
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00009A40 File Offset: 0x00007C40
		public void Set<TValue>(object obj, object key, TValue value)
		{
			bool flag = obj == null;
			if (flag)
			{
				throw new ArgumentNullException("obj");
			}
			bool flag2 = key == null;
			if (flag2)
			{
				throw new ArgumentNullException("key");
			}
			ListDictionary listDictionary;
			bool flag3 = !this.annotations.TryGetValue(obj, out listDictionary);
			if (flag3)
			{
				listDictionary = (this.annotations[new Annotations.WeakReferenceKey(obj)] = new ListDictionary());
			}
			listDictionary[key] = value;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00009AB4 File Offset: 0x00007CB4
		public void Trim()
		{
			foreach (object key in from kvp in this.annotations
			where !((Annotations.WeakReferenceKey)kvp.Key).IsAlive
			select kvp.Key)
			{
				this.annotations.Remove(key);
			}
		}

		// Token: 0x040000EC RID: 236
		private readonly Dictionary<object, ListDictionary> annotations = new Dictionary<object, ListDictionary>(Annotations.WeakReferenceComparer.Instance);

		// Token: 0x02000026 RID: 38
		private class WeakReferenceComparer : IEqualityComparer<object>
		{
			// Token: 0x060000D6 RID: 214 RVA: 0x00002583 File Offset: 0x00000783
			private WeakReferenceComparer()
			{
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x00009B54 File Offset: 0x00007D54
			public bool Equals(object x, object y)
			{
				bool flag = y is Annotations.WeakReferenceKey && !(x is WeakReference);
				bool result;
				if (flag)
				{
					result = this.Equals(y, x);
				}
				else
				{
					Annotations.WeakReferenceKey weakReferenceKey = x as Annotations.WeakReferenceKey;
					Annotations.WeakReferenceKey weakReferenceKey2 = y as Annotations.WeakReferenceKey;
					bool flag2 = weakReferenceKey != null && weakReferenceKey2 != null;
					if (flag2)
					{
						result = (weakReferenceKey.IsAlive && weakReferenceKey2.IsAlive && weakReferenceKey.Target == weakReferenceKey2.Target);
					}
					else
					{
						bool flag3 = weakReferenceKey != null && weakReferenceKey2 == null;
						if (flag3)
						{
							result = (weakReferenceKey.IsAlive && weakReferenceKey.Target == y);
						}
						else
						{
							bool flag4 = weakReferenceKey == null && weakReferenceKey2 == null;
							if (!flag4)
							{
								throw new UnreachableException();
							}
							result = (weakReferenceKey.IsAlive && weakReferenceKey.Target == y);
						}
					}
				}
				return result;
			}

			// Token: 0x060000D8 RID: 216 RVA: 0x00009C28 File Offset: 0x00007E28
			public int GetHashCode(object obj)
			{
				bool flag = obj is Annotations.WeakReferenceKey;
				int hashCode;
				if (flag)
				{
					hashCode = ((Annotations.WeakReferenceKey)obj).HashCode;
				}
				else
				{
					hashCode = obj.GetHashCode();
				}
				return hashCode;
			}

			// Token: 0x040000ED RID: 237
			public static readonly Annotations.WeakReferenceComparer Instance = new Annotations.WeakReferenceComparer();
		}

		// Token: 0x02000027 RID: 39
		private class WeakReferenceKey : WeakReference
		{
			// Token: 0x060000DA RID: 218 RVA: 0x00002599 File Offset: 0x00000799
			public WeakReferenceKey(object target) : base(target)
			{
				this.HashCode = target.GetHashCode();
			}

			// Token: 0x17000001 RID: 1
			// (get) Token: 0x060000DB RID: 219 RVA: 0x000025B1 File Offset: 0x000007B1
			// (set) Token: 0x060000DC RID: 220 RVA: 0x000025B9 File Offset: 0x000007B9
			public int HashCode { get; private set; }
		}
	}
}
