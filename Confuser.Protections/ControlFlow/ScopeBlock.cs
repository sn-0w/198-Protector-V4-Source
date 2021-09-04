using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000089 RID: 137
	internal class ScopeBlock : BlockBase
	{
		// Token: 0x0600026A RID: 618 RVA: 0x00002E58 File Offset: 0x00001058
		public ScopeBlock(BlockType type, ExceptionHandler handler) : base(type)
		{
			this.Handler = handler;
			this.Children = new List<BlockBase>();
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600026B RID: 619 RVA: 0x00002E73 File Offset: 0x00001073
		// (set) Token: 0x0600026C RID: 620 RVA: 0x00002E7B File Offset: 0x0000107B
		public ExceptionHandler Handler { get; private set; }

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x0600026D RID: 621 RVA: 0x00002E84 File Offset: 0x00001084
		// (set) Token: 0x0600026E RID: 622 RVA: 0x00002E8C File Offset: 0x0000108C
		public List<BlockBase> Children { get; set; }

		// Token: 0x0600026F RID: 623 RVA: 0x0000F738 File Offset: 0x0000D938
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (base.Type == BlockType.Try)
			{
				stringBuilder.Append("try ");
			}
			else if (base.Type == BlockType.Handler)
			{
				stringBuilder.Append("handler ");
			}
			else if (base.Type == BlockType.Finally)
			{
				stringBuilder.Append("finally ");
			}
			else if (base.Type == BlockType.Fault)
			{
				stringBuilder.Append("fault ");
			}
			stringBuilder.AppendLine("{");
			foreach (BlockBase value in this.Children)
			{
				stringBuilder.Append(value);
			}
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000F814 File Offset: 0x0000DA14
		public Instruction GetFirstInstr()
		{
			BlockBase blockBase = this.Children.First<BlockBase>();
			if (blockBase is ScopeBlock)
			{
				return ((ScopeBlock)blockBase).GetFirstInstr();
			}
			return ((InstrBlock)blockBase).Instructions.First<Instruction>();
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000F854 File Offset: 0x0000DA54
		public Instruction GetLastInstr()
		{
			BlockBase blockBase = this.Children.Last<BlockBase>();
			if (blockBase is ScopeBlock)
			{
				return ((ScopeBlock)blockBase).GetLastInstr();
			}
			return ((InstrBlock)blockBase).Instructions.Last<Instruction>();
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000F894 File Offset: 0x0000DA94
		public override void ToBody(CilBody body)
		{
			if (base.Type > BlockType.Normal)
			{
				if (base.Type == BlockType.Try)
				{
					this.Handler.TryStart = this.GetFirstInstr();
					this.Handler.TryEnd = this.GetLastInstr();
				}
				else if (base.Type != BlockType.Filter)
				{
					this.Handler.HandlerStart = this.GetFirstInstr();
					this.Handler.HandlerEnd = this.GetLastInstr();
				}
				else
				{
					this.Handler.FilterStart = this.GetFirstInstr();
				}
			}
			foreach (BlockBase blockBase in this.Children)
			{
				blockBase.ToBody(body);
			}
		}
	}
}
