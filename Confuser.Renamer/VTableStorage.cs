using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000012 RID: 18
	public class VTableStorage
	{
		// Token: 0x0600008E RID: 142 RVA: 0x00009910 File Offset: 0x00007B10
		public VTableStorage(Confuser.Core.ILogger logger)
		{
			this.logger = logger;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000992C File Offset: 0x00007B2C
		public Confuser.Core.ILogger GetLogger()
		{
			return this.logger;
		}

		// Token: 0x1700001A RID: 26
		public VTable this[TypeDef type]
		{
			get
			{
				return this.storage.GetValueOrDefault(type, null);
			}
			internal set
			{
				this.storage[type] = value;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00009974 File Offset: 0x00007B74
		private VTable GetOrConstruct(TypeDef type)
		{
			VTable result;
			bool flag = !this.storage.TryGetValue(type, out result);
			if (flag)
			{
				result = (this.storage[type] = VTable.ConstructVTable(type, this));
			}
			return result;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000099B4 File Offset: 0x00007BB4
		public VTable GetVTable(ITypeDefOrRef type)
		{
			bool flag = type == null;
			VTable result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = type is TypeDef;
				if (flag2)
				{
					result = this.GetOrConstruct((TypeDef)type);
				}
				else
				{
					bool flag3 = type is TypeRef;
					if (flag3)
					{
						result = this.GetOrConstruct(((TypeRef)type).ResolveThrow());
					}
					else
					{
						bool flag4 = type is TypeSpec;
						if (!flag4)
						{
							throw new UnreachableException();
						}
						TypeSig typeSig = ((TypeSpec)type).TypeSig;
						bool flag5 = typeSig is TypeDefOrRefSig;
						if (flag5)
						{
							TypeDef type2 = ((TypeDefOrRefSig)typeSig).TypeDefOrRef.ResolveTypeDefThrow();
							result = this.GetOrConstruct(type2);
						}
						else
						{
							bool flag6 = typeSig is GenericInstSig;
							if (!flag6)
							{
								throw new NotSupportedException("Unexpected type: " + type);
							}
							GenericInstSig genericInstSig = (GenericInstSig)typeSig;
							TypeDef typeDef = genericInstSig.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
							VTable orConstruct = this.GetOrConstruct(typeDef);
							result = VTableStorage.ResolveGenericArgument(typeDef, genericInstSig, orConstruct);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00009AC0 File Offset: 0x00007CC0
		private static VTableSlot ResolveSlot(TypeDef openType, VTableSlot slot, IList<TypeSig> genArgs)
		{
			MethodSig sig = GenericArgumentResolver.Resolve(slot.Signature.MethodSig, genArgs);
			TypeSig typeSig = slot.MethodDefDeclType;
			bool flag = default(SigComparer).Equals(typeSig, openType);
			if (flag)
			{
				typeSig = new GenericInstSig((ClassOrValueTypeSig)openType.ToTypeSig(true), genArgs.ToArray<TypeSig>());
			}
			else
			{
				typeSig = GenericArgumentResolver.Resolve(typeSig, genArgs);
			}
			return new VTableSlot(typeSig, slot.MethodDef, slot.DeclaringType, new VTableSignature(sig, slot.Signature.Name), slot.Overrides);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00009B4C File Offset: 0x00007D4C
		private static VTable ResolveGenericArgument(TypeDef openType, GenericInstSig genInst, VTable vTable)
		{
			Debug.Assert(default(SigComparer).Equals(openType, vTable.Type));
			VTable vtable = new VTable(genInst);
			foreach (VTableSlot slot2 in vTable.Slots)
			{
				vtable.Slots.Add(VTableStorage.ResolveSlot(openType, slot2, genInst.GenericArguments));
			}
			Func<VTableSlot, VTableSlot> <>9__0;
			foreach (KeyValuePair<TypeSig, IList<VTableSlot>> keyValuePair in vTable.InterfaceSlots)
			{
				IDictionary<TypeSig, IList<VTableSlot>> interfaceSlots = vtable.InterfaceSlots;
				TypeSig key = GenericArgumentResolver.Resolve(keyValuePair.Key, genInst.GenericArguments);
				IEnumerable<VTableSlot> value = keyValuePair.Value;
				Func<VTableSlot, VTableSlot> selector;
				if ((selector = <>9__0) == null)
				{
					selector = (<>9__0 = ((VTableSlot slot) => VTableStorage.ResolveSlot(openType, slot, genInst.GenericArguments)));
				}
				interfaceSlots.Add(key, value.Select(selector).ToList<VTableSlot>());
			}
			return vtable;
		}

		// Token: 0x0400003A RID: 58
		private Dictionary<TypeDef, VTable> storage = new Dictionary<TypeDef, VTable>();

		// Token: 0x0400003B RID: 59
		private Confuser.Core.ILogger logger;
	}
}
