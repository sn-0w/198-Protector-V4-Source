using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble.Scrambler
{
	// Token: 0x02000038 RID: 56
	internal abstract class ScannedItem
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00002790 File Offset: 0x00000990
		private IDictionary<TypeSig, GenericParam> Generics { get; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00002798 File Offset: 0x00000998
		internal IReadOnlyList<TypeSig> TrueTypes
		{
			get
			{
				return this._trueTypes;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600013E RID: 318 RVA: 0x000027A0 File Offset: 0x000009A0
		// (set) Token: 0x0600013F RID: 319 RVA: 0x000027A8 File Offset: 0x000009A8
		private ushort GenericCount { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000027B1 File Offset: 0x000009B1
		internal bool IsScambled
		{
			get
			{
				return this.GenericCount > 0;
			}
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000027BC File Offset: 0x000009BC
		protected ScannedItem(IGenericParameterProvider genericsProvider)
		{
			Debug.Assert(genericsProvider != null, "genericsProvider != null");
			this.GenericCount = 0;
			this.Generics = new Dictionary<TypeSig, GenericParam>();
			this._trueTypes = new List<TypeSig>();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00006F14 File Offset: 0x00005114
		internal bool RegisterGeneric(TypeSig t)
		{
			Debug.Assert(t != null, "t != null");
			t = ScannedItem.GetLeaf(t);
			bool flag = !this.Generics.ContainsKey(t);
			bool result;
			if (flag)
			{
				bool isGenericMethodParameter = t.IsGenericMethodParameter;
				GenericParam value;
				if (isGenericMethodParameter)
				{
					GenericMVar genericMVar = t.ToGenericMVar();
					Debug.Assert(genericMVar != null, "mVar != null");
					value = new GenericParamUser(this.GenericCount, genericMVar.GenericParam.Flags, "T")
					{
						Rid = genericMVar.Rid
					};
				}
				else
				{
					value = new GenericParamUser(this.GenericCount, GenericParamAttributes.NonVariant, "T");
				}
				this.Generics.Add(t, value);
				ushort genericCount = this.GenericCount;
				this.GenericCount = genericCount + 1;
				this._trueTypes.Add(t);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00006FF8 File Offset: 0x000051F8
		internal GenericMVar GetGeneric(TypeSig t)
		{
			Debug.Assert(t != null, "t != null");
			t = ScannedItem.GetLeaf(t);
			GenericMVar result = null;
			GenericParam genericParam;
			bool flag = this.Generics.TryGetValue(t, out genericParam);
			if (flag)
			{
				result = new GenericMVar((int)genericParam.Number);
			}
			return result;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007044 File Offset: 0x00005244
		internal TypeSig ConvertToGenericIfAvalible(TypeSig t)
		{
			Debug.Assert(t != null, "t != null");
			TypeSig typeSig = this.GetGeneric(t);
			bool flag = typeSig != null;
			if (flag)
			{
				bool flag2 = t is NonLeafSig;
				if (flag2)
				{
					Stack<NonLeafSig> stack = new Stack<NonLeafSig>();
					for (NonLeafSig nonLeafSig = t as NonLeafSig; nonLeafSig != null; nonLeafSig = (nonLeafSig.Next as NonLeafSig))
					{
						stack.Push(nonLeafSig);
					}
					while (stack.Any<NonLeafSig>())
					{
						NonLeafSig nonLeafSig = stack.Pop();
						SZArraySig szarraySig = nonLeafSig as SZArraySig;
						bool flag3 = szarraySig != null;
						if (flag3)
						{
							typeSig = new ArraySig(typeSig, szarraySig.Rank, szarraySig.GetSizes(), szarraySig.GetLowerBounds());
						}
						else
						{
							ByRefSig byRefSig = nonLeafSig as ByRefSig;
							bool flag4 = byRefSig != null;
							if (flag4)
							{
								typeSig = new ByRefSig(typeSig);
							}
							else
							{
								CModReqdSig cmodReqdSig = nonLeafSig as CModReqdSig;
								bool flag5 = cmodReqdSig != null;
								if (flag5)
								{
									typeSig = new CModReqdSig(cmodReqdSig.Modifier, typeSig);
								}
								else
								{
									CModOptSig cmodOptSig = nonLeafSig as CModOptSig;
									bool flag6 = cmodOptSig != null;
									if (flag6)
									{
										typeSig = new CModOptSig(cmodOptSig.Modifier, typeSig);
									}
									else
									{
										PtrSig ptrSig = nonLeafSig as PtrSig;
										bool flag7 = ptrSig != null;
										if (flag7)
										{
											typeSig = new PtrSig(typeSig);
										}
										else
										{
											PinnedSig pinnedSig = nonLeafSig as PinnedSig;
											bool flag8 = pinnedSig != null;
											if (flag8)
											{
												typeSig = new PinnedSig(typeSig);
											}
											else
											{
												Debug.Fail("Unexpected leaf signature: " + nonLeafSig.GetType().FullName);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return typeSig ?? t;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000071DC File Offset: 0x000053DC
		private static TypeSig GetLeaf(TypeSig t)
		{
			Debug.Assert(t != null, "t != null");
			for (;;)
			{
				NonLeafSig nonLeafSig = t as NonLeafSig;
				bool flag = nonLeafSig != null;
				if (!flag)
				{
					break;
				}
				t = nonLeafSig.Next;
			}
			return t;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000027F3 File Offset: 0x000009F3
		internal void PrepareGenerics()
		{
			this.PrepareGenerics(from gp in this.Generics.Values
			orderby gp.Number
			select gp);
		}

		// Token: 0x06000147 RID: 327
		protected abstract void PrepareGenerics(IEnumerable<GenericParam> scrambleParams);

		// Token: 0x06000148 RID: 328
		internal abstract IMemberDef GetMemberDef();

		// Token: 0x06000149 RID: 329
		internal abstract void Scan();

		// Token: 0x0600014A RID: 330
		internal abstract ClassOrValueTypeSig GetTarget();

		// Token: 0x04000040 RID: 64
		private readonly List<TypeSig> _trueTypes;
	}
}
