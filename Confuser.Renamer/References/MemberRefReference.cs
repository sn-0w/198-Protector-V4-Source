using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001D RID: 29
	public class MemberRefReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x0000A236 File Offset: 0x00008436
		public MemberRefReference(MemberRef memberRef, IDnlibDef memberDef)
		{
			this.memberRef = memberRef;
			this.memberDef = memberDef;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x0000A250 File Offset: 0x00008450
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.memberRef.Name = this.memberDef.Name;
			return true;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000A27C File Offset: 0x0000847C
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000057 RID: 87
		private readonly IDnlibDef memberDef;

		// Token: 0x04000058 RID: 88
		private readonly MemberRef memberRef;
	}
}
