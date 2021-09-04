using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000118 RID: 280
	public interface IMildReferenceProxyService
	{
		// Token: 0x060004B7 RID: 1207
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
