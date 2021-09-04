using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200002F RID: 47
	public interface IReferenceProxyService
	{
		// Token: 0x060000FC RID: 252
		void ExcludeMethod(ConfuserContext context, MethodDef method);

		// Token: 0x060000FD RID: 253
		void ExcludeTarget(ConfuserContext context, MethodDef method);

		// Token: 0x060000FE RID: 254
		bool IsTargeted(ConfuserContext context, MethodDef method);
	}
}
