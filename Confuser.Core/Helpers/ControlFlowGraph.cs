using System;
using System.Collections;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000A3 RID: 163
	public class ControlFlowGraph : IEnumerable<ControlFlowBlock>, IEnumerable
	{
		// Token: 0x060003D7 RID: 983 RVA: 0x00016240 File Offset: 0x00014440
		private ControlFlowGraph(CilBody body)
		{
			this.body = body;
			this.instrBlocks = new int[body.Instructions.Count];
			this.blocks = new List<ControlFlowBlock>();
			this.indexMap = new Dictionary<Instruction, int>();
			for (int i = 0; i < body.Instructions.Count; i++)
			{
				this.indexMap.Add(body.Instructions[i], i);
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x000162BC File Offset: 0x000144BC
		public int Count
		{
			get
			{
				return this.blocks.Count;
			}
		}

		// Token: 0x1700008A RID: 138
		public ControlFlowBlock this[int id]
		{
			get
			{
				return this.blocks[id];
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060003DA RID: 986 RVA: 0x000162FC File Offset: 0x000144FC
		public CilBody Body
		{
			get
			{
				return this.body;
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00016314 File Offset: 0x00014514
		IEnumerator<ControlFlowBlock> IEnumerable<ControlFlowBlock>.GetEnumerator()
		{
			return this.blocks.GetEnumerator();
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00016338 File Offset: 0x00014538
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.blocks.GetEnumerator();
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0001635C File Offset: 0x0001455C
		public ControlFlowBlock GetContainingBlock(int instrIndex)
		{
			return this.blocks[this.instrBlocks[instrIndex]];
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00016384 File Offset: 0x00014584
		public int IndexOf(Instruction instr)
		{
			return this.indexMap[instr];
		}

		// Token: 0x060003DF RID: 991 RVA: 0x000163A4 File Offset: 0x000145A4
		private void PopulateBlockHeaders(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
		{
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instruction = this.body.Instructions[i];
				bool flag = instruction.Operand is Instruction;
				if (flag)
				{
					blockHeaders.Add((Instruction)instruction.Operand);
					bool flag2 = i + 1 < this.body.Instructions.Count;
					if (flag2)
					{
						blockHeaders.Add(this.body.Instructions[i + 1]);
					}
				}
				else
				{
					bool flag3 = instruction.Operand is Instruction[];
					if (flag3)
					{
						foreach (Instruction item in (Instruction[])instruction.Operand)
						{
							blockHeaders.Add(item);
						}
						bool flag4 = i + 1 < this.body.Instructions.Count;
						if (flag4)
						{
							blockHeaders.Add(this.body.Instructions[i + 1]);
						}
					}
					else
					{
						bool flag5 = (instruction.OpCode.FlowControl == FlowControl.Throw || instruction.OpCode.FlowControl == FlowControl.Return) && i + 1 < this.body.Instructions.Count;
						if (flag5)
						{
							blockHeaders.Add(this.body.Instructions[i + 1]);
						}
					}
				}
			}
			blockHeaders.Add(this.body.Instructions[0]);
			foreach (ExceptionHandler exceptionHandler in this.body.ExceptionHandlers)
			{
				blockHeaders.Add(exceptionHandler.TryStart);
				blockHeaders.Add(exceptionHandler.HandlerStart);
				blockHeaders.Add(exceptionHandler.FilterStart);
				entryHeaders.Add(exceptionHandler.HandlerStart);
				entryHeaders.Add(exceptionHandler.FilterStart);
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000165C8 File Offset: 0x000147C8
		private void SplitBlocks(HashSet<Instruction> blockHeaders, HashSet<Instruction> entryHeaders)
		{
			int num = 0;
			int num2 = -1;
			Instruction instruction = null;
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instruction2 = this.body.Instructions[i];
				bool flag = blockHeaders.Contains(instruction2);
				if (flag)
				{
					bool flag2 = instruction != null;
					if (flag2)
					{
						Instruction instruction3 = this.body.Instructions[i - 1];
						ControlFlowBlockType controlFlowBlockType = ControlFlowBlockType.Normal;
						bool flag3 = entryHeaders.Contains(instruction) || instruction == this.body.Instructions[0];
						if (flag3)
						{
							controlFlowBlockType |= ControlFlowBlockType.Entry;
						}
						bool flag4 = instruction3.OpCode.FlowControl == FlowControl.Return || instruction3.OpCode.FlowControl == FlowControl.Throw;
						if (flag4)
						{
							controlFlowBlockType |= ControlFlowBlockType.Exit;
						}
						this.blocks.Add(new ControlFlowBlock(num2, controlFlowBlockType, instruction, instruction3));
					}
					num2 = num++;
					instruction = instruction2;
				}
				this.instrBlocks[i] = num2;
			}
			bool flag5 = this.blocks.Count == 0 || this.blocks[this.blocks.Count - 1].Id != num2;
			if (flag5)
			{
				Instruction instruction4 = this.body.Instructions[this.body.Instructions.Count - 1];
				ControlFlowBlockType controlFlowBlockType2 = ControlFlowBlockType.Normal;
				bool flag6 = entryHeaders.Contains(instruction) || instruction == this.body.Instructions[0];
				if (flag6)
				{
					controlFlowBlockType2 |= ControlFlowBlockType.Entry;
				}
				bool flag7 = instruction4.OpCode.FlowControl == FlowControl.Return || instruction4.OpCode.FlowControl == FlowControl.Throw;
				if (flag7)
				{
					controlFlowBlockType2 |= ControlFlowBlockType.Exit;
				}
				this.blocks.Add(new ControlFlowBlock(num2, controlFlowBlockType2, instruction, instruction4));
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000167AC File Offset: 0x000149AC
		private void LinkBlocks()
		{
			for (int i = 0; i < this.body.Instructions.Count; i++)
			{
				Instruction instruction = this.body.Instructions[i];
				bool flag = instruction.Operand is Instruction;
				if (flag)
				{
					ControlFlowBlock controlFlowBlock = this.blocks[this.instrBlocks[i]];
					ControlFlowBlock controlFlowBlock2 = this.blocks[this.instrBlocks[this.indexMap[(Instruction)instruction.Operand]]];
					controlFlowBlock2.Sources.Add(controlFlowBlock);
					controlFlowBlock.Targets.Add(controlFlowBlock2);
				}
				else
				{
					bool flag2 = instruction.Operand is Instruction[];
					if (flag2)
					{
						foreach (Instruction key in (Instruction[])instruction.Operand)
						{
							ControlFlowBlock controlFlowBlock3 = this.blocks[this.instrBlocks[i]];
							ControlFlowBlock controlFlowBlock4 = this.blocks[this.instrBlocks[this.indexMap[key]]];
							controlFlowBlock4.Sources.Add(controlFlowBlock3);
							controlFlowBlock3.Targets.Add(controlFlowBlock4);
						}
					}
				}
			}
			for (int k = 0; k < this.blocks.Count; k++)
			{
				bool flag3 = this.blocks[k].Footer.OpCode.FlowControl != FlowControl.Branch && this.blocks[k].Footer.OpCode.FlowControl != FlowControl.Return && this.blocks[k].Footer.OpCode.FlowControl != FlowControl.Throw;
				if (flag3)
				{
					this.blocks[k].Targets.Add(this.blocks[k + 1]);
					this.blocks[k + 1].Sources.Add(this.blocks[k]);
				}
			}
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x000169E0 File Offset: 0x00014BE0
		public static ControlFlowGraph Construct(CilBody body)
		{
			ControlFlowGraph controlFlowGraph = new ControlFlowGraph(body);
			bool flag = body.Instructions.Count == 0;
			ControlFlowGraph result;
			if (flag)
			{
				result = controlFlowGraph;
			}
			else
			{
				HashSet<Instruction> blockHeaders = new HashSet<Instruction>();
				HashSet<Instruction> entryHeaders = new HashSet<Instruction>();
				controlFlowGraph.PopulateBlockHeaders(blockHeaders, entryHeaders);
				controlFlowGraph.SplitBlocks(blockHeaders, entryHeaders);
				controlFlowGraph.LinkBlocks();
				result = controlFlowGraph;
			}
			return result;
		}

		// Token: 0x0400027B RID: 635
		private readonly List<ControlFlowBlock> blocks;

		// Token: 0x0400027C RID: 636
		private readonly CilBody body;

		// Token: 0x0400027D RID: 637
		private readonly int[] instrBlocks;

		// Token: 0x0400027E RID: 638
		private readonly Dictionary<Instruction, int> indexMap;
	}
}
