using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000078 RID: 120
	internal class VTableAnalyzer : IRenamer
	{
		// Token: 0x060002F4 RID: 756 RVA: 0x0002819C File Offset: 0x0002639C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			bool flag = def is TypeDef;
			if (flag)
			{
				TypeDef typeDef = (TypeDef)def;
				bool isInterface = typeDef.IsInterface;
				if (!isInterface)
				{
					VTable vtable = service.GetVTables()[typeDef];
					foreach (IList<VTableSlot> list in vtable.InterfaceSlots.Values)
					{
						foreach (VTableSlot vtableSlot in list)
						{
							bool flag2 = vtableSlot.Overrides == null;
							if (!flag2)
							{
								Debug.Assert(vtableSlot.Overrides.MethodDef.DeclaringType.IsInterface);
								bool flag3 = context.Modules.Contains(vtableSlot.MethodDef.DeclaringType.Module as ModuleDefMD);
								bool flag4 = context.Modules.Contains(vtableSlot.Overrides.MethodDef.DeclaringType.Module as ModuleDefMD);
								bool flag5 = (!flag3 && flag4) || !service.CanRename(vtableSlot.MethodDef);
								if (flag5)
								{
									service.SetCanRename(vtableSlot.Overrides.MethodDef, false);
								}
								else
								{
									bool flag6 = (flag3 && !flag4) || !service.CanRename(vtableSlot.Overrides.MethodDef);
									if (flag6)
									{
										service.SetCanRename(vtableSlot.MethodDef, false);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				bool flag7 = def is MethodDef;
				if (flag7)
				{
					MethodDef methodDef = (MethodDef)def;
					bool flag8 = !methodDef.IsVirtual;
					if (!flag8)
					{
						VTable vtable = service.GetVTables()[methodDef.DeclaringType];
						VTableSignature vtableSignature = VTableSignature.FromMethod(methodDef);
						IEnumerable<VTableSlot> enumerable = vtable.FindSlots(methodDef);
						bool flag9 = !methodDef.IsAbstract;
						if (flag9)
						{
							foreach (VTableSlot vtableSlot2 in enumerable)
							{
								bool flag10 = vtableSlot2.Overrides == null;
								if (!flag10)
								{
									VTableAnalyzer.SetupOverwriteReferences(context, service, vtableSlot2, methodDef.Module);
								}
							}
						}
						else
						{
							foreach (VTableSlot vtableSlot3 in enumerable)
							{
								bool flag11 = vtableSlot3.Overrides == null;
								if (!flag11)
								{
									service.SetCanRename(methodDef, false);
									service.SetCanRename(vtableSlot3.Overrides.MethodDef, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000284CC File Offset: 0x000266CC
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef method = def as MethodDef;
			bool flag = method == null || !method.IsVirtual || method.Overrides.Count == 0;
			if (!flag)
			{
				HashSet<IMethodDefOrRef> hashSet = new HashSet<IMethodDefOrRef>(VTableAnalyzer.MethodDefOrRefComparer.Instance);
				method.Overrides.RemoveWhere((MethodOverride impl) => VTableAnalyzer.MethodDefOrRefComparer.Instance.Equals(impl.MethodDeclaration, method));
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00028548 File Offset: 0x00026748
		private static void AddImportReference(ConfuserContext context, INameService service, ModuleDef module, MethodDef method, MemberRef methodRef)
		{
			bool flag = method.Module != module && context.Modules.Contains((ModuleDefMD)module);
			if (flag)
			{
				TypeRef typeRef = (TypeRef)methodRef.DeclaringType.ScopeType;
				service.AddReference<TypeDef>(method.DeclaringType, new TypeRefReference(typeRef, method.DeclaringType));
				service.AddReference<IDnlibDef>(method, new MemberRefReference(methodRef, method));
				List<ITypeDefOrRef> list = methodRef.MethodSig.Params.SelectMany((TypeSig param) => param.FindTypeRefs()).ToList<ITypeDefOrRef>();
				list.AddRange(methodRef.MethodSig.RetType.FindTypeRefs());
				list.AddRange(methodRef.DeclaringType.ToTypeSig(true).FindTypeRefs());
				foreach (ITypeDefOrRef typeDefOrRef in list)
				{
					VTableAnalyzer.SetupTypeReference(context, service, module, typeDefOrRef);
				}
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00028668 File Offset: 0x00026868
		private static void SetupTypeReference(ConfuserContext context, INameService service, ModuleDef module, ITypeDefOrRef typeDefOrRef)
		{
			TypeRef typeRef = typeDefOrRef as TypeRef;
			bool flag = typeRef == null;
			if (!flag)
			{
				TypeDef typeDef = typeRef.ResolveTypeDefThrow();
				bool flag2 = typeDef.Module != module && context.Modules.Contains((ModuleDefMD)typeDef.Module);
				if (flag2)
				{
					service.AddReference<TypeDef>(typeDef, new TypeRefReference(typeRef, typeDef));
				}
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x000286C8 File Offset: 0x000268C8
		private static GenericInstSig SetupSignatureReferences(ConfuserContext context, INameService service, ModuleDef module, GenericInstSig typeSig)
		{
			ClassOrValueTypeSig genericType = VTableAnalyzer.SetupSignatureReferences<ClassOrValueTypeSig>(context, service, module, typeSig.GenericType);
			List<TypeSig> genArgs = (from a in typeSig.GenericArguments
			select VTableAnalyzer.SetupSignatureReferences<TypeSig>(context, service, module, a)).ToList<TypeSig>();
			return new GenericInstSig(genericType, genArgs);
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00028738 File Offset: 0x00026938
		private static T SetupSignatureReferences<T>(ConfuserContext context, INameService service, ModuleDef module, T typeSig) where T : TypeSig
		{
			TypeRef typeRef = typeSig.TryGetTypeRef();
			bool flag = typeRef != null;
			if (flag)
			{
				VTableAnalyzer.SetupTypeReference(context, service, module, typeRef);
			}
			return typeSig;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0002876C File Offset: 0x0002696C
		private static void SetupOverwriteReferences(ConfuserContext context, INameService service, VTableSlot slot, ModuleDef module)
		{
			MethodDef methodDef = slot.MethodDef;
			VTableSlot overrides = slot.Overrides;
			MethodDef methodDef2 = overrides.MethodDef;
			OverrideDirectiveReference reference = new OverrideDirectiveReference(slot, overrides);
			service.AddReference<MethodDef>(methodDef, reference);
			service.AddReference<MethodDef>(slot.Overrides.MethodDef, reference);
			Importer importer = new Importer(module, ImporterOptions.TryToUseTypeDefs);
			TypeSig methodDefDeclType = overrides.MethodDefDeclType;
			GenericInstSig genericInstSig = methodDefDeclType as GenericInstSig;
			bool flag = genericInstSig != null;
			IMethod target;
			if (flag)
			{
				GenericInstSig sig = VTableAnalyzer.SetupSignatureReferences(context, service, module, genericInstSig);
				MemberRef memberRef = new MemberRefUser(module, methodDef2.Name, methodDef2.MethodSig, sig.ToTypeDefOrRef());
				memberRef = importer.Import(memberRef);
				service.AddReference<IDnlibDef>(methodDef2, new MemberRefReference(memberRef, methodDef2));
				target = memberRef;
			}
			else
			{
				target = methodDef2;
				bool flag2 = target.Module != module;
				if (flag2)
				{
					target = importer.Import(methodDef2);
					MemberRef memberRef2 = target as MemberRef;
					bool flag3 = memberRef2 != null;
					if (flag3)
					{
						service.AddReference<IDnlibDef>(methodDef2, new MemberRefReference(memberRef2, methodDef2));
					}
				}
			}
			target.MethodSig = importer.Import(methodDef.MethodSig);
			MemberRef memberRef3 = target as MemberRef;
			bool flag4 = memberRef3 != null;
			if (flag4)
			{
				VTableAnalyzer.AddImportReference(context, service, module, methodDef2, memberRef3);
			}
			bool flag5 = methodDef.Overrides.Any(delegate(MethodOverride impl)
			{
				SigComparer sigComparer = default(SigComparer);
				bool result;
				if (sigComparer.Equals(impl.MethodDeclaration.MethodSig, target.MethodSig))
				{
					sigComparer = default(SigComparer);
					result = sigComparer.Equals(impl.MethodDeclaration.DeclaringType.ResolveTypeDef(), target.DeclaringType.ResolveTypeDef());
				}
				else
				{
					result = false;
				}
				return result;
			});
			if (!flag5)
			{
				methodDef.Overrides.Add(new MethodOverride(methodDef, (IMethodDefOrRef)target));
			}
		}

		// Token: 0x0200009F RID: 159
		private class MethodDefOrRefComparer : IEqualityComparer<IMethodDefOrRef>
		{
			// Token: 0x06000386 RID: 902 RVA: 0x0000A2FF File Offset: 0x000084FF
			private MethodDefOrRefComparer()
			{
			}

			// Token: 0x06000387 RID: 903 RVA: 0x0002AD44 File Offset: 0x00028F44
			public bool Equals(IMethodDefOrRef x, IMethodDefOrRef y)
			{
				SigComparer sigComparer = default(SigComparer);
				bool result;
				if (sigComparer.Equals(x, y))
				{
					sigComparer = default(SigComparer);
					result = sigComparer.Equals(x.DeclaringType, y.DeclaringType);
				}
				else
				{
					result = false;
				}
				return result;
			}

			// Token: 0x06000388 RID: 904 RVA: 0x0002AD88 File Offset: 0x00028F88
			public int GetHashCode(IMethodDefOrRef obj)
			{
				SigComparer sigComparer = default(SigComparer);
				int num = sigComparer.GetHashCode(obj) * 5;
				sigComparer = default(SigComparer);
				return num + sigComparer.GetHashCode(obj.DeclaringType);
			}

			// Token: 0x040005A8 RID: 1448
			public static readonly VTableAnalyzer.MethodDefOrRefComparer Instance = new VTableAnalyzer.MethodDefOrRefComparer();
		}
	}
}
