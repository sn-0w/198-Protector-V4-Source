using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007A RID: 122
	public interface IRuntimeService
	{
		// Token: 0x060002F2 RID: 754
		TypeDef GetRuntimeType(string fullName);
	}
}
