using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000011 RID: 17
	public class VTable
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00008CAD File Offset: 0x00006EAD
		internal VTable(TypeSig type)
		{
			this.Type = type;
			this.Slots = new List<VTableSlot>();
			this.InterfaceSlots = new Dictionary<TypeSig, IList<VTableSlot>>();
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00008CD7 File Offset: 0x00006ED7
		// (set) Token: 0x06000083 RID: 131 RVA: 0x00008CDF File Offset: 0x00006EDF
		public TypeSig Type { get; private set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00008CE8 File Offset: 0x00006EE8
		// (set) Token: 0x06000085 RID: 133 RVA: 0x00008CF0 File Offset: 0x00006EF0
		public IList<VTableSlot> Slots { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000086 RID: 134 RVA: 0x00008CF9 File Offset: 0x00006EF9
		// (set) Token: 0x06000087 RID: 135 RVA: 0x00008D01 File Offset: 0x00006F01
		public IDictionary<TypeSig, IList<VTableSlot>> InterfaceSlots { get; private set; }

		// Token: 0x06000088 RID: 136 RVA: 0x00008D0C File Offset: 0x00006F0C
		public IEnumerable<VTableSlot> FindSlots(IMethod method)
		{
			return from slot in this.Slots.Concat(this.InterfaceSlots.SelectMany((KeyValuePair<TypeSig, IList<VTableSlot>> iface) => iface.Value))
			where slot.MethodDef == method
			select slot;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00008D74 File Offset: 0x00006F74
		public static VTable ConstructVTable(TypeDef typeDef, VTableStorage storage)
		{
			VTable vtable = new VTable(typeDef.ToTypeSig(true));
			Dictionary<VTableSignature, MethodDef> virtualMethods = (from method in typeDef.Methods
			where method.IsVirtual
			select method).ToDictionary((MethodDef method) => VTableSignature.FromMethod(method), (MethodDef method) => method);
			VTable.VTableConstruction vTbl = new VTable.VTableConstruction();
			VTable vtable2 = storage.GetVTable(typeDef.GetBaseTypeThrow());
			bool flag = vtable2 != null;
			if (flag)
			{
				VTable.Inherits(vTbl, vtable2);
			}
			foreach (InterfaceImpl interfaceImpl in typeDef.Interfaces)
			{
				VTable vtable3 = storage.GetVTable(interfaceImpl.Interface);
				bool flag2 = vtable3 != null;
				if (flag2)
				{
					VTable.Implements(vTbl, virtualMethods, vtable3, interfaceImpl.Interface.ToTypeSig(true));
				}
			}
			bool flag3 = !typeDef.IsInterface;
			if (flag3)
			{
				Func<VTableSignature, bool> <>9__9;
				Func<ValueTuple<VTableSignature, VTableSlot>, VTableSlot> <>9__7;
				foreach (TypeSig key in vTbl.InterfaceSlots.Keys.ToList<TypeSig>())
				{
					ILookup<VTableSignature, VTableSlot> lookup = vTbl.InterfaceSlots[key];
					IEnumerable<VTableSignature> source = from g in lookup
					select g.Key;
					Func<VTableSignature, bool> predicate;
					if ((predicate = <>9__9) == null)
					{
						predicate = (<>9__9 = ((VTableSignature sig) => virtualMethods.ContainsKey(sig) || vTbl.SlotsMap.ContainsKey(sig)));
					}
					bool flag4 = source.Any(predicate);
					if (flag4)
					{
						IEnumerable<ValueTuple<VTableSignature, VTableSlot>> source2 = lookup.SelectMany((IGrouping<VTableSignature, VTableSlot> g) => from slot in g
						select new ValueTuple<VTableSignature, VTableSlot>(g.Key, slot));
						Func<ValueTuple<VTableSignature, VTableSlot>, VTableSignature> keySelector = ([TupleElementNames(new string[]
						{
							"Key",
							"Slot"
						})] ValueTuple<VTableSignature, VTableSlot> t) => t.Item1;
						Func<ValueTuple<VTableSignature, VTableSlot>, VTableSlot> elementSelector;
						if ((elementSelector = <>9__7) == null)
						{
							elementSelector = (<>9__7 = delegate([TupleElementNames(new string[]
							{
								"Key",
								"Slot"
							})] ValueTuple<VTableSignature, VTableSlot> t)
							{
								bool flag6 = !t.Item2.MethodDef.DeclaringType.IsInterface;
								VTableSlot result;
								if (flag6)
								{
									result = t.Item2;
								}
								else
								{
									MethodDef method;
									bool flag7 = virtualMethods.TryGetValue(t.Item1, out method);
									if (flag7)
									{
										result = t.Item2.OverridedBy(method);
									}
									else
									{
										VTableSlot vtableSlot3;
										bool flag8 = vTbl.SlotsMap.TryGetValue(t.Item1, out vtableSlot3);
										if (flag8)
										{
											result = t.Item2.OverridedBy(vtableSlot3.MethodDef);
										}
										else
										{
											result = t.Item2;
										}
									}
								}
								return result;
							});
						}
						lookup = source2.ToLookup(keySelector, elementSelector);
						vTbl.InterfaceSlots[key] = lookup;
					}
				}
			}
			foreach (KeyValuePair<VTableSignature, MethodDef> keyValuePair in virtualMethods)
			{
				bool isNewSlot = keyValuePair.Value.IsNewSlot;
				VTableSlot vtableSlot;
				if (isNewSlot)
				{
					vtableSlot = new VTableSlot(keyValuePair.Value, typeDef.ToTypeSig(true), keyValuePair.Key);
				}
				else
				{
					bool flag5 = vTbl.SlotsMap.TryGetValue(keyValuePair.Key, out vtableSlot);
					if (flag5)
					{
						Debug.Assert(!vtableSlot.MethodDef.IsFinal);
						vtableSlot = vtableSlot.OverridedBy(keyValuePair.Value);
					}
					else
					{
						vtableSlot = new VTableSlot(keyValuePair.Value, typeDef.ToTypeSig(true), keyValuePair.Key);
					}
				}
				vTbl.SlotsMap[keyValuePair.Key] = vtableSlot;
				vTbl.AllSlots.Add(vtableSlot);
			}
			using (Dictionary<VTableSignature, MethodDef>.Enumerator enumerator4 = virtualMethods.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					KeyValuePair<VTableSignature, MethodDef> method = enumerator4.Current;
					foreach (MethodOverride methodOverride in method.Value.Overrides)
					{
						Debug.Assert(methodOverride.MethodBody == method.Value);
						MethodDef targetMethod = methodOverride.MethodDeclaration.ResolveThrow();
						bool isInterface = targetMethod.DeclaringType.IsInterface;
						if (isInterface)
						{
							TypeSig key2 = methodOverride.MethodDeclaration.DeclaringType.ToTypeSig(true);
							VTable.CheckKeyExist<TypeSig, ILookup<VTableSignature, VTableSlot>>(storage, vTbl.InterfaceSlots, key2, "MethodImpl Iface");
							ILookup<VTableSignature, VTableSlot> lookup2 = vTbl.InterfaceSlots[key2];
							VTableSignature signature = VTableSignature.FromMethod(methodOverride.MethodDeclaration);
							VTable.CheckKeyExist<VTableSignature, VTableSlot>(storage, lookup2, signature, "MethodImpl Iface Sig");
							vTbl.InterfaceSlots[key2] = lookup2.SelectMany((IGrouping<VTableSignature, VTableSlot> g) => from slot in g
							select new ValueTuple<VTableSignature, VTableSlot>(g.Key, slot)).ToLookup(([TupleElementNames(new string[]
							{
								"Key",
								"Slot"
							})] ValueTuple<VTableSignature, VTableSlot> t) => t.Item1, delegate([TupleElementNames(new string[]
							{
								"Key",
								"Slot"
							})] ValueTuple<VTableSignature, VTableSlot> t)
							{
								bool flag6 = !t.Item1.Equals(signature);
								VTableSlot result;
								if (flag6)
								{
									result = t.Item2;
								}
								else
								{
									VTableSlot vtableSlot3 = t.Item2;
									while (vtableSlot3.Overrides != null)
									{
										vtableSlot3 = vtableSlot3.Overrides;
									}
									Debug.Assert(vtableSlot3.MethodDef.DeclaringType.IsInterface);
									Debug.Assert(vtableSlot3.Signature.Equals(t.Item2.Signature));
									result = vtableSlot3.OverridedBy(method.Value);
								}
								return result;
							});
						}
						else
						{
							VTableSlot vtableSlot2 = vTbl.AllSlots.Single((VTableSlot slot) => slot.MethodDef == targetMethod);
							VTable.CheckKeyExist<VTableSignature, VTableSlot>(storage, vTbl.SlotsMap, vtableSlot2.Signature, "MethodImpl Normal Sig");
							vtableSlot2 = vTbl.SlotsMap[vtableSlot2.Signature];
							while (vtableSlot2.MethodDef.DeclaringType == typeDef)
							{
								vtableSlot2 = vtableSlot2.Overrides;
							}
							vTbl.SlotsMap[vtableSlot2.Signature] = vtableSlot2.OverridedBy(method.Value);
						}
					}
				}
			}
			vtable.InterfaceSlots = vTbl.InterfaceSlots.ToDictionary((KeyValuePair<TypeSig, ILookup<VTableSignature, VTableSlot>> kvp) => kvp.Key, (KeyValuePair<TypeSig, ILookup<VTableSignature, VTableSlot>> kvp) => kvp.Value.SelectMany((IGrouping<VTableSignature, VTableSlot> g) => g).ToList<VTableSlot>());
			foreach (VTableSlot item in vTbl.AllSlots)
			{
				vtable.Slots.Add(item);
			}
			return vtable;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000094A4 File Offset: 0x000076A4
		private static void Implements(VTable.VTableConstruction vTbl, Dictionary<VTableSignature, MethodDef> virtualMethods, VTable ifaceVTbl, TypeSig iface)
		{
			Func<VTableSlot, VTableSlot> elementSelector = delegate(VTableSlot slot)
			{
				MethodDef methodDef;
				bool flag3 = virtualMethods.TryGetValue(slot.Signature, out methodDef) && methodDef.IsNewSlot && !methodDef.DeclaringType.IsInterface;
				VTableSlot result;
				if (flag3)
				{
					VTableSlot vtableSlot = slot;
					while (vtableSlot.Overrides != null && !vtableSlot.MethodDef.DeclaringType.IsInterface)
					{
						vtableSlot = vtableSlot.Overrides;
					}
					Debug.Assert(vtableSlot.MethodDef.DeclaringType.IsInterface);
					result = vtableSlot.OverridedBy(methodDef);
				}
				else
				{
					result = slot;
				}
				return result;
			};
			bool flag = vTbl.InterfaceSlots.ContainsKey(iface);
			if (flag)
			{
				vTbl.InterfaceSlots[iface] = vTbl.InterfaceSlots[iface].SelectMany((IGrouping<VTableSignature, VTableSlot> g) => g).ToLookup((VTableSlot slot) => slot.Signature, elementSelector);
			}
			else
			{
				vTbl.InterfaceSlots.Add(iface, ifaceVTbl.Slots.ToLookup((VTableSlot slot) => slot.Signature, elementSelector));
			}
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> keyValuePair in ifaceVTbl.InterfaceSlots)
			{
				bool flag2 = vTbl.InterfaceSlots.ContainsKey(keyValuePair.Key);
				if (flag2)
				{
					vTbl.InterfaceSlots[keyValuePair.Key] = vTbl.InterfaceSlots[keyValuePair.Key].SelectMany((IGrouping<VTableSignature, VTableSlot> g) => g).ToLookup((VTableSlot slot) => slot.Signature, elementSelector);
				}
				else
				{
					vTbl.InterfaceSlots.Add(keyValuePair.Key, keyValuePair.Value.ToLookup((VTableSlot slot) => slot.Signature, elementSelector));
				}
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000968C File Offset: 0x0000788C
		private static void Inherits(VTable.VTableConstruction vTbl, VTable baseVTbl)
		{
			foreach (VTableSlot vtableSlot in baseVTbl.Slots)
			{
				vTbl.AllSlots.Add(vtableSlot);
				vTbl.SlotsMap[vtableSlot.Signature] = vtableSlot;
			}
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> keyValuePair in baseVTbl.InterfaceSlots)
			{
				Debug.Assert(!vTbl.InterfaceSlots.ContainsKey(keyValuePair.Key));
				vTbl.InterfaceSlots.Add(keyValuePair.Key, keyValuePair.Value.ToLookup((VTableSlot slot) => slot.Signature, (VTableSlot slot) => slot));
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000097B0 File Offset: 0x000079B0
		[Conditional("DEBUG")]
		private static void CheckKeyExist<TKey, TValue>(VTableStorage storage, IDictionary<TKey, TValue> dictionary, TKey key, string name)
		{
			bool flag = !dictionary.ContainsKey(key);
			if (flag)
			{
				storage.GetLogger().ErrorFormat("{0} not found: {1}", new object[]
				{
					name,
					key
				});
				foreach (TKey tkey in dictionary.Keys)
				{
					storage.GetLogger().ErrorFormat("    {0}", new object[]
					{
						tkey
					});
				}
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00009850 File Offset: 0x00007A50
		[Conditional("DEBUG")]
		private static void CheckKeyExist<TKey, TValue>(VTableStorage storage, ILookup<TKey, TValue> lookup, TKey key, string name)
		{
			bool flag = !lookup.Contains(key);
			if (flag)
			{
				storage.GetLogger().ErrorFormat("{0} not found: {1}", new object[]
				{
					name,
					key
				});
				foreach (TKey tkey in from g in lookup
				select g.Key)
				{
					storage.GetLogger().ErrorFormat("    {0}", new object[]
					{
						tkey
					});
				}
			}
		}

		// Token: 0x02000083 RID: 131
		private class VTableConstruction
		{
			// Token: 0x04000550 RID: 1360
			public List<VTableSlot> AllSlots = new List<VTableSlot>();

			// Token: 0x04000551 RID: 1361
			public Dictionary<VTableSignature, VTableSlot> SlotsMap = new Dictionary<VTableSignature, VTableSlot>();

			// Token: 0x04000552 RID: 1362
			public Dictionary<TypeSig, ILookup<VTableSignature, VTableSlot>> InterfaceSlots = new Dictionary<TypeSig, ILookup<VTableSignature, VTableSlot>>(VTable.VTableConstruction.TypeSigComparer.Instance);

			// Token: 0x020000A7 RID: 167
			private class TypeSigComparer : IEqualityComparer<TypeSig>
			{
				// Token: 0x06000396 RID: 918 RVA: 0x0002AEB0 File Offset: 0x000290B0
				public bool Equals(TypeSig x, TypeSig y)
				{
					return default(SigComparer).Equals(x, y);
				}

				// Token: 0x06000397 RID: 919 RVA: 0x0002AED4 File Offset: 0x000290D4
				public int GetHashCode(TypeSig obj)
				{
					return default(SigComparer).GetHashCode(obj);
				}

				// Token: 0x040005B5 RID: 1461
				public static readonly VTable.VTableConstruction.TypeSigComparer Instance = new VTable.VTableConstruction.TypeSigComparer();
			}
		}
	}
}
