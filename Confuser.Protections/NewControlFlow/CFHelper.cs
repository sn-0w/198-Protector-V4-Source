using System;
using System.Collections.Generic;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.NewControlFlow
{
	// Token: 0x0200007D RID: 125
	public class CFHelper
	{
		// Token: 0x0600023F RID: 575 RVA: 0x0000DAAC File Offset: 0x0000BCAC
		public bool HasUnsafeInstructions(MethodDef methodDef)
		{
			bool hasBody = methodDef.HasBody;
			if (hasBody)
			{
				bool hasVariables = methodDef.Body.HasVariables;
				if (hasVariables)
				{
					return methodDef.Body.Variables.Any((Local x) => x.Type.IsPointer);
				}
			}
			return false;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000DB0C File Offset: 0x0000BD0C
		public Blocks GetBlocks(MethodDef method)
		{
			Blocks blocks = new Blocks();
			Block block = new Block();
			int num = 0;
			int num2 = 0;
			block.ID = num;
			num++;
			block.nextBlock = block.ID + 1;
			block.instructions.Add(OpCodes.Nop.ToInstruction());
			blocks.blocks.Add(block);
			block = new Block();
			foreach (Instruction instruction in method.Body.Instructions)
			{
				int num3 = 0;
				int num4;
				instruction.CalculateStackUsage(out num4, out num3);
				block.instructions.Add(instruction);
				num2 += num4 - num3;
				bool flag = num4 == 0;
				if (flag)
				{
					bool flag2 = instruction.OpCode != OpCodes.Nop;
					if (flag2)
					{
						bool flag3 = num2 == 0 || instruction.OpCode == OpCodes.Ret;
						if (flag3)
						{
							block.ID = num;
							num++;
							block.nextBlock = block.ID + 1;
							blocks.blocks.Add(block);
							block = new Block();
						}
					}
				}
			}
			return blocks;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000DC54 File Offset: 0x0000BE54
		public List<Instruction> Calc(int value)
		{
			List<Instruction> list = new List<Instruction>();
			Random random = new Random(Guid.NewGuid().GetHashCode());
			int num = random.Next(0, 100000);
			int num2 = random.Next(0, 100000);
			bool flag = Convert.ToBoolean(random.Next(0, 2));
			list.Add(Instruction.Create(OpCodes.Ldc_I4, value - num + (flag ? (0 - num2) : num2)));
			list.Add(Instruction.Create(OpCodes.Ldc_I4, num));
			list.Add(Instruction.Create(OpCodes.Add));
			list.Add(Instruction.Create(OpCodes.Ldc_I4, num2));
			list.Add(Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
			return list;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000DD24 File Offset: 0x0000BF24
		public bool Simplify(MethodDef method)
		{
			bool flag = method.Parameters == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				method.Body.SimplifyMacros(method.Parameters);
				result = true;
			}
			return result;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000DD5C File Offset: 0x0000BF5C
		public bool Optimize(MethodDef method)
		{
			bool flag = method.Body == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				method.Body.OptimizeMacros();
				method.Body.OptimizeBranches();
				result = true;
			}
			return result;
		}
	}
}
