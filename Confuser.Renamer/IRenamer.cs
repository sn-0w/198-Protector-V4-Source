using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000006 RID: 6
	public interface IRenamer
	{
		// Token: 0x0600001B RID: 27
		void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);

		// Token: 0x0600001C RID: 28
		void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);

		// Token: 0x0600001D RID: 29
		void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def);
	}
}
