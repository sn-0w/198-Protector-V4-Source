using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000081 RID: 129
	public interface ITraceService
	{
		// Token: 0x06000310 RID: 784
		MethodTrace Trace(MethodDef method);
	}
}
