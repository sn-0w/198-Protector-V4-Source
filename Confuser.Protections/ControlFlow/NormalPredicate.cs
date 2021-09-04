using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x0200008D RID: 141
	internal class NormalPredicate : IPredicate
	{
		// Token: 0x06000280 RID: 640 RVA: 0x00002EC8 File Offset: 0x000010C8
		public NormalPredicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000281 RID: 641 RVA: 0x00002ED7 File Offset: 0x000010D7
		public void Init(CilBody body)
		{
			if (!this.inited)
			{
				this.xorKey = this.ctx.Random.NextInt32();
				this.inited = true;
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00002F01 File Offset: 0x00001101
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Ldc_I4, this.xorKey));
			instrs.Add(Instruction.Create(OpCodes.Xor));
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00002F29 File Offset: 0x00001129
		public int GetSwitchKey(int key)
		{
			return key ^ this.xorKey;
		}

		// Token: 0x040000F2 RID: 242
		private readonly CFContext ctx;

		// Token: 0x040000F3 RID: 243
		private bool inited;

		// Token: 0x040000F4 RID: 244
		private int xorKey;
	}
}
