using System;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DA RID: 218
	public abstract class iArithmetic
	{
		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600039E RID: 926
		public abstract string Name { get; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600039F RID: 927
		public abstract string Description { get; }

		// Token: 0x060003A0 RID: 928
		public abstract void Init();
	}
}
