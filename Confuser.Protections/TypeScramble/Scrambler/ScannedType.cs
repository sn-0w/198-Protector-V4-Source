using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble.Scrambler
{
	// Token: 0x0200003B RID: 59
	internal sealed class ScannedType : ScannedItem
	{
		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00002871 File Offset: 0x00000A71
		// (set) Token: 0x0600015A RID: 346 RVA: 0x00002879 File Offset: 0x00000A79
		internal TypeDef TargetType { get; private set; }

		// Token: 0x0600015B RID: 347 RVA: 0x00002882 File Offset: 0x00000A82
		public ScannedType(TypeDef target) : base(target)
		{
			Debug.Assert(target != null, "target != null");
			this.TargetType = target;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x000078FC File Offset: 0x00005AFC
		internal override void Scan()
		{
			foreach (FieldDef fieldDef in this.TargetType.Fields)
			{
				base.RegisterGeneric(fieldDef.FieldType);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00007958 File Offset: 0x00005B58
		protected override void PrepareGenerics(IEnumerable<GenericParam> scrambleParams)
		{
			Debug.Assert(scrambleParams != null, "scrambleParams != null");
			foreach (GenericParam item in scrambleParams)
			{
				this.TargetType.GenericParameters.Add(item);
			}
			foreach (FieldDef fieldDef in this.TargetType.Fields)
			{
				fieldDef.FieldType = base.ConvertToGenericIfAvalible(fieldDef.FieldType);
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000028A4 File Offset: 0x00000AA4
		internal GenericInstSig CreateGenericTypeSig(ScannedType from)
		{
			return new GenericInstSig(this.GetTarget(), base.TrueTypes.Count);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x000028BC File Offset: 0x00000ABC
		internal override IMemberDef GetMemberDef()
		{
			return this.TargetType;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000028C4 File Offset: 0x00000AC4
		internal override ClassOrValueTypeSig GetTarget()
		{
			return this.TargetType.TryGetClassOrValueTypeSig();
		}
	}
}
