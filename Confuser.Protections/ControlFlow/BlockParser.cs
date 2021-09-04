using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000086 RID: 134
	internal static class BlockParser
	{
		// Token: 0x06000263 RID: 611 RVA: 0x0000F3B4 File Offset: 0x0000D5B4
		public static ScopeBlock ParseBody(CilBody body)
		{
			Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>> dictionary = new Dictionary<ExceptionHandler, Tuple<ScopeBlock, ScopeBlock, ScopeBlock>>();
			foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
			{
				ScopeBlock item = new ScopeBlock(BlockType.Try, exceptionHandler);
				BlockType type = BlockType.Handler;
				if (exceptionHandler.HandlerType == ExceptionHandlerType.Finally)
				{
					type = BlockType.Finally;
				}
				else if (exceptionHandler.HandlerType == ExceptionHandlerType.Fault)
				{
					type = BlockType.Fault;
				}
				ScopeBlock item2 = new ScopeBlock(type, exceptionHandler);
				if (exceptionHandler.FilterStart == null)
				{
					dictionary[exceptionHandler] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(item, item2, null);
				}
				else
				{
					ScopeBlock item3 = new ScopeBlock(BlockType.Filter, exceptionHandler);
					dictionary[exceptionHandler] = Tuple.Create<ScopeBlock, ScopeBlock, ScopeBlock>(item, item2, item3);
				}
			}
			ScopeBlock scopeBlock = new ScopeBlock(BlockType.Normal, null);
			Stack<ScopeBlock> stack = new Stack<ScopeBlock>();
			stack.Push(scopeBlock);
			foreach (Instruction instruction in body.Instructions)
			{
				foreach (ExceptionHandler exceptionHandler2 in body.ExceptionHandlers)
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> tuple = dictionary[exceptionHandler2];
					if (instruction == exceptionHandler2.TryEnd)
					{
						stack.Pop();
					}
					if (instruction == exceptionHandler2.HandlerEnd)
					{
						stack.Pop();
					}
					if (exceptionHandler2.FilterStart != null && instruction == exceptionHandler2.HandlerStart)
					{
						stack.Pop();
					}
				}
				foreach (ExceptionHandler exceptionHandler3 in body.ExceptionHandlers.Reverse<ExceptionHandler>())
				{
					Tuple<ScopeBlock, ScopeBlock, ScopeBlock> tuple2 = dictionary[exceptionHandler3];
					ScopeBlock scopeBlock2;
					if (stack.Count > 0)
					{
						scopeBlock2 = stack.Peek();
					}
					else
					{
						scopeBlock2 = null;
					}
					ScopeBlock scopeBlock3 = scopeBlock2;
					if (instruction == exceptionHandler3.TryStart)
					{
						if (scopeBlock3 != null)
						{
							scopeBlock3.Children.Add(tuple2.Item1);
						}
						stack.Push(tuple2.Item1);
					}
					if (instruction == exceptionHandler3.HandlerStart)
					{
						if (scopeBlock3 != null)
						{
							scopeBlock3.Children.Add(tuple2.Item2);
						}
						stack.Push(tuple2.Item2);
					}
					if (instruction == exceptionHandler3.FilterStart)
					{
						if (scopeBlock3 != null)
						{
							scopeBlock3.Children.Add(tuple2.Item3);
						}
						stack.Push(tuple2.Item3);
					}
				}
				ScopeBlock scopeBlock4 = stack.Peek();
				InstrBlock instrBlock = scopeBlock4.Children.LastOrDefault<BlockBase>() as InstrBlock;
				if (instrBlock == null)
				{
					List<BlockBase> children = scopeBlock4.Children;
					InstrBlock instrBlock2 = new InstrBlock();
					instrBlock = instrBlock2;
					children.Add(instrBlock2);
				}
				instrBlock.Instructions.Add(instruction);
			}
			foreach (ExceptionHandler exceptionHandler4 in body.ExceptionHandlers)
			{
				if (exceptionHandler4.TryEnd == null)
				{
					stack.Pop();
				}
				if (exceptionHandler4.HandlerEnd == null)
				{
					stack.Pop();
				}
			}
			return scopeBlock;
		}
	}
}
