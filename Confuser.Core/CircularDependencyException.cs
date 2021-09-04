using System;
using System.Diagnostics;

namespace Confuser.Core
{
	// Token: 0x02000044 RID: 68
	internal class CircularDependencyException : Exception
	{
		// Token: 0x0600016D RID: 365 RVA: 0x00002A07 File Offset: 0x00000C07
		internal CircularDependencyException(Protection a, Protection b) : base(string.Format("The protections '{0}' and '{1}' has a circular dependency between them.", a, b))
		{
			Debug.Assert(a != null);
			Debug.Assert(b != null);
			this.ProtectionA = a;
			this.ProtectionB = b;
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600016E RID: 366 RVA: 0x00002A41 File Offset: 0x00000C41
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00002A49 File Offset: 0x00000C49
		public Protection ProtectionA { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00002A52 File Offset: 0x00000C52
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00002A5A File Offset: 0x00000C5A
		public Protection ProtectionB { get; private set; }
	}
}
