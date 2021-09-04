using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Services
{
	// Token: 0x02000078 RID: 120
	public sealed class MethodTrace
	{
		// Token: 0x060002E2 RID: 738 RVA: 0x000033C6 File Offset: 0x000015C6
		internal MethodTrace(MethodDef method)
		{
			this.Method = method;
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x000033D6 File Offset: 0x000015D6
		private MethodDef Method { get; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x000033DE File Offset: 0x000015DE
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x000033E6 File Offset: 0x000015E6
		private Instruction[] Instructions { get; set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000033EF File Offset: 0x000015EF
		public Func<uint, int> OffsetToIndexMap
		{
			get
			{
				return (uint offset) => this._offset2Index[offset];
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x000033FD File Offset: 0x000015FD
		// (set) Token: 0x060002E8 RID: 744 RVA: 0x00003405 File Offset: 0x00001605
		private int[] BeforeStackDepths { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000340E File Offset: 0x0000160E
		// (set) Token: 0x060002EA RID: 746 RVA: 0x00003416 File Offset: 0x00001616
		private int[] AfterStackDepths { get; set; }

		// Token: 0x060002EB RID: 747 RVA: 0x0000341F File Offset: 0x0000161F
		public bool IsBranchTarget(int instrIndex)
		{
			return this._fromInstrs.ContainsKey(instrIndex);
		}

		// Token: 0x060002EC RID: 748 RVA: 0x000129B8 File Offset: 0x00010BB8
		internal MethodTrace Trace()
		{
			CilBody body = this.Method.Body;
			this.Method.Body.UpdateInstructionOffsets();
			Instruction[] array = this.Instructions = this.Method.Body.Instructions.ToArray<Instruction>();
			this._offset2Index = new Dictionary<uint, int>();
			int[] array2 = new int[array.Length];
			int[] array3 = new int[array.Length];
			this._fromInstrs = new Dictionary<int, List<Instruction>>();
			for (int i = 0; i < array.Length; i++)
			{
				this._offset2Index.Add(array[i].Offset, i);
				array2[i] = int.MinValue;
			}
			foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
			{
				array2[this.OffsetToIndexMap(exceptionHandler.TryStart.Offset)] = 0;
				array2[this.OffsetToIndexMap(exceptionHandler.HandlerStart.Offset)] = ((exceptionHandler.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
				bool flag = exceptionHandler.FilterStart != null;
				if (flag)
				{
					array2[this.OffsetToIndexMap(exceptionHandler.FilterStart.Offset)] = 1;
				}
			}
			int num = 0;
			int j = 0;
			while (j < array.Length)
			{
				Instruction instruction = array[j];
				bool flag2 = array2[j] != int.MinValue;
				if (flag2)
				{
					num = array2[j];
				}
				array2[j] = num;
				instruction.UpdateStack(ref num);
				array3[j] = num;
				switch (instruction.OpCode.FlowControl)
				{
				case FlowControl.Branch:
				{
					int num2 = this.OffsetToIndexMap(((Instruction)instruction.Operand).Offset);
					bool flag3 = array2[num2] == int.MinValue;
					if (flag3)
					{
						array2[num2] = num;
					}
					this._fromInstrs.AddListEntry(this.OffsetToIndexMap(((Instruction)instruction.Operand).Offset), instruction);
					num = 0;
					break;
				}
				case FlowControl.Break:
					break;
				case FlowControl.Call:
				{
					bool flag4 = instruction.OpCode.Code == Code.Jmp;
					if (flag4)
					{
						num = 0;
					}
					break;
				}
				case FlowControl.Cond_Branch:
				{
					bool flag5 = instruction.OpCode.Code == Code.Switch;
					if (flag5)
					{
						foreach (Instruction instruction2 in (Instruction[])instruction.Operand)
						{
							int num3 = this.OffsetToIndexMap(instruction2.Offset);
							bool flag6 = array2[num3] == int.MinValue;
							if (flag6)
							{
								array2[num3] = num;
							}
							this._fromInstrs.AddListEntry(this.OffsetToIndexMap(instruction2.Offset), instruction);
						}
					}
					else
					{
						int num4 = this.OffsetToIndexMap(((Instruction)instruction.Operand).Offset);
						bool flag7 = array2[num4] == int.MinValue;
						if (flag7)
						{
							array2[num4] = num;
						}
						this._fromInstrs.AddListEntry(this.OffsetToIndexMap(((Instruction)instruction.Operand).Offset), instruction);
					}
					break;
				}
				case FlowControl.Meta:
					break;
				case FlowControl.Next:
					break;
				case FlowControl.Phi:
					goto IL_349;
				case FlowControl.Return:
					break;
				case FlowControl.Throw:
					break;
				default:
					goto IL_349;
				}
				j++;
				continue;
				IL_349:
				throw new UnreachableException();
			}
			foreach (int num5 in array2)
			{
				bool flag8 = num5 == int.MinValue;
				if (flag8)
				{
					throw new InvalidMethodException("Bad method body.");
				}
			}
			foreach (int num6 in array3)
			{
				bool flag9 = num6 == int.MinValue;
				if (flag9)
				{
					throw new InvalidMethodException("Bad method body.");
				}
			}
			this.BeforeStackDepths = array2;
			this.AfterStackDepths = array3;
			return this;
		}

		// Token: 0x060002ED RID: 749 RVA: 0x00012DC4 File Offset: 0x00010FC4
		public int[] TraceArguments(Instruction instr)
		{
			bool flag = instr.OpCode.Code != Code.Call && instr.OpCode.Code != Code.Callvirt && instr.OpCode.Code != Code.Newobj;
			if (flag)
			{
				throw new ArgumentException("Invalid call instruction.", "instr");
			}
			int num;
			int num2;
			instr.CalculateStackUsage(out num, out num2);
			bool flag2 = num2 == 0;
			int[] result;
			if (flag2)
			{
				result = new int[0];
			}
			else
			{
				int num3 = this.OffsetToIndexMap(instr.Offset);
				int num4 = num2;
				int num5 = this.BeforeStackDepths[num3] - num4;
				int num6 = -1;
				HashSet<uint> hashSet = new HashSet<uint>();
				Queue<int> queue = new Queue<int>();
				queue.Enqueue(this.OffsetToIndexMap(instr.Offset) - 1);
				while (queue.Count > 0)
				{
					int i;
					for (i = queue.Dequeue(); i >= 0; i--)
					{
						bool flag3 = this.BeforeStackDepths[i] == num5;
						if (flag3)
						{
							break;
						}
						bool flag4 = this._fromInstrs.ContainsKey(i);
						if (flag4)
						{
							foreach (Instruction instruction in this._fromInstrs[i])
							{
								bool flag5 = !hashSet.Contains(instruction.Offset);
								if (flag5)
								{
									hashSet.Add(instruction.Offset);
									queue.Enqueue(this.OffsetToIndexMap(instruction.Offset));
								}
							}
						}
					}
					bool flag6 = i < 0;
					if (flag6)
					{
						return null;
					}
					bool flag7 = num6 == -1;
					if (flag7)
					{
						num6 = i;
					}
					else
					{
						bool flag8 = num6 != i;
						if (flag8)
						{
							return null;
						}
					}
				}
				while (this.Instructions[num6].OpCode.Code == Code.Dup)
				{
					num6--;
				}
				hashSet.Clear();
				Queue<ValueTuple<int, Stack<int>>> queue2 = new Queue<ValueTuple<int, Stack<int>>>();
				queue2.Clear();
				queue2.Enqueue(new ValueTuple<int, Stack<int>>(num6, new Stack<int>()));
				int[] array = null;
				while (queue2.Count > 0)
				{
					ValueTuple<int, Stack<int>> valueTuple = queue2.Dequeue();
					int num7 = valueTuple.Item1;
					Stack<int> item = valueTuple.Item2;
					while (num7 != num3 && num7 < this.Instructions.Length)
					{
						Instruction instruction2 = this.Instructions[num7];
						instruction2.CalculateStackUsage(out num, out num2);
						bool flag9 = instruction2.OpCode.Code == Code.Dup;
						if (flag9)
						{
							int item2 = item.Pop();
							item.Push(item2);
							item.Push(item2);
						}
						else
						{
							int num8 = num - num2;
							bool flag10 = num8 < 0;
							if (flag10)
							{
								for (int j = 0; j < -num8; j++)
								{
									item.Pop();
								}
							}
							else
							{
								for (int k = 0; k < num8; k++)
								{
									item.Push(num7);
								}
							}
						}
						object operand = instruction2.Operand;
						object obj = operand;
						Instruction instruction3 = obj as Instruction;
						if (instruction3 == null)
						{
							Instruction[] array2 = obj as Instruction[];
							if (array2 == null)
							{
								num7++;
							}
							else
							{
								foreach (Instruction instruction4 in array2)
								{
									queue2.Enqueue(new ValueTuple<int, Stack<int>>(this.OffsetToIndexMap(instruction4.Offset), new Stack<int>(item)));
								}
								num7++;
							}
						}
						else
						{
							int num9 = this.OffsetToIndexMap(instruction3.Offset);
							bool flag11 = instruction2.OpCode.FlowControl == FlowControl.Branch;
							if (flag11)
							{
								num7 = num9;
							}
							else
							{
								queue2.Enqueue(new ValueTuple<int, Stack<int>>(num9, new Stack<int>(item)));
								num7++;
							}
						}
					}
					bool flag12 = item.Count > num4;
					if (flag12)
					{
						int[] source = item.ToArray();
						item.Clear();
						foreach (int item3 in source.Take(num4).Reverse<int>())
						{
							item.Push(item3);
						}
					}
					bool flag13 = item.Count != num4;
					if (flag13)
					{
						return null;
					}
					bool flag14 = array != null && !item.SequenceEqual(array);
					if (flag14)
					{
						return null;
					}
					array = item.ToArray();
				}
				bool flag15 = array == null;
				if (flag15)
				{
					result = null;
				}
				else
				{
					Array.Reverse(array);
					result = array;
				}
			}
			return result;
		}

		// Token: 0x0400022A RID: 554
		private Dictionary<int, List<Instruction>> _fromInstrs;

		// Token: 0x0400022B RID: 555
		private Dictionary<uint, int> _offset2Index;
	}
}
