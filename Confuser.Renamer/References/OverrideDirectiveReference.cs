using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001C RID: 28
	internal class OverrideDirectiveReference : INameReference<MethodDef>, INameReference
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x0000A1EF File Offset: 0x000083EF
		public OverrideDirectiveReference(VTableSlot thisSlot, VTableSlot baseSlot)
		{
			this.thisSlot = thisSlot;
			this.baseSlot = baseSlot;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000A207 File Offset: 0x00008407
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			return true;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x0000A20A File Offset: 0x0000840A
		public bool ShouldCancelRename()
		{
			return this.baseSlot.MethodDefDeclType is GenericInstSig && this.thisSlot.MethodDef.Module.IsClr20;
		}

		// Token: 0x04000055 RID: 85
		private readonly VTableSlot baseSlot;

		// Token: 0x04000056 RID: 86
		private readonly VTableSlot thisSlot;
	}
}
