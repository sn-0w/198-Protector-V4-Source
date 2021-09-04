using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000A9 RID: 169
	public static class KeySequence
	{
		// Token: 0x060003EC RID: 1004 RVA: 0x00016AC0 File Offset: 0x00014CC0
		public static BlockKey[] ComputeKeys(ControlFlowGraph graph, RandomGenerator random)
		{
			BlockKey[] array = new BlockKey[graph.Count];
			foreach (ControlFlowBlock controlFlowBlock in ((IEnumerable<ControlFlowBlock>)graph))
			{
				BlockKey blockKey = default(BlockKey);
				bool flag = (controlFlowBlock.Type & ControlFlowBlockType.Entry) > ControlFlowBlockType.Normal;
				if (flag)
				{
					blockKey.Type = BlockKeyType.Explicit;
				}
				else
				{
					blockKey.Type = BlockKeyType.Incremental;
				}
				array[controlFlowBlock.Id] = blockKey;
			}
			KeySequence.ProcessBlocks(array, graph, random);
			return array;
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x00016B5C File Offset: 0x00014D5C
		private static void ProcessBlocks(BlockKey[] keys, ControlFlowGraph graph, RandomGenerator random)
		{
			uint num = 0U;
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].EntryState = num++;
				keys[i].ExitState = num++;
			}
			Dictionary<ExceptionHandler, uint> dictionary = new Dictionary<ExceptionHandler, uint>();
			Dictionary<ControlFlowBlock, List<ExceptionHandler>> dictionary2 = new Dictionary<ControlFlowBlock, List<ExceptionHandler>>();
			Func<ControlFlowBlock, uint> <>9__0;
			Func<ControlFlowBlock, uint> <>9__1;
			bool flag;
			do
			{
				flag = false;
				foreach (ControlFlowBlock controlFlowBlock in ((IEnumerable<ControlFlowBlock>)graph))
				{
					BlockKey blockKey = keys[controlFlowBlock.Id];
					bool flag2 = controlFlowBlock.Sources.Count > 0;
					if (flag2)
					{
						IEnumerable<ControlFlowBlock> sources = controlFlowBlock.Sources;
						Func<ControlFlowBlock, uint> selector;
						if ((selector = <>9__0) == null)
						{
							selector = (<>9__0 = ((ControlFlowBlock b) => keys[b.Id].ExitState));
						}
						uint num2 = sources.Select(selector).Max<uint>();
						bool flag3 = blockKey.EntryState != num2;
						if (flag3)
						{
							blockKey.EntryState = num2;
							flag = true;
						}
					}
					bool flag4 = controlFlowBlock.Targets.Count > 0;
					if (flag4)
					{
						IEnumerable<ControlFlowBlock> targets = controlFlowBlock.Targets;
						Func<ControlFlowBlock, uint> selector2;
						if ((selector2 = <>9__1) == null)
						{
							selector2 = (<>9__1 = ((ControlFlowBlock b) => keys[b.Id].EntryState));
						}
						uint num3 = targets.Select(selector2).Max<uint>();
						bool flag5 = blockKey.ExitState != num3;
						if (flag5)
						{
							blockKey.ExitState = num3;
							flag = true;
						}
					}
					bool flag6 = controlFlowBlock.Footer.OpCode.Code == Code.Endfilter || controlFlowBlock.Footer.OpCode.Code == Code.Endfinally;
					if (flag6)
					{
						List<ExceptionHandler> list;
						bool flag7 = !dictionary2.TryGetValue(controlFlowBlock, out list);
						if (flag7)
						{
							list = new List<ExceptionHandler>();
							int num4 = graph.IndexOf(controlFlowBlock.Footer);
							foreach (ExceptionHandler exceptionHandler in graph.Body.ExceptionHandlers)
							{
								bool flag8 = exceptionHandler.FilterStart != null && controlFlowBlock.Footer.OpCode.Code == Code.Endfilter;
								if (flag8)
								{
									bool flag9 = num4 >= graph.IndexOf(exceptionHandler.FilterStart) && num4 < graph.IndexOf(exceptionHandler.HandlerStart);
									if (flag9)
									{
										list.Add(exceptionHandler);
									}
								}
								else
								{
									bool flag10 = exceptionHandler.HandlerType == ExceptionHandlerType.Finally || exceptionHandler.HandlerType == ExceptionHandlerType.Fault;
									if (flag10)
									{
										bool flag11 = num4 >= graph.IndexOf(exceptionHandler.HandlerStart) && (exceptionHandler.HandlerEnd == null || num4 < graph.IndexOf(exceptionHandler.HandlerEnd));
										if (flag11)
										{
											list.Add(exceptionHandler);
										}
									}
								}
							}
							dictionary2[controlFlowBlock] = list;
						}
						foreach (ExceptionHandler key in list)
						{
							uint num5;
							bool flag12 = dictionary.TryGetValue(key, out num5);
							if (flag12)
							{
								bool flag13 = blockKey.ExitState > num5;
								if (flag13)
								{
									dictionary[key] = blockKey.ExitState;
									flag = true;
								}
								else
								{
									bool flag14 = blockKey.ExitState < num5;
									if (flag14)
									{
										blockKey.ExitState = num5;
										flag = true;
									}
								}
							}
							else
							{
								dictionary[key] = blockKey.ExitState;
								flag = true;
							}
						}
					}
					else
					{
						bool flag15 = controlFlowBlock.Footer.OpCode.Code == Code.Leave || controlFlowBlock.Footer.OpCode.Code == Code.Leave_S;
						if (flag15)
						{
							List<ExceptionHandler> list2;
							bool flag16 = !dictionary2.TryGetValue(controlFlowBlock, out list2);
							if (flag16)
							{
								list2 = new List<ExceptionHandler>();
								int num6 = graph.IndexOf(controlFlowBlock.Footer);
								foreach (ExceptionHandler exceptionHandler2 in graph.Body.ExceptionHandlers)
								{
									bool flag17 = num6 >= graph.IndexOf(exceptionHandler2.TryStart) && (exceptionHandler2.TryEnd == null || num6 < graph.IndexOf(exceptionHandler2.TryEnd));
									if (flag17)
									{
										list2.Add(exceptionHandler2);
									}
								}
								dictionary2[controlFlowBlock] = list2;
							}
							uint? num7 = null;
							foreach (ExceptionHandler key2 in list2)
							{
								uint num8;
								bool flag18;
								if (dictionary.TryGetValue(key2, out num8))
								{
									if (num7 != null)
									{
										uint num9 = num8;
										uint? num10 = num7;
										flag18 = (num9 > num10.GetValueOrDefault() & num10 != null);
									}
									else
									{
										flag18 = true;
									}
								}
								else
								{
									flag18 = false;
								}
								bool flag19 = flag18;
								if (flag19)
								{
									bool flag20 = num7 != null;
									if (flag20)
									{
										flag = true;
									}
									num7 = new uint?(num8);
								}
							}
							bool flag21 = num7 != null;
							if (flag21)
							{
								bool flag22 = blockKey.ExitState > num7.Value;
								if (flag22)
								{
									num7 = new uint?(blockKey.ExitState);
									flag = true;
								}
								else
								{
									bool flag23 = blockKey.ExitState < num7.Value;
									if (flag23)
									{
										blockKey.ExitState = num7.Value;
										flag = true;
									}
								}
								foreach (ExceptionHandler key3 in list2)
								{
									dictionary[key3] = num7.Value;
								}
							}
						}
					}
					keys[controlFlowBlock.Id] = blockKey;
				}
			}
			while (flag);
			bool flag24 = random != null;
			if (flag24)
			{
				Dictionary<uint, uint> dictionary3 = new Dictionary<uint, uint>();
				for (int j = 0; j < keys.Length; j++)
				{
					BlockKey blockKey2 = keys[j];
					uint entryState = blockKey2.EntryState;
					bool flag25 = !dictionary3.TryGetValue(entryState, out blockKey2.EntryState);
					if (flag25)
					{
						blockKey2.EntryState = (dictionary3[entryState] = random.NextUInt32());
					}
					uint exitState = blockKey2.ExitState;
					bool flag26 = !dictionary3.TryGetValue(exitState, out blockKey2.ExitState);
					if (flag26)
					{
						blockKey2.ExitState = (dictionary3[exitState] = random.NextUInt32());
					}
					keys[j] = blockKey2;
				}
			}
		}
	}
}
