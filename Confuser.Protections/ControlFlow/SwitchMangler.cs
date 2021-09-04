using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000098 RID: 152
	internal class SwitchMangler : ManglerBase
	{
		// Token: 0x060002AC RID: 684 RVA: 0x00010DE0 File Offset: 0x0000EFE0
		private LinkedList<Instruction[]> SplitStatements(InstrBlock block, SwitchMangler.Trace trace, CFContext ctx)
		{
			LinkedList<Instruction[]> linkedList = new LinkedList<Instruction[]>();
			List<Instruction> list = new List<Instruction>();
			HashSet<Instruction> hashSet = new HashSet<Instruction>();
			for (int i = 0; i < block.Instructions.Count; i++)
			{
				Instruction instruction = block.Instructions[i];
				list.Add(instruction);
				bool flag = i + 1 < block.Instructions.Count && trace.HasMultipleSources(block.Instructions[i + 1].Offset);
				FlowControl flowControl = instruction.OpCode.FlowControl;
				FlowControl flowControl2 = flowControl;
				if (flowControl2 == FlowControl.Branch || flowControl2 == FlowControl.Cond_Branch || flowControl2 - FlowControl.Return <= 1)
				{
					flag = true;
					bool flag2 = trace.AfterStack[instruction.Offset] != 0;
					if (flag2)
					{
						object operand = instruction.Operand;
						Instruction instruction2 = operand as Instruction;
						bool flag3 = instruction2 != null;
						if (flag3)
						{
							hashSet.Add(instruction2);
						}
						else
						{
							object operand2 = instruction.Operand;
							Instruction[] array = operand2 as Instruction[];
							bool flag4 = array != null;
							if (flag4)
							{
								foreach (Instruction item in array)
								{
									hashSet.Add(item);
								}
							}
						}
					}
				}
				hashSet.Remove(instruction);
				bool flag5 = instruction.OpCode.OpCodeType != OpCodeType.Prefix && trace.AfterStack[instruction.Offset] == 0 && hashSet.Count == 0 && (flag || ctx.Intensity > ctx.Random.NextDouble()) && (i == 0 || block.Instructions[i - 1].OpCode.Code != Code.Tailcall);
				if (flag5)
				{
					linkedList.AddLast(list.ToArray());
					list.Clear();
				}
			}
			bool flag6 = list.Count > 0;
			if (flag6)
			{
				linkedList.AddLast(list.ToArray());
			}
			return linkedList;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x00010FDC File Offset: 0x0000F1DC
		private static OpCode InverseBranch(OpCode opCode)
		{
			OpCode result;
			switch (opCode.Code)
			{
			case Code.Brfalse:
				result = OpCodes.Brtrue;
				break;
			case Code.Brtrue:
				result = OpCodes.Brfalse;
				break;
			case Code.Beq:
				result = OpCodes.Bne_Un;
				break;
			case Code.Bge:
				result = OpCodes.Blt;
				break;
			case Code.Bgt:
				result = OpCodes.Ble;
				break;
			case Code.Ble:
				result = OpCodes.Bgt;
				break;
			case Code.Blt:
				result = OpCodes.Bge;
				break;
			case Code.Bne_Un:
				result = OpCodes.Beq;
				break;
			case Code.Bge_Un:
				result = OpCodes.Blt_Un;
				break;
			case Code.Bgt_Un:
				result = OpCodes.Ble_Un;
				break;
			case Code.Ble_Un:
				result = OpCodes.Bgt_Un;
				break;
			case Code.Blt_Un:
				result = OpCodes.Bge_Un;
				break;
			default:
				throw new NotSupportedException();
			}
			return result;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00011098 File Offset: 0x0000F298
		public override void Mangle(CilBody body, ScopeBlock root, CFContext ctx)
		{
			SwitchMangler.<>c__DisplayClass3_0 CS$<>8__locals1 = new SwitchMangler.<>c__DisplayClass3_0();
			CS$<>8__locals1.trace = new SwitchMangler.Trace(body, ctx.Method.ReturnType.RemoveModifiers().ElementType != ElementType.Void);
			Local local = new Local(ctx.Method.Module.CorLibTypes.UInt32);
			body.Variables.Add(local);
			body.InitLocals = true;
			body.MaxStack += 2;
			IPredicate predicate = null;
			bool flag = ctx.Predicate == PredicateType.Normal;
			if (flag)
			{
				predicate = new NormalPredicate(ctx);
			}
			else
			{
				bool flag2 = ctx.Predicate == PredicateType.Expression;
				if (flag2)
				{
					predicate = new ExpressionPredicate(ctx);
				}
				else
				{
					bool flag3 = ctx.Predicate == PredicateType.x86;
					if (flag3)
					{
						predicate = new x86Predicate(ctx);
					}
				}
			}
			using (IEnumerator<InstrBlock> enumerator = ManglerBase.GetAllBlocks(root).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SwitchMangler.<>c__DisplayClass3_1 CS$<>8__locals2 = new SwitchMangler.<>c__DisplayClass3_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.block = enumerator.Current;
					LinkedList<Instruction[]> statements = this.SplitStatements(CS$<>8__locals2.block, CS$<>8__locals2.CS$<>8__locals1.trace, ctx);
					bool isInstanceConstructor = ctx.Method.IsInstanceConstructor;
					if (isInstanceConstructor)
					{
						List<Instruction> list = new List<Instruction>();
						while (statements.First != null)
						{
							list.AddRange(statements.First.Value);
							Instruction instruction = statements.First.Value.Last<Instruction>();
							statements.RemoveFirst();
							bool flag4 = instruction.OpCode == OpCodes.Call && ((IMethod)instruction.Operand).Name == ".ctor";
							if (flag4)
							{
								break;
							}
						}
						statements.AddFirst(list.ToArray());
					}
					bool flag5 = statements.Count < 3;
					if (!flag5)
					{
						int[] array = Enumerable.Range(0, statements.Count).ToArray<int>();
						ctx.Random.Shuffle<int>(array);
						int[] array2 = new int[array.Length];
						int i;
						for (i = 0; i < array2.Length; i++)
						{
							int num = ctx.Random.NextInt32() & int.MaxValue;
							array2[i] = num - num % statements.Count + array[i];
						}
						Dictionary<Instruction, int> dictionary = new Dictionary<Instruction, int>();
						LinkedListNode<Instruction[]> linkedListNode = statements.First;
						i = 0;
						while (linkedListNode != null)
						{
							bool flag6 = i != 0;
							if (flag6)
							{
								dictionary[linkedListNode.Value[0]] = array2[i];
							}
							i++;
							linkedListNode = linkedListNode.Next;
						}
						HashSet<Instruction> statementLast = new HashSet<Instruction>(from st in statements
						select st.Last<Instruction>());
						Func<Instruction, bool> <>9__4;
						Func<Instruction, bool> <>9__5;
						Func<Instruction, bool> <>9__2;
						Func<IList<Instruction>, bool> func = delegate(IList<Instruction> instrs)
						{
							Func<Instruction, bool> predicate2;
							if ((predicate2 = <>9__2) == null)
							{
								predicate2 = (<>9__2 = delegate(Instruction instr)
								{
									bool flag22 = CS$<>8__locals2.CS$<>8__locals1.trace.HasMultipleSources(instr.Offset);
									bool result;
									if (flag22)
									{
										result = true;
									}
									else
									{
										List<Instruction> list5;
										bool flag23 = CS$<>8__locals2.CS$<>8__locals1.trace.BrRefs.TryGetValue(instr.Offset, out list5);
										if (flag23)
										{
											bool flag24 = list5.Any((Instruction src) => src.Operand is Instruction[]);
											if (flag24)
											{
												return true;
											}
											IEnumerable<Instruction> source = list5;
											Func<Instruction, bool> predicate3;
											if ((predicate3 = <>9__4) == null)
											{
												predicate3 = (<>9__4 = ((Instruction src) => src.Offset <= statements.First.Value.Last<Instruction>().Offset || src.Offset >= CS$<>8__locals2.block.Instructions.Last<Instruction>().Offset));
											}
											bool flag25 = source.Any(predicate3);
											if (flag25)
											{
												return true;
											}
											IEnumerable<Instruction> source2 = list5;
											Func<Instruction, bool> predicate4;
											if ((predicate4 = <>9__5) == null)
											{
												predicate4 = (<>9__5 = ((Instruction src) => statementLast.Contains(src)));
											}
											bool flag26 = source2.Any(predicate4);
											if (flag26)
											{
												return true;
											}
										}
										result = false;
									}
									return result;
								});
							}
							return instrs.Any(predicate2);
						};
						Instruction instruction2 = new Instruction(OpCodes.Switch);
						List<Instruction> list2 = new List<Instruction>();
						bool flag7 = predicate != null;
						if (flag7)
						{
							predicate.Init(body);
							list2.Add(Instruction.CreateLdcI4(predicate.GetSwitchKey(array2[1])));
							predicate.EmitSwitchLoad(list2);
						}
						else
						{
							list2.Add(Instruction.CreateLdcI4(array2[1]));
						}
						list2.Add(Instruction.Create(OpCodes.Dup));
						list2.Add(Instruction.Create(OpCodes.Stloc, local));
						list2.Add(Instruction.Create(OpCodes.Ldc_I4, statements.Count));
						list2.Add(Instruction.Create(OpCodes.Rem_Un));
						list2.Add(instruction2);
						ctx.AddJump(list2, statements.Last.Value[0]);
						ctx.AddJunk(list2);
						Instruction[] array3 = new Instruction[statements.Count];
						linkedListNode = statements.First;
						i = 0;
						while (linkedListNode.Next != null)
						{
							List<Instruction> list3 = new List<Instruction>(linkedListNode.Value);
							bool flag8 = i != 0;
							if (flag8)
							{
								bool flag9 = false;
								bool flag10 = list3.Last<Instruction>().IsBr();
								if (flag10)
								{
									Instruction key = (Instruction)list3.Last<Instruction>().Operand;
									int num2;
									bool flag11 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(list3.Last<Instruction>().Offset) && dictionary.TryGetValue(key, out num2);
									if (flag11)
									{
										int num3 = (predicate != null) ? predicate.GetSwitchKey(num2) : num2;
										bool flag12 = func(list3);
										list3.RemoveAt(list3.Count - 1);
										bool flag13 = flag12;
										if (flag13)
										{
											list3.Add(Instruction.Create(OpCodes.Ldc_I4, num3));
										}
										else
										{
											int num4 = array2[i];
											int num5 = ctx.Random.NextInt32();
											list3.Add(Instruction.Create(OpCodes.Ldloc, local));
											list3.Add(Instruction.CreateLdcI4(num5));
											list3.Add(Instruction.Create(OpCodes.Mul));
											list3.Add(Instruction.Create(OpCodes.Ldc_I4, num4 * num5 ^ num3));
											list3.Add(Instruction.Create(OpCodes.Xor));
										}
										ctx.AddJump(list3, list2[1]);
										ctx.AddJunk(list3);
										array3[array[i]] = list3[0];
										flag9 = true;
									}
								}
								else
								{
									bool flag14 = list3.Last<Instruction>().IsConditionalBranch();
									if (flag14)
									{
										Instruction key2 = (Instruction)list3.Last<Instruction>().Operand;
										int num6;
										bool flag15 = !CS$<>8__locals2.CS$<>8__locals1.trace.IsBranchTarget(list3.Last<Instruction>().Offset) && dictionary.TryGetValue(key2, out num6);
										if (flag15)
										{
											bool flag16 = func(list3);
											int num7 = array2[i + 1];
											OpCode opCode = list3.Last<Instruction>().OpCode;
											list3.RemoveAt(list3.Count - 1);
											bool flag17 = ctx.Random.NextBoolean();
											if (flag17)
											{
												opCode = SwitchMangler.InverseBranch(opCode);
												int num8 = num6;
												num6 = num7;
												num7 = num8;
											}
											int num9 = array2[i];
											int num10 = 0;
											int num11 = 0;
											bool flag18 = !flag16;
											if (flag18)
											{
												num10 = ctx.Random.NextInt32();
												num11 = num9 * num10;
											}
											Instruction instruction3 = Instruction.CreateLdcI4(num11 ^ ((predicate != null) ? predicate.GetSwitchKey(num6) : num6));
											Instruction item = Instruction.CreateLdcI4(num11 ^ ((predicate != null) ? predicate.GetSwitchKey(num7) : num7));
											Instruction instruction4 = Instruction.Create(OpCodes.Pop);
											list3.Add(Instruction.Create(opCode, instruction3));
											list3.Add(item);
											list3.Add(Instruction.Create(OpCodes.Dup));
											list3.Add(Instruction.Create(OpCodes.Br, instruction4));
											list3.Add(instruction3);
											list3.Add(Instruction.Create(OpCodes.Dup));
											list3.Add(instruction4);
											bool flag19 = !flag16;
											if (flag19)
											{
												list3.Add(Instruction.Create(OpCodes.Ldloc, local));
												list3.Add(Instruction.CreateLdcI4(num10));
												list3.Add(Instruction.Create(OpCodes.Mul));
												list3.Add(Instruction.Create(OpCodes.Xor));
											}
											ctx.AddJump(list3, list2[1]);
											ctx.AddJunk(list3);
											array3[array[i]] = list3[0];
											flag9 = true;
										}
									}
								}
								bool flag20 = !flag9;
								if (flag20)
								{
									int num12 = (predicate != null) ? predicate.GetSwitchKey(array2[i + 1]) : array2[i + 1];
									bool flag21 = !func(list3);
									if (flag21)
									{
										int num13 = array2[i];
										int num14 = ctx.Random.NextInt32();
										list3.Add(Instruction.Create(OpCodes.Ldloc, local));
										list3.Add(Instruction.CreateLdcI4(num14));
										list3.Add(Instruction.Create(OpCodes.Mul));
										list3.Add(Instruction.Create(OpCodes.Ldc_I4, num13 * num14 ^ num12));
										list3.Add(Instruction.Create(OpCodes.Xor));
									}
									else
									{
										list3.Add(Instruction.Create(OpCodes.Ldc_I4, num12));
									}
									ctx.AddJump(list3, list2[1]);
									ctx.AddJunk(list3);
									array3[array[i]] = list3[0];
								}
							}
							else
							{
								array3[array[i]] = list2[0];
							}
							linkedListNode.Value = list3.ToArray();
							linkedListNode = linkedListNode.Next;
							i++;
						}
						array3[array[i]] = linkedListNode.Value[0];
						instruction2.Operand = array3;
						Instruction[] value = statements.First.Value;
						statements.RemoveFirst();
						Instruction[] value2 = statements.Last.Value;
						statements.RemoveLast();
						List<Instruction[]> list4 = statements.ToList<Instruction[]>();
						ctx.Random.Shuffle<Instruction[]>(list4);
						CS$<>8__locals2.block.Instructions.Clear();
						CS$<>8__locals2.block.Instructions.AddRange(value);
						CS$<>8__locals2.block.Instructions.AddRange(list2);
						foreach (Instruction[] collection in list4)
						{
							CS$<>8__locals2.block.Instructions.AddRange(collection);
						}
						CS$<>8__locals2.block.Instructions.AddRange(value2);
					}
				}
			}
		}

		// Token: 0x02000099 RID: 153
		private struct Trace
		{
			// Token: 0x060002B0 RID: 688 RVA: 0x00011B04 File Offset: 0x0000FD04
			private static void Increment(Dictionary<uint, int> counts, uint key)
			{
				int num;
				bool flag = !counts.TryGetValue(key, out num);
				if (flag)
				{
					num = 0;
				}
				counts[key] = num + 1;
			}

			// Token: 0x060002B1 RID: 689 RVA: 0x00011B30 File Offset: 0x0000FD30
			public Trace(CilBody body, bool hasReturnValue)
			{
				this.RefCount = new Dictionary<uint, int>();
				this.BrRefs = new Dictionary<uint, List<Instruction>>();
				this.BeforeStack = new Dictionary<uint, int>();
				this.AfterStack = new Dictionary<uint, int>();
				body.UpdateInstructionOffsets();
				foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
				{
					this.BeforeStack[exceptionHandler.TryStart.Offset] = 0;
					this.BeforeStack[exceptionHandler.HandlerStart.Offset] = ((exceptionHandler.HandlerType != ExceptionHandlerType.Finally) ? 1 : 0);
					bool flag = exceptionHandler.FilterStart != null;
					if (flag)
					{
						this.BeforeStack[exceptionHandler.FilterStart.Offset] = 1;
					}
				}
				int value = 0;
				int i = 0;
				while (i < body.Instructions.Count)
				{
					Instruction instruction = body.Instructions[i];
					bool flag2 = this.BeforeStack.ContainsKey(instruction.Offset);
					if (flag2)
					{
						value = this.BeforeStack[instruction.Offset];
					}
					this.BeforeStack[instruction.Offset] = value;
					instruction.UpdateStack(ref value, hasReturnValue);
					this.AfterStack[instruction.Offset] = value;
					switch (instruction.OpCode.FlowControl)
					{
					case FlowControl.Branch:
					{
						uint offset = ((Instruction)instruction.Operand).Offset;
						bool flag3 = !this.BeforeStack.ContainsKey(offset);
						if (flag3)
						{
							this.BeforeStack[offset] = value;
						}
						SwitchMangler.Trace.Increment(this.RefCount, offset);
						this.BrRefs.AddListEntry(offset, instruction);
						value = 0;
						break;
					}
					case FlowControl.Break:
					case FlowControl.Meta:
					case FlowControl.Next:
						goto IL_2F9;
					case FlowControl.Call:
					{
						bool flag4 = instruction.OpCode.Code == Code.Jmp;
						if (flag4)
						{
							value = 0;
						}
						goto IL_2F9;
					}
					case FlowControl.Cond_Branch:
					{
						bool flag5 = instruction.OpCode.Code == Code.Switch;
						if (flag5)
						{
							foreach (Instruction instruction2 in (Instruction[])instruction.Operand)
							{
								bool flag6 = !this.BeforeStack.ContainsKey(instruction2.Offset);
								if (flag6)
								{
									this.BeforeStack[instruction2.Offset] = value;
								}
								SwitchMangler.Trace.Increment(this.RefCount, instruction2.Offset);
								this.BrRefs.AddListEntry(instruction2.Offset, instruction);
							}
						}
						else
						{
							uint offset = ((Instruction)instruction.Operand).Offset;
							bool flag7 = !this.BeforeStack.ContainsKey(offset);
							if (flag7)
							{
								this.BeforeStack[offset] = value;
							}
							SwitchMangler.Trace.Increment(this.RefCount, offset);
							this.BrRefs.AddListEntry(offset, instruction);
						}
						goto IL_2F9;
					}
					case FlowControl.Phi:
						goto IL_2F3;
					case FlowControl.Return:
					case FlowControl.Throw:
						break;
					default:
						goto IL_2F3;
					}
					IL_337:
					i++;
					continue;
					IL_2F9:
					bool flag8 = i + 1 < body.Instructions.Count;
					if (flag8)
					{
						uint offset = body.Instructions[i + 1].Offset;
						SwitchMangler.Trace.Increment(this.RefCount, offset);
					}
					goto IL_337;
					IL_2F3:
					throw new UnreachableException();
				}
			}

			// Token: 0x060002B2 RID: 690 RVA: 0x00011EA4 File Offset: 0x000100A4
			public bool IsBranchTarget(uint offset)
			{
				List<Instruction> list;
				bool flag = this.BrRefs.TryGetValue(offset, out list);
				return flag && list.Count > 0;
			}

			// Token: 0x060002B3 RID: 691 RVA: 0x00011ED8 File Offset: 0x000100D8
			public bool HasMultipleSources(uint offset)
			{
				int num;
				bool flag = this.RefCount.TryGetValue(offset, out num);
				return flag && num > 1;
			}

			// Token: 0x04000112 RID: 274
			public Dictionary<uint, int> RefCount;

			// Token: 0x04000113 RID: 275
			public Dictionary<uint, List<Instruction>> BrRefs;

			// Token: 0x04000114 RID: 276
			public Dictionary<uint, int> BeforeStack;

			// Token: 0x04000115 RID: 277
			public Dictionary<uint, int> AfterStack;
		}
	}
}
