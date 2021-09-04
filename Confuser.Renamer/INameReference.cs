using System;
using Confuser.Core;

namespace Confuser.Renamer
{
	// Token: 0x02000008 RID: 8
	public interface INameReference
	{
		// Token: 0x06000027 RID: 39
		bool UpdateNameReference(ConfuserContext context, INameService service);

		// Token: 0x06000028 RID: 40
		bool ShouldCancelRename();
	}
}
