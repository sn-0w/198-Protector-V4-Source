using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000016 RID: 22
	internal class CAMemberReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000A1 RID: 161 RVA: 0x00009E73 File Offset: 0x00008073
		public CAMemberReference(CANamedArgument namedArg, IDnlibDef definition)
		{
			this.namedArg = namedArg;
			this.definition = definition;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00009E8C File Offset: 0x0000808C
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.namedArg.Name = this.definition.Name;
			return true;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00009EB8 File Offset: 0x000080B8
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000044 RID: 68
		private readonly IDnlibDef definition;

		// Token: 0x04000045 RID: 69
		private readonly CANamedArgument namedArg;
	}
}
