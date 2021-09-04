using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x02000080 RID: 128
	internal class TraceService : ITraceService
	{
		// Token: 0x0600030D RID: 781 RVA: 0x000034CB File Offset: 0x000016CB
		public TraceService(ConfuserContext context)
		{
			this.context = context;
		}

		// Token: 0x0600030E RID: 782 RVA: 0x00013900 File Offset: 0x00011B00
		public MethodTrace Trace(MethodDef method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new ArgumentNullException("method");
			}
			return this._cache.GetValueOrDefaultLazy(method, (MethodDef m) => this._cache[m] = new MethodTrace(m)).Trace();
		}

		// Token: 0x0400023A RID: 570
		private readonly Dictionary<MethodDef, MethodTrace> _cache = new Dictionary<MethodDef, MethodTrace>();

		// Token: 0x0400023B RID: 571
		private ConfuserContext context;
	}
}
