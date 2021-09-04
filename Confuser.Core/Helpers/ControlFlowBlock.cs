using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000A5 RID: 165
	public class ControlFlowBlock
	{
		// Token: 0x060003E3 RID: 995 RVA: 0x0000396A File Offset: 0x00001B6A
		internal ControlFlowBlock(int id, ControlFlowBlockType type, Instruction header, Instruction footer)
		{
			this.Id = id;
			this.Type = type;
			this.Header = header;
			this.Footer = footer;
			this.Sources = new List<ControlFlowBlock>();
			this.Targets = new List<ControlFlowBlock>();
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x000039A9 File Offset: 0x00001BA9
		// (set) Token: 0x060003E5 RID: 997 RVA: 0x000039B1 File Offset: 0x00001BB1
		public IList<ControlFlowBlock> Sources { get; private set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x000039BA File Offset: 0x00001BBA
		// (set) Token: 0x060003E7 RID: 999 RVA: 0x000039C2 File Offset: 0x00001BC2
		public IList<ControlFlowBlock> Targets { get; private set; }

		// Token: 0x060003E8 RID: 1000 RVA: 0x00016A38 File Offset: 0x00014C38
		public override string ToString()
		{
			return string.Format("Block {0} => {1} {2}", this.Id, this.Type, string.Join(", ", (from block in this.Targets
			select block.Id.ToString()).ToArray<string>()));
		}

		// Token: 0x04000283 RID: 643
		public readonly Instruction Footer;

		// Token: 0x04000284 RID: 644
		public readonly Instruction Header;

		// Token: 0x04000285 RID: 645
		public readonly int Id;

		// Token: 0x04000286 RID: 646
		public readonly ControlFlowBlockType Type;
	}
}
