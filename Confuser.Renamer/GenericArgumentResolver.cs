using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000004 RID: 4
	public struct GenericArgumentResolver
	{
		// Token: 0x06000010 RID: 16 RVA: 0x00006AC8 File Offset: 0x00004CC8
		public static TypeSig Resolve(TypeSig typeSig, IList<TypeSig> typeGenArgs)
		{
			bool flag = typeGenArgs == null;
			if (flag)
			{
				throw new ArgumentException("No generic arguments to resolve.");
			}
			GenericArgumentResolver genericArgumentResolver = default(GenericArgumentResolver);
			genericArgumentResolver.genericArguments = new GenericArguments();
			genericArgumentResolver.recursionCounter = default(RecursionCounter);
			bool flag2 = typeGenArgs != null;
			if (flag2)
			{
				genericArgumentResolver.genericArguments.PushTypeArgs(typeGenArgs);
			}
			return genericArgumentResolver.ResolveGenericArgs(typeSig);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00006B2C File Offset: 0x00004D2C
		public static MethodSig Resolve(MethodSig methodSig, IList<TypeSig> typeGenArgs)
		{
			bool flag = typeGenArgs == null;
			if (flag)
			{
				throw new ArgumentException("No generic arguments to resolve.");
			}
			GenericArgumentResolver genericArgumentResolver = default(GenericArgumentResolver);
			genericArgumentResolver.genericArguments = new GenericArguments();
			genericArgumentResolver.recursionCounter = default(RecursionCounter);
			bool flag2 = typeGenArgs != null;
			if (flag2)
			{
				genericArgumentResolver.genericArguments.PushTypeArgs(typeGenArgs);
			}
			return genericArgumentResolver.ResolveGenericArgs(methodSig);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00006B90 File Offset: 0x00004D90
		private bool ReplaceGenericArg(ref TypeSig typeSig)
		{
			bool flag = this.genericArguments == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				TypeSig typeSig2 = this.genericArguments.Resolve(typeSig);
				bool flag2 = typeSig2 != typeSig;
				if (flag2)
				{
					typeSig = typeSig2;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00006BD8 File Offset: 0x00004DD8
		private MethodSig ResolveGenericArgs(MethodSig sig)
		{
			bool flag = sig == null;
			MethodSig result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = !this.recursionCounter.Increment();
				if (flag2)
				{
					result = null;
				}
				else
				{
					MethodSig methodSig = this.ResolveGenericArgs(new MethodSig(sig.GetCallingConvention()), sig);
					this.recursionCounter.Decrement();
					result = methodSig;
				}
			}
			return result;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00006C2C File Offset: 0x00004E2C
		private MethodSig ResolveGenericArgs(MethodSig sig, MethodSig old)
		{
			sig.RetType = this.ResolveGenericArgs(old.RetType);
			foreach (TypeSig typeSig in old.Params)
			{
				sig.Params.Add(this.ResolveGenericArgs(typeSig));
			}
			sig.GenParamCount = old.GenParamCount;
			bool flag = sig.ParamsAfterSentinel != null;
			if (flag)
			{
				foreach (TypeSig typeSig2 in old.ParamsAfterSentinel)
				{
					sig.ParamsAfterSentinel.Add(this.ResolveGenericArgs(typeSig2));
				}
			}
			return sig;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00006D0C File Offset: 0x00004F0C
		private TypeSig ResolveGenericArgs(TypeSig typeSig)
		{
			bool flag = !this.recursionCounter.Increment();
			TypeSig result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.ReplaceGenericArg(ref typeSig);
				if (flag2)
				{
					this.recursionCounter.Decrement();
					result = typeSig;
				}
				else
				{
					ElementType elementType = typeSig.ElementType;
					ElementType elementType2 = elementType;
					TypeSig typeSig2;
					switch (elementType2)
					{
					case ElementType.Ptr:
						typeSig2 = new PtrSig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_290;
					case ElementType.ByRef:
						typeSig2 = new ByRefSig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_290;
					case ElementType.ValueType:
					case ElementType.Class:
					case ElementType.TypedByRef:
					case ElementType.I:
					case ElementType.U:
					case ElementType.R:
					case ElementType.Object:
						break;
					case ElementType.Var:
						typeSig2 = new GenericVar((typeSig as GenericVar).Number);
						goto IL_290;
					case ElementType.Array:
					{
						ArraySig arraySig = (ArraySig)typeSig;
						List<uint> sizes = new List<uint>(arraySig.Sizes);
						List<int> lowerBounds = new List<int>(arraySig.LowerBounds);
						typeSig2 = new ArraySig(this.ResolveGenericArgs(typeSig.Next), arraySig.Rank, sizes, lowerBounds);
						goto IL_290;
					}
					case ElementType.GenericInst:
					{
						GenericInstSig genericInstSig = (GenericInstSig)typeSig;
						List<TypeSig> list = new List<TypeSig>(genericInstSig.GenericArguments.Count);
						foreach (TypeSig typeSig3 in genericInstSig.GenericArguments)
						{
							list.Add(this.ResolveGenericArgs(typeSig3));
						}
						typeSig2 = new GenericInstSig(this.ResolveGenericArgs(genericInstSig.GenericType) as ClassOrValueTypeSig, list);
						goto IL_290;
					}
					case ElementType.ValueArray:
						typeSig2 = new ValueArraySig(this.ResolveGenericArgs(typeSig.Next), (typeSig as ValueArraySig).Size);
						goto IL_290;
					case ElementType.FnPtr:
						throw new NotSupportedException("FnPtr is not supported.");
					case ElementType.SZArray:
						typeSig2 = new SZArraySig(this.ResolveGenericArgs(typeSig.Next));
						goto IL_290;
					case ElementType.MVar:
						typeSig2 = new GenericMVar((typeSig as GenericMVar).Number);
						goto IL_290;
					case ElementType.CModReqd:
						typeSig2 = new CModReqdSig((typeSig as ModifierSig).Modifier, this.ResolveGenericArgs(typeSig.Next));
						goto IL_290;
					case ElementType.CModOpt:
						typeSig2 = new CModOptSig((typeSig as ModifierSig).Modifier, this.ResolveGenericArgs(typeSig.Next));
						goto IL_290;
					default:
						if (elementType2 == ElementType.Module)
						{
							typeSig2 = new ModuleSig((typeSig as ModuleSig).Index, this.ResolveGenericArgs(typeSig.Next));
							goto IL_290;
						}
						if (elementType2 == ElementType.Pinned)
						{
							typeSig2 = new PinnedSig(this.ResolveGenericArgs(typeSig.Next));
							goto IL_290;
						}
						break;
					}
					typeSig2 = typeSig;
					IL_290:
					this.recursionCounter.Decrement();
					result = typeSig2;
				}
			}
			return result;
		}

		// Token: 0x04000004 RID: 4
		private GenericArguments genericArguments;

		// Token: 0x04000005 RID: 5
		private RecursionCounter recursionCounter;
	}
}
