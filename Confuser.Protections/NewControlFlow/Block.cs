using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.NewControlFlow
{
	// Token: 0x0200007A RID: 122
	public class Block
	{
		// Token: 0x040000CA RID: 202
		public int ID = 0;

		// Token: 0x040000CB RID: 203
		public int nextBlock = 0;

		// Token: 0x040000CC RID: 204
		public List<Instruction> instructions = new List<Instruction>();
	}
}
