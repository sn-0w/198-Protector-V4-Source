using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x0200008A RID: 138
	internal class InstrBlock : BlockBase
	{
		// Token: 0x06000273 RID: 627 RVA: 0x00002E95 File Offset: 0x00001095
		public InstrBlock() : base(BlockType.Normal)
		{
			this.Instructions = new List<Instruction>();
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000274 RID: 628 RVA: 0x00002EA9 File Offset: 0x000010A9
		// (set) Token: 0x06000275 RID: 629 RVA: 0x00002EB1 File Offset: 0x000010B1
		public List<Instruction> Instructions { get; set; }

		// Token: 0x06000276 RID: 630 RVA: 0x0000F964 File Offset: 0x0000DB64
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Instruction instruction in this.Instructions)
			{
				stringBuilder.AppendLine(instruction.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000F9CC File Offset: 0x0000DBCC
		public override void ToBody(CilBody body)
		{
			foreach (Instruction item in this.Instructions)
			{
				body.Instructions.Add(item);
			}
		}
	}
}
