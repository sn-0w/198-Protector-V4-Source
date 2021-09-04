using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000015 RID: 21
	internal class BAMLTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00009E19 File Offset: 0x00008019
		public BAMLTypeReference(TypeSig sig, TypeInfoRecord rec)
		{
			this.sig = sig;
			this.rec = rec;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00009E34 File Offset: 0x00008034
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.rec.TypeFullName = this.sig.ReflectionFullName;
			return true;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00009E60 File Offset: 0x00008060
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000042 RID: 66
		private readonly TypeInfoRecord rec;

		// Token: 0x04000043 RID: 67
		private readonly TypeSig sig;
	}
}
