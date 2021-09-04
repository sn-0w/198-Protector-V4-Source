using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer;
using Confuser.Renamer.References;
using dnlib.DotNet;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000065 RID: 101
	internal abstract class RPMode
	{
		// Token: 0x060001F3 RID: 499
		public abstract void ProcessCall(RPContext ctx, int instrIndex);

		// Token: 0x060001F4 RID: 500
		public abstract void Finalize(RPContext ctx);

		// Token: 0x060001F5 RID: 501 RVA: 0x0000ACE8 File Offset: 0x00008EE8
		private static ITypeDefOrRef Import(RPContext ctx, TypeDef typeDef)
		{
			ITypeDefOrRef typeDefOrRef = new Importer(ctx.Module, ImporterOptions.TryToUseTypeDefs).Import(typeDef);
			bool flag = typeDef.Module != ctx.Module && ctx.Context.Modules.Contains((ModuleDefMD)typeDef.Module);
			if (flag)
			{
				ctx.Name.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)typeDefOrRef, typeDef));
			}
			return typeDefOrRef;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000AD5C File Offset: 0x00008F5C
		protected static MethodSig CreateProxySignature(RPContext ctx, IMethod method, bool newObj)
		{
			ModuleDef module = ctx.Module;
			MethodSig result;
			if (newObj)
			{
				Debug.Assert(method.MethodSig.HasThis);
				Debug.Assert(method.Name == ".ctor");
				TypeSig[] argTypes = method.MethodSig.Params.Select(delegate(TypeSig type)
				{
					bool flag4 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
					TypeSig result2;
					if (flag4)
					{
						result2 = module.CorLibTypes.Object;
					}
					else
					{
						result2 = type;
					}
					return result2;
				}).ToArray<TypeSig>();
				bool typeErasure = ctx.TypeErasure;
				TypeSig retType;
				if (typeErasure)
				{
					retType = module.CorLibTypes.Object;
				}
				else
				{
					TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
					retType = RPMode.Import(ctx, typeDef).ToTypeSig(true);
				}
				result = MethodSig.CreateStatic(retType, argTypes);
			}
			else
			{
				IEnumerable<TypeSig> enumerable = method.MethodSig.Params.Select(delegate(TypeSig type)
				{
					bool flag4 = ctx.TypeErasure && type.IsClassSig && method.MethodSig.HasThis;
					TypeSig result2;
					if (flag4)
					{
						result2 = module.CorLibTypes.Object;
					}
					else
					{
						result2 = type;
					}
					return result2;
				});
				bool flag = method.MethodSig.HasThis && !method.MethodSig.ExplicitThis;
				if (flag)
				{
					TypeDef typeDef2 = method.DeclaringType.ResolveTypeDefThrow();
					bool flag2 = ctx.TypeErasure && !typeDef2.IsValueType;
					if (flag2)
					{
						enumerable = new CorLibTypeSig[]
						{
							module.CorLibTypes.Object
						}.Concat(enumerable);
					}
					else
					{
						enumerable = new TypeSig[]
						{
							RPMode.Import(ctx, typeDef2).ToTypeSig(true)
						}.Concat(enumerable);
					}
				}
				TypeSig typeSig = method.MethodSig.RetType;
				bool flag3 = ctx.TypeErasure && typeSig.IsClassSig;
				if (flag3)
				{
					typeSig = module.CorLibTypes.Object;
				}
				result = MethodSig.CreateStatic(typeSig, enumerable.ToArray<TypeSig>());
			}
			return result;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000AF70 File Offset: 0x00009170
		protected static TypeDef GetDelegateType(RPContext ctx, MethodSig sig)
		{
			TypeDef typeDef;
			bool flag = ctx.Delegates.TryGetValue(sig, out typeDef);
			TypeDef result;
			if (flag)
			{
				result = typeDef;
			}
			else
			{
				typeDef = new TypeDefUser(ctx.Name.ObfuscateName(ctx.Method.DeclaringType.Namespace, RenameMode.RealNames), ctx.Name.RandomName(), ctx.Module.CorLibTypes.GetTypeRef("System", "MulticastDelegate"));
				typeDef.Attributes = TypeAttributes.Sealed;
				MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(ctx.Module.CorLibTypes.Void, ctx.Module.CorLibTypes.Object, ctx.Module.CorLibTypes.IntPtr));
				methodDefUser.Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
				methodDefUser.ImplAttributes = MethodImplAttributes.CodeTypeMask;
				typeDef.Methods.Add(methodDefUser);
				MethodDefUser methodDefUser2 = new MethodDefUser("Invoke", sig.Clone());
				methodDefUser2.MethodSig.HasThis = true;
				methodDefUser2.Attributes = (MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask);
				methodDefUser2.ImplAttributes = MethodImplAttributes.CodeTypeMask;
				typeDef.Methods.Add(methodDefUser2);
				ctx.Module.Types.Add(typeDef);
				foreach (IDnlibDef dnlibDef in typeDef.FindDefinitions())
				{
					ctx.Marker.Mark(dnlibDef, ctx.Protection);
					ctx.Name.SetCanRename(dnlibDef, false);
				}
				ctx.Delegates[sig] = typeDef;
				result = typeDef;
			}
			return result;
		}
	}
}
