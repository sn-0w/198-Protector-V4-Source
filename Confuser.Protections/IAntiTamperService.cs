using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000006 RID: 6
	public interface IAntiTamperService
	{
		// Token: 0x0600001B RID: 27
		void ExcludeMethod(ConfuserContext context, MethodDef method);
	}
}
