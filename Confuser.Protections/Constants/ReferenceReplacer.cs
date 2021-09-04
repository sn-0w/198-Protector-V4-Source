using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000A2 RID: 162
	internal class ReferenceReplacer
	{
		// Token: 0x060002C7 RID: 711 RVA: 0x00012260 File Offset: 0x00010460
		public static void ReplaceReference(CEContext ctx, ProtectionParameters parameters)
		{
			foreach (KeyValuePair<MethodDef, List<Tuple<Instruction, uint, IMethod>>> keyValuePair in ctx.ReferenceRepl)
			{
				bool parameter = parameters.GetParameter<bool>(ctx.Context, keyValuePair.Key, "cfg", false);
				if (parameter)
				{
					ReferenceReplacer.ReplaceCFG(keyValuePair.Key, keyValuePair.Value, ctx);
				}
				else
				{
					ReferenceReplacer.ReplaceNormal(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x000122FC File Offset: 0x000104FC
		private static string XorShit(string text, string key, string meme)
		{
			StringBuilder stringBuilder = new StringBuilder();
			key += meme;
			for (int i = 0; i < text.Length; i++)
			{
				stringBuilder.Append(text[i] ^ key[i % key.Length]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x00012358 File Offset: 0x00010558
		private static void ReplaceNormal(MethodDef method, List<Tuple<Instruction, uint, IMethod>> instrs)
		{
			string text = ReferenceReplacer.memes[ReferenceReplacer.rand.Next(0, ReferenceReplacer.memes.Length)];
			int value = ReferenceReplacer.rand.Next(1, 9999);
			foreach (Tuple<Instruction, uint, IMethod> tuple in instrs)
			{
				int num = method.Body.Instructions.IndexOf(tuple.Item1);
				tuple.Item1.OpCode = OpCodes.Ldc_I4;
				tuple.Item1.Operand = (int)(tuple.Item2 - (uint)ReferenceReplacer.rand.Next(1, 7000));
				method.Body.Instructions.Insert(num + 1, Instruction.Create(OpCodes.Ldstr, ReferenceReplacer.XorShit(Convert.ToString(tuple.Item2), Convert.ToString(value), text)));
				method.Body.Instructions.Insert(num + 2, Instruction.Create(OpCodes.Ldc_I4, value));
				method.Body.Instructions.Insert(num + 3, Instruction.Create(OpCodes.Ldstr, text));
				method.Body.Instructions.Insert(num + 4, Instruction.Create(OpCodes.Call, tuple.Item3));
				method.Body.Instructions.Insert(num + 4, Instruction.Create(OpCodes.Br_S, method.Body.Instructions[num + 4]));
			}
			ReferenceReplacer.Mutate(method);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00012504 File Offset: 0x00010704
		private static void Mutate(MethodDef method)
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			int num = 0;
			ITypeDefOrRef type = null;
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				bool flag = instruction.IsLdcI4();
				if (flag)
				{
					switch (random.Next(1, 16))
					{
					case 1:
						type = method.Module.Import(typeof(int));
						num = 4;
						break;
					case 2:
						type = method.Module.Import(typeof(sbyte));
						num = 1;
						break;
					case 3:
						type = method.Module.Import(typeof(byte));
						num = 1;
						break;
					case 4:
						type = method.Module.Import(typeof(bool));
						num = 1;
						break;
					case 5:
						type = method.Module.Import(typeof(decimal));
						num = 16;
						break;
					case 6:
						type = method.Module.Import(typeof(short));
						num = 2;
						break;
					case 7:
						type = method.Module.Import(typeof(long));
						num = 8;
						break;
					case 8:
						type = method.Module.Import(typeof(uint));
						num = 4;
						break;
					case 9:
						type = method.Module.Import(typeof(float));
						num = 4;
						break;
					case 10:
						type = method.Module.Import(typeof(char));
						num = 2;
						break;
					case 11:
						type = method.Module.Import(typeof(ushort));
						num = 2;
						break;
					case 12:
						type = method.Module.Import(typeof(double));
						num = 8;
						break;
					case 13:
						type = method.Module.Import(typeof(DateTime));
						num = 8;
						break;
					case 14:
						type = method.Module.Import(typeof(ConsoleKeyInfo));
						num = 12;
						break;
					case 15:
						type = method.Module.Import(typeof(Guid));
						num = 16;
						break;
					}
					int num2 = random.Next(1, 1000);
					bool flag2 = Convert.ToBoolean(random.Next(0, 2));
					switch ((num != 0) ? ((Convert.ToInt32(instruction.Operand) % num == 0) ? random.Next(1, 5) : random.Next(1, 4)) : random.Next(1, 4))
					{
					case 1:
						method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
						method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
						instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag2 ? (-num2) : num2);
						method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
						method.Body.Instructions.Insert(i + 4, Instruction.Create(flag2 ? OpCodes.Add : OpCodes.Sub));
						i += 4;
						break;
					case 2:
						method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
						method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Sub));
						instruction.Operand = Convert.ToInt32(instruction.Operand) + num + (flag2 ? (-num2) : num2);
						method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
						method.Body.Instructions.Insert(i + 4, Instruction.Create(flag2 ? OpCodes.Add : OpCodes.Sub));
						i += 4;
						break;
					case 3:
						method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
						method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Add));
						instruction.Operand = Convert.ToInt32(instruction.Operand) - num + (flag2 ? (-num2) : num2);
						method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
						method.Body.Instructions.Insert(i + 4, Instruction.Create(flag2 ? OpCodes.Add : OpCodes.Sub));
						i += 4;
						break;
					case 4:
						method.Body.Instructions.Insert(i + 1, Instruction.Create(OpCodes.Sizeof, type));
						method.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Mul));
						instruction.Operand = Convert.ToInt32(instruction.Operand) / num;
						i += 2;
						break;
					default:
						method.Body.Instructions.Insert(i + 3, Instruction.CreateLdcI4(num2));
						method.Body.Instructions.Insert(i + 4, Instruction.Create(flag2 ? OpCodes.Add : OpCodes.Sub));
						i += 4;
						break;
					}
				}
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x00012AC4 File Offset: 0x00010CC4
		private static void InjectStateType(CEContext ctx)
		{
			bool flag = ctx.CfgCtxType == null;
			if (flag)
			{
				TypeDef runtimeType = ctx.Context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.CFGCtx");
				ctx.CfgCtxType = InjectHelper.Inject(runtimeType, ctx.Module);
				ctx.Module.Types.Add(ctx.CfgCtxType);
				ctx.CfgCtxCtor = ctx.CfgCtxType.FindMethod(".ctor");
				ctx.CfgCtxNext = ctx.CfgCtxType.FindMethod("Next");
				ctx.Name.MarkHelper(ctx.CfgCtxType, ctx.Marker, ctx.Protection);
				foreach (FieldDef def in ctx.CfgCtxType.Fields)
				{
					ctx.Name.MarkHelper(def, ctx.Marker, ctx.Protection);
				}
				foreach (MethodDef def2 in ctx.CfgCtxType.Methods)
				{
					ctx.Name.MarkHelper(def2, ctx.Marker, ctx.Protection);
				}
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00012C34 File Offset: 0x00010E34
		private static void InsertEmptyStateUpdate(ReferenceReplacer.CFGContext ctx, ControlFlowBlock block)
		{
			CilBody body = ctx.Graph.Body;
			BlockKey blockKey = ctx.Keys[block.Id];
			bool flag = blockKey.EntryState == blockKey.ExitState;
			if (!flag)
			{
				int num = body.Instructions.IndexOf(block.Header);
				ReferenceReplacer.CFGState cfgstate;
				bool flag2 = !ctx.StatesMap.TryGetValue(blockKey.EntryState, out cfgstate);
				if (flag2)
				{
					blockKey.Type = BlockKeyType.Explicit;
				}
				bool flag3 = blockKey.Type == BlockKeyType.Incremental;
				Instruction newInstr;
				if (flag3)
				{
					ReferenceReplacer.CFGState value;
					bool flag4 = !ctx.StatesMap.TryGetValue(blockKey.ExitState, out value);
					if (flag4)
					{
						value = cfgstate;
						int num2 = ctx.Random.NextInt32(3);
						uint num3 = ctx.Random.NextUInt32();
						value.UpdateExplicit(num2, num3);
						int getId = ctx.Random.NextInt32(3);
						byte b = ReferenceReplacer.CFGState.EncodeFlag(false, num2, getId);
						uint incrementalUpdate = cfgstate.GetIncrementalUpdate(num2, num3);
						body.Instructions.Insert(num++, newInstr = Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4, (int)incrementalUpdate));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Pop));
						ctx.StatesMap[blockKey.ExitState] = value;
					}
					else
					{
						int index = num;
						for (int i = 0; i < 4; i++)
						{
							bool flag5 = cfgstate.Get(i) == value.Get(i);
							if (!flag5)
							{
								uint target = value.Get(i);
								int getId2 = ctx.Random.NextInt32(3);
								byte b2 = ReferenceReplacer.CFGState.EncodeFlag(false, i, getId2);
								uint incrementalUpdate2 = cfgstate.GetIncrementalUpdate(i, target);
								body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
								body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b2));
								body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4, (int)incrementalUpdate2));
								body.Instructions.Insert(num++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
								body.Instructions.Insert(num++, Instruction.Create(OpCodes.Pop));
							}
						}
						newInstr = body.Instructions[index];
					}
				}
				else
				{
					ReferenceReplacer.CFGState value2;
					bool flag6 = !ctx.StatesMap.TryGetValue(blockKey.ExitState, out value2);
					if (flag6)
					{
						uint num4 = ctx.Random.NextUInt32();
						value2 = new ReferenceReplacer.CFGState(num4);
						body.Instructions.Insert(num++, newInstr = Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4, (int)num4));
						body.Instructions.Insert(num++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxCtor));
						ctx.StatesMap[blockKey.ExitState] = value2;
					}
					else
					{
						int index2 = num;
						for (int j = 0; j < 4; j++)
						{
							uint value3 = value2.Get(j);
							int getId3 = ctx.Random.NextInt32(3);
							byte b3 = ReferenceReplacer.CFGState.EncodeFlag(true, j, getId3);
							body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
							body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b3));
							body.Instructions.Insert(num++, Instruction.Create(OpCodes.Ldc_I4, (int)value3));
							body.Instructions.Insert(num++, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
							body.Instructions.Insert(num++, Instruction.Create(OpCodes.Pop));
						}
						newInstr = body.Instructions[index2];
					}
				}
				ctx.Graph.Body.ReplaceReference(block.Header, newInstr);
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x000130DC File Offset: 0x000112DC
		private static uint InsertStateGetAndUpdate(ReferenceReplacer.CFGContext ctx, ref int index, BlockKeyType type, ref ReferenceReplacer.CFGState currentState, ReferenceReplacer.CFGState? targetState)
		{
			CilBody body = ctx.Graph.Body;
			bool flag = type == BlockKeyType.Incremental;
			uint result;
			if (flag)
			{
				bool flag2 = targetState == null;
				if (flag2)
				{
					int num = ctx.Random.NextInt32(3);
					uint num2 = ctx.Random.NextUInt32();
					int num3 = ctx.Random.NextInt32(3);
					byte b = ReferenceReplacer.CFGState.EncodeFlag(false, num, num3);
					uint incrementalUpdate = currentState.GetIncrementalUpdate(num, num2);
					currentState.UpdateExplicit(num, num2);
					IList<Instruction> instructions = body.Instructions;
					int num4 = index;
					index = num4 + 1;
					instructions.Insert(num4, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
					IList<Instruction> instructions2 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions2.Insert(num4, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b));
					IList<Instruction> instructions3 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions3.Insert(num4, Instruction.Create(OpCodes.Ldc_I4, (int)incrementalUpdate));
					IList<Instruction> instructions4 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions4.Insert(num4, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
					result = currentState.Get(num3);
				}
				else
				{
					int[] array = new int[]
					{
						0,
						1,
						2,
						3
					};
					ctx.Random.Shuffle<int>(array);
					int num5 = 0;
					uint num6 = 0U;
					foreach (int num7 in array)
					{
						bool flag3 = currentState.Get(num7) == targetState.Value.Get(num7) && num5 != array.Length - 1;
						if (flag3)
						{
							num5++;
						}
						else
						{
							uint num8 = targetState.Value.Get(num7);
							int num9 = ctx.Random.NextInt32(3);
							byte b2 = ReferenceReplacer.CFGState.EncodeFlag(false, num7, num9);
							uint incrementalUpdate2 = currentState.GetIncrementalUpdate(num7, num8);
							currentState.UpdateExplicit(num7, num8);
							IList<Instruction> instructions5 = body.Instructions;
							int num4 = index;
							index = num4 + 1;
							instructions5.Insert(num4, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
							IList<Instruction> instructions6 = body.Instructions;
							num4 = index;
							index = num4 + 1;
							instructions6.Insert(num4, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b2));
							IList<Instruction> instructions7 = body.Instructions;
							num4 = index;
							index = num4 + 1;
							instructions7.Insert(num4, Instruction.Create(OpCodes.Ldc_I4, (int)incrementalUpdate2));
							IList<Instruction> instructions8 = body.Instructions;
							num4 = index;
							index = num4 + 1;
							instructions8.Insert(num4, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
							num5++;
							bool flag4 = num5 == array.Length;
							if (flag4)
							{
								num6 = currentState.Get(num9);
							}
							else
							{
								IList<Instruction> instructions9 = body.Instructions;
								num4 = index;
								index = num4 + 1;
								instructions9.Insert(num4, Instruction.Create(OpCodes.Pop));
							}
						}
					}
					result = num6;
				}
			}
			else
			{
				bool flag5 = targetState == null;
				if (flag5)
				{
					uint num10 = ctx.Random.NextUInt32();
					currentState = new ReferenceReplacer.CFGState(num10);
					IList<Instruction> instructions10 = body.Instructions;
					int num4 = index;
					index = num4 + 1;
					instructions10.Insert(num4, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
					IList<Instruction> instructions11 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions11.Insert(num4, Instruction.Create(OpCodes.Dup));
					IList<Instruction> instructions12 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions12.Insert(num4, Instruction.Create(OpCodes.Ldc_I4, (int)num10));
					IList<Instruction> instructions13 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions13.Insert(num4, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxCtor));
					int num11 = ctx.Random.NextInt32(3);
					uint num12 = ctx.Random.NextUInt32();
					int num13 = ctx.Random.NextInt32(3);
					byte b3 = ReferenceReplacer.CFGState.EncodeFlag(false, num11, num13);
					uint incrementalUpdate3 = currentState.GetIncrementalUpdate(num11, num12);
					currentState.UpdateExplicit(num11, num12);
					IList<Instruction> instructions14 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions14.Insert(num4, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b3));
					IList<Instruction> instructions15 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions15.Insert(num4, Instruction.Create(OpCodes.Ldc_I4, (int)incrementalUpdate3));
					IList<Instruction> instructions16 = body.Instructions;
					num4 = index;
					index = num4 + 1;
					instructions16.Insert(num4, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
					result = currentState.Get(num13);
				}
				else
				{
					int[] array3 = new int[]
					{
						0,
						1,
						2,
						3
					};
					ctx.Random.Shuffle<int>(array3);
					int num14 = 0;
					uint num15 = 0U;
					foreach (int num16 in array3)
					{
						uint value = targetState.Value.Get(num16);
						int num17 = ctx.Random.NextInt32(3);
						byte b4 = ReferenceReplacer.CFGState.EncodeFlag(true, num16, num17);
						currentState.UpdateExplicit(num16, value);
						IList<Instruction> instructions17 = body.Instructions;
						int num4 = index;
						index = num4 + 1;
						instructions17.Insert(num4, Instruction.Create(OpCodes.Ldloca, ctx.StateVariable));
						IList<Instruction> instructions18 = body.Instructions;
						num4 = index;
						index = num4 + 1;
						instructions18.Insert(num4, Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)b4));
						IList<Instruction> instructions19 = body.Instructions;
						num4 = index;
						index = num4 + 1;
						instructions19.Insert(num4, Instruction.Create(OpCodes.Ldc_I4, (int)value));
						IList<Instruction> instructions20 = body.Instructions;
						num4 = index;
						index = num4 + 1;
						instructions20.Insert(num4, Instruction.Create(OpCodes.Call, ctx.Ctx.CfgCtxNext));
						num14++;
						bool flag6 = num14 == array3.Length;
						if (flag6)
						{
							num15 = targetState.Value.Get(num17);
						}
						else
						{
							IList<Instruction> instructions21 = body.Instructions;
							num4 = index;
							index = num4 + 1;
							instructions21.Insert(num4, Instruction.Create(OpCodes.Pop));
						}
					}
					result = num15;
				}
			}
			return result;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x000136D4 File Offset: 0x000118D4
		private static void ReplaceCFG(MethodDef method, List<Tuple<Instruction, uint, IMethod>> instrs, CEContext ctx)
		{
			ReferenceReplacer.InjectStateType(ctx);
			ControlFlowGraph controlFlowGraph = ControlFlowGraph.Construct(method.Body);
			BlockKey[] array = KeySequence.ComputeKeys(controlFlowGraph, null);
			ReferenceReplacer.CFGContext cfgcontext = new ReferenceReplacer.CFGContext
			{
				Ctx = ctx,
				Graph = controlFlowGraph,
				Keys = array,
				StatesMap = new Dictionary<uint, ReferenceReplacer.CFGState>(),
				Random = ctx.Random
			};
			cfgcontext.StateVariable = new Local(ctx.CfgCtxType.ToTypeSig(true));
			method.Body.Variables.Add(cfgcontext.StateVariable);
			method.Body.InitLocals = true;
			Dictionary<int, SortedList<int, Tuple<Instruction, uint, IMethod>>> dictionary = new Dictionary<int, SortedList<int, Tuple<Instruction, uint, IMethod>>>();
			foreach (Tuple<Instruction, uint, IMethod> tuple in instrs)
			{
				int num = controlFlowGraph.IndexOf(tuple.Item1);
				ControlFlowBlock containingBlock = controlFlowGraph.GetContainingBlock(num);
				SortedList<int, Tuple<Instruction, uint, IMethod>> sortedList;
				bool flag = !dictionary.TryGetValue(containingBlock.Id, out sortedList);
				if (flag)
				{
					sortedList = (dictionary[containingBlock.Id] = new SortedList<int, Tuple<Instruction, uint, IMethod>>());
				}
				sortedList.Add(num, tuple);
			}
			for (int i = 0; i < controlFlowGraph.Count; i++)
			{
				ControlFlowBlock controlFlowBlock = controlFlowGraph[i];
				bool flag2 = dictionary.ContainsKey(controlFlowBlock.Id);
				if (!flag2)
				{
					ReferenceReplacer.InsertEmptyStateUpdate(cfgcontext, controlFlowBlock);
				}
			}
			foreach (KeyValuePair<int, SortedList<int, Tuple<Instruction, uint, IMethod>>> keyValuePair in dictionary)
			{
				BlockKey blockKey = array[keyValuePair.Key];
				ReferenceReplacer.CFGState value;
				bool flag3 = !cfgcontext.StatesMap.TryGetValue(blockKey.EntryState, out value);
				if (flag3)
				{
					Debug.Assert((controlFlowGraph[keyValuePair.Key].Type & ControlFlowBlockType.Entry) > ControlFlowBlockType.Normal);
					Debug.Assert(blockKey.Type == BlockKeyType.Explicit);
					uint num2 = ctx.Random.NextUInt32();
					value = new ReferenceReplacer.CFGState(num2);
					cfgcontext.StatesMap[blockKey.EntryState] = value;
					int num3 = controlFlowGraph.Body.Instructions.IndexOf(controlFlowGraph[keyValuePair.Key].Header);
					Instruction newInstr;
					method.Body.Instructions.Insert(num3++, newInstr = Instruction.Create(OpCodes.Ldloca, cfgcontext.StateVariable));
					method.Body.Instructions.Insert(num3++, Instruction.Create(OpCodes.Ldc_I4, (int)num2));
					method.Body.Instructions.Insert(num3++, Instruction.Create(OpCodes.Call, ctx.CfgCtxCtor));
					method.Body.ReplaceReference(controlFlowGraph[keyValuePair.Key].Header, newInstr);
					blockKey.Type = BlockKeyType.Incremental;
				}
				BlockKeyType type = blockKey.Type;
				for (int j = 0; j < keyValuePair.Value.Count; j++)
				{
					Tuple<Instruction, uint, IMethod> tuple2 = keyValuePair.Value.Values[j];
					ReferenceReplacer.CFGState? targetState = null;
					bool flag4 = j == keyValuePair.Value.Count - 1;
					if (flag4)
					{
						ReferenceReplacer.CFGState value2;
						bool flag5 = cfgcontext.StatesMap.TryGetValue(blockKey.ExitState, out value2);
						if (flag5)
						{
							targetState = new ReferenceReplacer.CFGState?(value2);
						}
					}
					int index = controlFlowGraph.Body.Instructions.IndexOf(tuple2.Item1) + 1;
					uint num4 = ReferenceReplacer.InsertStateGetAndUpdate(cfgcontext, ref index, type, ref value, targetState);
					tuple2.Item1.OpCode = OpCodes.Ldc_I4;
					tuple2.Item1.Operand = (int)(tuple2.Item2 ^ num4);
					method.Body.Instructions.Insert(index++, Instruction.Create(OpCodes.Xor));
					method.Body.Instructions.Insert(index, Instruction.Create(OpCodes.Call, tuple2.Item3));
					bool flag6 = j == keyValuePair.Value.Count - 1 && targetState == null;
					if (flag6)
					{
						cfgcontext.StatesMap[blockKey.ExitState] = value;
					}
					type = BlockKeyType.Incremental;
				}
			}
		}

		// Token: 0x0400013E RID: 318
		private static Random rand = new Random(Guid.NewGuid().GetHashCode());

		// Token: 0x0400013F RID: 319
		private static string[] memes = new string[]
		{
			"ṇ̷̐̽́̍͊̑̾̿͝ỏ̴̘̺̝͚̫̗͈͎̝͓̓̅̐͛̑̕͠w̶̤̠̣͍͂͛͆͂̂̇͘ ̸̛͓̌̽̊̐͆̈͒͋͘r̶̤̈͗̾̆͆͂̏͝o̶̗̙͍͑̿̈́o̷̧̼̠̩̹̞̜̻̭̓f̶̧̮͔̬̞̱̼̎͗̏͂́̏̍̀͠ ̵̰̻̘̲̲̗̼̱̹͌͒̒͆̀͑̚͜ẃ̶̮̲̫̙̑̐͆̆ͅh̶̨͍̠͔̯̹̦̞̮͒̏͛̚͠͠o̶̳̝̟̘̘̎̂͆͘͝m̴̥̤̽͌͛̈́̇̾͠͝ ̷̡̺̿s̵̘̘̰̳̭̤̍̈́ụ̵̄̈́̈̚͜c̵̳̦͖̍͜͜͜h̵̩̳̻̯̖̩̣͒͑̉͗̎ ̶̧͖͑̈ǹ̴̨̧̼͓͊̿͗̆͜ͅͅͅė̶̠̲̪̣͂͒͋͋͘͘x̵̛̛̭̼̝̙̰̙͕͌̋̌̊̈́̈́͜͠t̸͖̞̟͚̪̱͇̯̪̏ ̷̯̎͊̔̈̍͊̾̕͝w̵͚̲͕̠͓̗̜̫̄͛̌̓̃͠a̷̝͈͍͂̓̚s̷̢̛̳͎̻͔̺͚͑̆̽̍ͅ ̸̡̱͉͔͈̱̙̝̖͍̋͌̓̈́̄̀͝Ĥ̴̡͔̼̤̞̥͍̀á̷̹͈̙͚́́̏̀̀̾̎͘͜m̵͕̯̪̱̪̠͈͉̯̿͝͝ ̴̦̦̥̱͙̠̥̟̆̄̆͊̅̇͆̂̔ṕ̴̭̻͕̅͊r̸̢̦͍̗̦̪̦̄̒̉̀ͅͅe̵̥͇̫̞͍͚̟̙͉͋̾̎̍͜ţ̵̫͓͓̀̋̅̏̇͊͗̉̚͝ţ̶͎̟͚̿͌́͝y̸̬͗ ̷̟̦̞̺̀͂̌̊̿̊̊͝ȏ̷̢̪̯͌̿̂̐͜ů̶͎̝̻̘̻̞̩͙̰̄̏̚r̷̡̝͓̩͓̩̒̆͊̓̓͌̄̾̏͘ ̸͕͉̳͋́͌̿̎͐͌̽p̶̤͔͗̽̎e̴̤̖͈͚͎̺͉̬̅̏͑̾́̄̅̚ō̷̧̝̬̼̦̿̎͒͌͂̓̕p̴̭̉͋̉͒̀̑̌̐̕l̴̢̛̲̠͕̘̜̹̾̃͑͛͐̕͘e̴̛̠̯̋́̽̀̄͒̂̀ ̶͍̜̥̰̗̮̩̊̎͋̎̈͑͘͘ͅm̷̧̥̣͖̺̳̆̌̀̓͒ö̴̰̻̲́̆̂͌͗́́͠m̶̡̩̤͔̳̐̐̀̾́̊è̶͉̱̣͈͇̹̯̲͗̌̍͜ͅṉ̴̬̺̳̼̲͈́̈́̅̔́̈͝͝ṭ̴̠̜͕̪̖͉͎͐́͠ ̷̼̦̯͎̟̯̮̈́̅͜͜p̶̩̝̈́ͅư̵̹̺̱̇̈́̕ẗ̴͈̯̫̩͕̙̃́͊̈́͛̈́͜͝ ̸͓̪̤̤̦̟̘̉̇͝ͅȇ̸̛͕͎̲͎̔̾̎̑̋̋x̸̝͉̂͋̂̂̀͐̋͊͠c̵̡̫̄͛̆͊̾u̵͇̥̖͙̭̔̈́̅̒̕͘̚s̸̨̧̮̙͚̗̱̈́̈́̂̇̚͝͝ę̷͓͕̣̲̦͈̞̤͍̈́ ̸͎̌̏́́̉͋̚͠͝n̵̝̩͊͂͛͆a̶̛͙̞̦̮̳͊̔̌r̸̛͚̋̀͊͂̓̕r̷̠͇̦̥͍͙̒̈o̵̡̠̯̗͍̻̾̀̐̂̎ẃ̵̧̥̹̮͋̅̽̅̈̾̋̍̚ ̷̳̀͊̐̄̐̓̀̀̈́͝Ş̷̮̤͓̣͚̒̍̌̎͑̓̈́̉͠p̴̞̙̜̯̙͔̦̦͈̩͗͆͆͘i̶̫̒́̄̀͘͝t̵̛͍̗̭͌̏̓̔̕ȩ̶̧̲͇͚̬̰̲̖͎̽̄̍̆̐͋͐͗̋̿ ̷̡̢̻̮͎̠͕͙̻̿̇͂͝m̵̰̟͖̻̖̤̩̩̱͉̽̎̑̿́i̵̧̍̽̈́̐ŕ̴͕̹̲̬̯̹̝ͅț̵̻̲̗͈̊͋͌̉͋̕͜h̷͙̳͇̩͕̹͙̿͆ͅ ̸̜̐̌̀m̵̢̼̙͕͎̐͒͛͐̔̐̀̇͋͝o̶͓̮͍̒n̴̢͙͚̹͚̝͐̑̑̚ë̶̙́͆̐͘͝y̵̨̢͉̲̦̹͂̈́̇̉̎͌̕ ̷̢̡̞̺͔̲̱̈̕͜͜s̵͈̘̣̖͐̉̊̃̉͌͝i̸̘͍̰͔͕͚͛̈́̒͂͜͝͝x̴̝̹͔̭͋̃ ̷͐̔̔̊̔͂̑ͅã̶̤̙͈̎̇́̈́͐͊͘͝b̶̩̤͕̘͋̇o̷͈̖̣̳͑͌͐ͅv̸̢̲͉͚̲̺͕͗͌̂̈́͛͘̕é̴̞̮̕ ̴̛̮̦́͆͆̇̈́̀͐̀ḡ̶̤̇̈̓̔̇̈́͘e̴͈̗̥͊̒̀̽̽̔͂́͒̍t̴͉͕̰̼͕̜̾͘ ̴̢͙̮̗̞͓̮̗̐g̴͔̝̭̦̃̒́ȯ̵̧͚̪ḯ̷̡̨̭̩͙̹̟̖̬̉͐̈́̍̅̅̆́̓n̵̛̜̠͚̘̼̞͈͚̖̟͊̔́̚g̶̳̱̠̬͌̐ ̸̮͈̭̰̦͎͍̫̰̲̅̈́̈̄͂̎̋́̕͘ģ̴̆̓̓̊̆͠͠ṙ̶̨̘̰̞̎̄͒́͝e̴̱̝̥̙͔̿͂͝a̴̫̠̙̎̀̓́̈̈́t̶̛̠̫̤̔͛̏̈́͊̑̽̓͘ͅ ̶̙̮̫̫̙͍͉̙̹̮̔̍̍̃̃̂̔̕ơ̸̰̰͉̦̫̟̾̊̎̉̂̑ẉ̵̨̧̩̙̹͙͈͗͐͑̏͆̿̄͗͘ñ̵̪̤̜̰̒͋̔̇ ̵̲͚̰̪̼̆̂̃̐͆̅̓̚͘S̸͔̏̉̆͋͜t̸̨̨͓̯͉̥̮̝͙̅̒͑̋̈́͑͌̏̈́ͅȃ̵̰̘͚̞̀̇̏̂͋̕͠ṟ̷̺̫̲̜̀͛̓̀̾͘͜t̶͈̠̬̼̋̿̂͊̂̕ë̷̛͇̓̈́̉͛̓͌̿͊d̴͉̺̰͆̀̅͗̓̐̿͆̕ ̸̧̨̛͙̙̘̟̩̦̅̽͜ñ̷̡̩̤̲̦̭̦̝͓͐̓̐́̃ͅo̵̖̕w̶̡̢̰̮͖̓̃̐͐̆͆̀̌̾ ̶̢̭͙͎̰̦̫̬̾͒͘ṣ̵̛̬͙͐́̀̔͆h̷͎̍̀̈͐o̸̼͕͕̫͙̍̂̆̌͛̄́ȑ̴̢̼͓̯̮͉̝̉͂̎͂͆̃̔̍̚͜t̷̬̟͛̂͗͠l̶̛͖̬̋͛̀͐̐̂ý̶̪̘̰͔̥̺͆̎̆͑͊̐͝ ̶̬̥͇̇͆̏̔̆̂̔̕h̵̻͉̃́̈à̷̘͖̒̅̌̒̽̊͌̍̕d̶̘̙͔̳͎̓͑̊̊̓̚ ̸̧͚͍̗̖͔̭́͆f̸̨̻̫̣̠͇̮̮̏̊̽̕ǫ̸̺͔̙̙͍͔͍̋̈̓͘͘r̷̨̨̧̛̠͍͇̖̜͚͋̓̓̏̈́̒ ̷̮̫̠͙̪̭̋̀̈́͂͘͘͜ͅa̸̟̰̤̯͍͍̍̀̈́̄͒͗͂͜͝͠š̶̨̱̭̥̰͉̠̳͖͛̐̈̓͠ͅs̸̤̫̤͕̿ų̸̱̖̞̏̽̓͊̂̏̈́͂r̴̼̖̘̩̼̼̣̫͕͝e̴̳̋̓d̶̟̳̟̮͇͛́̿ͅ ̸̡͍̺̜̖̦̞̜̉̈́h̵̢̨̥̻͉͇̘̜̦͒́̈́͝é̴̮̳̬̥̱͐̓̄̍̈́͒͛͠͠å̶̬͘r̶̮̩̹̭̺̘͆́͜i̶̮͒͊͛̃͗͠͝ņ̸̡̀̅̑g̶̦̫̚ ̷̨̣͎̱̰͐̏͆ͅȩ̴̡͓̘̥̯̺̯͔̻̈́̀x̷̤̆̌͒̋͘ṕ̸̳͇̲͉̟̥͔̬̾̈́̐͐͋͝é̵̛̜̇̌͊͝n̵̨̧͈̮̹̗̱̈̄͛s̸̛͍͙͉̱̤͑̃̄̓̑͋̓͋͝e̷̠̥̋͋̔͝͝ ̶͓̓̄ͅL̷̨̡̞̯͖̩̂̿e̶̖̝͎̔͆̆̉̔̓̓͝d̶͓͊̓̀͒͘ ̵̭͒͊̓̍̕͜j̸̠̻̭̀u̷͇̩͉̣̺̐̒̎̃̅̔̂͒͝͝v̷̤̐̄͂̿̇̇͐ë̶̡͍̲̀̍̒̆͑̉͊̇͝n̶͇͍̬̦͔͍̹̈́͌̔̇̾̌͊̍i̸̲͊͛͠l̷̢̛̲͍̩̪̙͇͕̟̓͆̀̾͑̓͠e̸͓̩̐̔̓̓̎̋̔̐͠ͅ ̸̢͓͓̜͈͚͍͓̙̐̂͊̀̀̉h̷͚̱̉̇ȉ̶̢̙̹̖̥̮̰̐͆ͅͅs̷̢̮̘̟̗̦̤̪͖̦̿̅̏̈́͝ ̶̮̤̫̙͊̽̉͊͆̈́͐ḷ̸͙̥͍̟̈́̑͌̉̐͘ͅá̷̫͓̙̭̽̋̔̓̌̏̍u̴͓̟͗̎͋̆̍̔̉̂̈̔g̷̦̬̉͐h̵̛̠͗͆̏͝i̴͓͕͕̯̖̙̟͖̮͋̀͆̓͌̉̏̀͠͝ͅn̵̺̱̗̦̮̞̰̳̭̭͒̇̒g̵̹̙̠̏̋̐͛̃͆͑̿̍ ̵͙̞́͛̎̄͑̕͠s̴̨̭̤̝̗͛̆̈́̏̎͛̚͝p̵̜͚̙͍̠̬̜̞͋͐͜͝è̶̥͚̹̞͉̜̪̀͌̎̓͋̍͆̚͝é̴̺̤d̴͚̟̝͒̊̒̎i̸̯͒̇͗̀͊̑̈́̆͑͝l̷̦̺͇͚̦̲͉̘͉͍͌̈̊̑́̔̾ȳ̵̧̤͓̘̦̼͚͈̞̄͑ͅ ̷͎̤̝̗͋̒̍̂̌͆̕͝p̴̣̬͎̦͍̤͚̖̖̮̽̅́̾̌̆̚͘͝u̸̝͐̒͆͝t̵͎͐̑͒̉̌̇̚ ̸̠̭̮͓̗͕͈̂̉̏͝p̸̗̬̝̣͕͇͆̐̊ĺ̵̢͍͙̖͔̳͇͙̙̊͌̀̇̈́̚͝ȇ̷͉̭̦͉͕̖̓͋̓̓͂̏̉̇à̴̢̨̦͎́͂̐̊̂̂͠͝s̵̘͚̟̦̪̓̑͗ȧ̸̧̬̟̲̤̜̫̮̮̳̋̈͗̉͛̄n̵͎̼͎̟͇̱̯͗͆̉̅̏̚t̶̡̛̮̻͔͐̈́̓̿̒͂͆͝ͅ ̷̱̖̖̥̳͊̉̐̐̾̐̅͛̀́ṟ̸̡̧̲̲̦̥͇̞͙̈͐̍̀̏͋́̾́͠ë̴̡͙̠͇͚̻̜̮́̈́͊̈͒͝͝l̴̹͊̍̏̀̀̚͘̕ͅa̶̢̛̼̬̳̽̏̐̄̒̕t̷̲͇͎͎͚̽̽̏̐̏͗i̴͓̘̩̩͍̾̃̇̄̀̕͝ó̴͚̲̯͇̏͐͗͆̈̿͗́͜n̴̺͈̖͙̙̙͉̮̼̔̀̈́̑̍ͅ ̵̨̩͉͖̹̱̭̒ͅơ̴͈̹̙͉͍͚̘̝̮͓͂̔͠f̶̜͌̒͛̆̈́̕͝͝͝f̶̨̻͔̺͓̜͍̟̯̒͌͒̎͛̾̄̓̈́e̷̠̰̭̙̯̞͛͋ͅr̸̼͍̪͎̪͕͛̽̀̽͆͋̅̒͒̌î̸͔̦̫̲̭͌͛͂͠n̶̛͎͍͔̦͝g̴̡̛̻̦̻̩̲̰͕͊̓̆̂̇̓̋̐͗ͅ ̷̡͔̖̩̫͔͖̦͓̤̄̓̎͒̃͑͌͘Ḟ̶̬̰̘̟̱̩͑̉̂͗̂̀̚͘͝e̴͎̜̯̰͚͒͒̆̃̌̀̓ř̶̛̩̜̮̦̻̟̂́̽͌̋͑͝ŗ̷̟͕̲̮̰̒a̵̹͚͙͓̝̒̽͘͝ř̴̛͖̦̰̝̣̗͑͛͌̚̚͝͝s̴͚̃͂̈̀́̎̃̑̋͝ ̴̼̅̈́̍̑̀͝a̸̡͇̣̫̺̯͇͕͛̓͑̿̕͜l̷̯͉͉̫̠͕͙͈͆ḽ̵̜̥͈͇͋͆́̓̀͝ ̸̡̳̼̞̻̤͖̯̓̋̈́͜͝͝ͅş̴̗̬̻͈̬̐͜p̸̳͖̻͔̫̬̻͓̀̓͝ͅi̶̼͎̩̖̤͕̺̹͂͒͊́͊͂͆̽̂͗r̵̂̅͌̓̃͊̊ͅĭ̸̛̥͍̋̾̈́͊͑͗̕ţ̶̡͎̤͐̃̓́́͘͜͝s̴̗̙̟͎̈͒̍ͅ ̸̢̰̝͍̬̱̆͑̌̃̚͝h̴̤̺͕̪̤͛̾͌̑̒̆̋́ͅi̴̼̠͋̋̏͊͌͝͝s̶̢̛̽̏͗̇̒̓̀͝ͅ ̴̢̻̞̭͚̮̫̭̒͠ị̷̧̳̺̮̜̣̰͖̘͊̓̓ḿ̸̙̙̤̫̑͒͌͝͠͝a̶̰̱̜͇̱̮͌̓̈́̅̈̇́͋̒͘g̶̺̜̗̖̱̰̦̰̿́̚ͅỉ̴̢̝̥̙̜̼̥̺̲̉̋̐̏͗̊̋͘͝ͅn̵̯̺̝̗͍̬̝͕̾͆͗̓̐͌̚̕͝ē̴̛͍̬͈̺͔͍͉͍͆͋͌́̍ ̶͕͗͗̐́̽̾̾̕ͅȇ̴̢̛̱̠̺̦̌̈͘f̶̢̨͉̲̻͍̙̼̬̙̽̌̂̆̕̚f̷̨̢̔͆̏̽̇̄́͛͠͝ë̸̢̢̥̣̰̬̰́̋͘͜͝c̴̝̓͐̏̈̊̀t̵͚̤̟̥̋̒̎͐s̸̼̬̯̣̺̮̝͐̆͜ ̴̝̬̅̈͐̑̚a̴͓͖͍͛̃̂̊̓m̴̡̖̟̙̯̜̦̰̽o̵̢͉̟̘̹̺͉͙̘͈̽͆ń̷̤͕̳̯̱̮͎͍͂̉̉̑̚͜g̷̨̧̭̱̝͔̗̽̓ŝ̶͚̲̲t̴̲̣̗̤̰̠̣̄̓̽̽͒̋͘͝ ̸̨̹͎̹͇̱̲͆n̸͎̭̊̈́̀ȩ̵̨̩̱̜̠̍̋̅̂̚͜i̸̧͉̠͙̳̒t̵͇̺͈̺̙̩̍̒̈̍͝͝ͅh̷̢̛̩͇̞͌̎͗̀̓͘͝ͅe̷̳̤͔̲͐́͌̈́̔̿̚ŗ̴̥̩͍́̿ ̶͔̜͖͍͙͎̬̑̏̃͛̈́̀̑̃͘͜͝Ị̵̲̑̈́t̵̨͎̙̤̬̦̓ ̸̢̰̱̟̟̹̣̔̂̒̉͜ͅb̸̢̡̦̩̂̈́͒͂́̄͘͠à̸͔͓̻̭͙̺̳͐̓̇̾̌́͒͊͊c̶̡̻͎̖͎͎͙̻͈̔̉̽̾̕͝h̶̡̖̟̣̻̩̼̳̿̎͂͒̋͆̿ͅe̶̙̺͓͗̈͗͝͝l̵̻̤͖͎̺͔̝̠͋́̓o̷̦̗̩̘̝̫̻͍͛̚r̵̖̱̮̤͙̟̞̔̓̕ ̷̼̃͊̈́́̀̈́̑̐̿̚"
		};

		// Token: 0x020000A3 RID: 163
		private struct CFGContext
		{
			// Token: 0x04000140 RID: 320
			public CEContext Ctx;

			// Token: 0x04000141 RID: 321
			public ControlFlowGraph Graph;

			// Token: 0x04000142 RID: 322
			public BlockKey[] Keys;

			// Token: 0x04000143 RID: 323
			public RandomGenerator Random;

			// Token: 0x04000144 RID: 324
			public Dictionary<uint, ReferenceReplacer.CFGState> StatesMap;

			// Token: 0x04000145 RID: 325
			public Local StateVariable;
		}

		// Token: 0x020000A4 RID: 164
		private struct CFGState
		{
			// Token: 0x060002D1 RID: 721 RVA: 0x00013BA8 File Offset: 0x00011DA8
			public CFGState(uint seed)
			{
				seed = (this.A = seed * 557916961U);
				seed = (this.B = seed * 557916961U);
				seed = (this.C = seed * 557916961U);
				seed = (this.D = seed * 557916961U);
			}

			// Token: 0x060002D2 RID: 722 RVA: 0x00013BF8 File Offset: 0x00011DF8
			public void UpdateExplicit(int id, uint value)
			{
				switch (id)
				{
				case 0:
					this.A = value;
					break;
				case 1:
					this.B = value;
					break;
				case 2:
					this.C = value;
					break;
				case 3:
					this.D = value;
					break;
				}
			}

			// Token: 0x060002D3 RID: 723 RVA: 0x00013C48 File Offset: 0x00011E48
			public void UpdateIncremental(int id, uint value)
			{
				switch (id)
				{
				case 0:
					this.A *= value;
					break;
				case 1:
					this.B += value;
					break;
				case 2:
					this.C ^= value;
					break;
				case 3:
					this.D -= value;
					break;
				}
			}

			// Token: 0x060002D4 RID: 724 RVA: 0x00013CB4 File Offset: 0x00011EB4
			public uint GetIncrementalUpdate(int id, uint target)
			{
				uint result;
				switch (id)
				{
				case 0:
					result = (this.A ^ target);
					break;
				case 1:
					result = target - this.B;
					break;
				case 2:
					result = (this.C ^ target);
					break;
				case 3:
					result = this.D - target;
					break;
				default:
					throw new UnreachableException();
				}
				return result;
			}

			// Token: 0x060002D5 RID: 725 RVA: 0x00013D14 File Offset: 0x00011F14
			public uint Get(int id)
			{
				uint result;
				switch (id)
				{
				case 0:
					result = this.A;
					break;
				case 1:
					result = this.B;
					break;
				case 2:
					result = this.C;
					break;
				case 3:
					result = this.D;
					break;
				default:
					throw new UnreachableException();
				}
				return result;
			}

			// Token: 0x060002D6 RID: 726 RVA: 0x00013D6C File Offset: 0x00011F6C
			public static byte EncodeFlag(bool exp, int updateId, int getId)
			{
				byte b = exp ? 128 : 0;
				b |= (byte)updateId;
				return b | (byte)(getId << 2);
			}

			// Token: 0x04000146 RID: 326
			public uint A;

			// Token: 0x04000147 RID: 327
			public uint B;

			// Token: 0x04000148 RID: 328
			public uint C;

			// Token: 0x04000149 RID: 329
			public uint D;
		}
	}
}
