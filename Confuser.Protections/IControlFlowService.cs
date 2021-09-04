using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000016 RID: 22
	public interface IControlFlowService
	{
		// Token: 0x06000070 RID: 112
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
