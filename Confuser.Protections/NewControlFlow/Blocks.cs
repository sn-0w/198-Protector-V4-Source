using System;
using System.Collections.Generic;
using System.Linq;

namespace Confuser.Protections.NewControlFlow
{
	// Token: 0x0200007B RID: 123
	public class Blocks
	{
		// Token: 0x0600023A RID: 570 RVA: 0x0000D9E8 File Offset: 0x0000BBE8
		public Block getBlock(int id)
		{
			return this.blocks.Single((Block block) => block.ID == id);
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000DA1C File Offset: 0x0000BC1C
		public void Scramble(out Blocks incGroups)
		{
			Blocks blocks = new Blocks();
			foreach (Block item in this.blocks)
			{
				blocks.blocks.Insert(new Random(Guid.NewGuid().GetHashCode()).Next(blocks.blocks.Count), item);
			}
			incGroups = blocks;
		}

		// Token: 0x040000CD RID: 205
		public List<Block> blocks = new List<Block>();
	}
}
