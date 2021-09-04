using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000087 RID: 135
	internal abstract class BlockBase
	{
		// Token: 0x06000264 RID: 612 RVA: 0x00002E27 File Offset: 0x00001027
		public BlockBase(BlockType type)
		{
			this.Type = type;
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000265 RID: 613 RVA: 0x00002E36 File Offset: 0x00001036
		// (set) Token: 0x06000266 RID: 614 RVA: 0x00002E3E File Offset: 0x0000103E
		public ScopeBlock Parent { get; private set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000267 RID: 615 RVA: 0x00002E47 File Offset: 0x00001047
		// (set) Token: 0x06000268 RID: 616 RVA: 0x00002E4F File Offset: 0x0000104F
		public BlockType Type { get; private set; }

		// Token: 0x06000269 RID: 617
		public abstract void ToBody(CilBody body);
	}
}
