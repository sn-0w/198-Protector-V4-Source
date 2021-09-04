using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000094 RID: 148
	internal class JumpMangler : ManglerBase
	{
		// Token: 0x06000299 RID: 665 RVA: 0x00010830 File Offset: 0x0000EA30
		private LinkedList<Instruction[]> SpiltFragments(InstrBlock block, CFContext ctx)
		{
			LinkedList<Instruction[]> linkedList = new LinkedList<Instruction[]>();
			List<Instruction> list = new List<Instruction>();
			int num = -1;
			int i = 0;
			while (i < block.Instructions.Count)
			{
				if (num == -1)
				{
					goto IL_5A;
				}
				if (num <= 0)
				{
					linkedList.AddLast(list.ToArray());
					list.Clear();
					num = -1;
					goto IL_5A;
				}
				list.Add(block.Instructions[i]);
				num--;
				IL_51:
				i++;
				continue;
				IL_5A:
				if (block.Instructions[i].OpCode.OpCodeType == OpCodeType.Prefix)
				{
					num = 1;
					list.Add(block.Instructions[i]);
				}
				if (i + 2 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Dup && block.Instructions[i + 1].OpCode.Code == Code.Ldvirtftn && block.Instructions[i + 2].OpCode.Code == Code.Newobj)
				{
					num = 2;
					list.Add(block.Instructions[i]);
				}
				if (i + 4 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldc_I4 && block.Instructions[i + 1].OpCode.Code == Code.Newarr && block.Instructions[i + 2].OpCode.Code == Code.Dup && block.Instructions[i + 3].OpCode.Code == Code.Ldtoken && block.Instructions[i + 4].OpCode.Code == Code.Call)
				{
					num = 4;
					list.Add(block.Instructions[i]);
				}
				if (i + 1 < block.Instructions.Count && block.Instructions[i].OpCode.Code == Code.Ldftn && block.Instructions[i + 1].OpCode.Code == Code.Newobj)
				{
					num = 1;
					list.Add(block.Instructions[i]);
				}
				list.Add(block.Instructions[i]);
				if (ctx.Intensity > ctx.Random.NextDouble())
				{
					linkedList.AddLast(list.ToArray());
					list.Clear();
					goto IL_51;
				}
				goto IL_51;
			}
			if (list.Count > 0)
			{
				linkedList.AddLast(list.ToArray());
			}
			return linkedList;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00010AD0 File Offset: 0x0000ECD0
		public override void Mangle(CilBody body, ScopeBlock root, CFContext ctx)
		{
			ushort maxStack = body.MaxStack;
			foreach (InstrBlock instrBlock in ManglerBase.GetAllBlocks(root))
			{
				LinkedList<Instruction[]> linkedList = this.SpiltFragments(instrBlock, ctx);
				if (linkedList.Count >= 4)
				{
					LinkedListNode<Instruction[]> linkedListNode = linkedList.First;
					while (linkedListNode.Next != null)
					{
						List<Instruction> list = new List<Instruction>(linkedListNode.Value);
						ctx.AddJump(list, linkedListNode.Next.Value[0]);
						ctx.AddJunk(list);
						linkedListNode.Value = list.ToArray();
						linkedListNode = linkedListNode.Next;
					}
					Instruction[] value = linkedList.First.Value;
					linkedList.RemoveFirst();
					Instruction[] value2 = linkedList.Last.Value;
					linkedList.RemoveLast();
					List<Instruction[]> list2 = linkedList.ToList<Instruction[]>();
					ctx.Random.Shuffle<Instruction[]>(list2);
					instrBlock.Instructions = value.Concat(list2.SelectMany((Instruction[] fragment) => fragment)).Concat(value2).ToList<Instruction>();
				}
			}
		}
	}
}
