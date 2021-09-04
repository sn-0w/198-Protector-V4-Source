using System;
using dnlib.DotNet;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000B9 RID: 185
	public interface IMethodInjectProcessor
	{
		// Token: 0x0600043F RID: 1087
		void Process(MethodDef method);
	}
}
