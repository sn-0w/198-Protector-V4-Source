using System;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.NewControlFlow;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x0200008C RID: 140
	internal class NewControlFlowPhase2 : ProtectionPhase
	{
		// Token: 0x0600027C RID: 636 RVA: 0x00002136 File Offset: 0x00000336
		public NewControlFlowPhase2(ControlFlowProtection parent) : base(parent)
		{
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x0600027D RID: 637 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00002EC1 File Offset: 0x000010C1
		public override string Name
		{
			get
			{
				return "New Control Flow 2";
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000FD40 File Offset: 0x0000DF40
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				CFHelper cfhelper = new CFHelper();
				bool flag = methodDef.HasBody && methodDef.Body.Instructions.Count > 0 && !methodDef.IsConstructor;
				if (flag)
				{
					bool flag2 = !cfhelper.HasUnsafeInstructions(methodDef);
					if (flag2)
					{
						bool flag3 = cfhelper.Simplify(methodDef);
						if (flag3)
						{
							Blocks blocks = cfhelper.GetBlocks(methodDef);
							bool flag4 = blocks.blocks.Count != 1;
							if (flag4)
							{
								blocks.Scramble(out blocks);
								methodDef.Body.Instructions.Clear();
								Local local = new Local(context.CurrentModule.CorLibTypes.Int32);
								methodDef.Body.Variables.Add(local);
								Instruction instruction = Instruction.Create(OpCodes.Nop);
								Instruction instruction2 = Instruction.Create(OpCodes.Br, instruction);
								foreach (Instruction item in cfhelper.Calc(0))
								{
									methodDef.Body.Instructions.Add(item);
								}
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Br, instruction2));
								methodDef.Body.Instructions.Add(instruction);
								foreach (Block block in blocks.blocks)
								{
									bool flag5 = block != blocks.getBlock(blocks.blocks.Count - 1);
									if (flag5)
									{
										methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
										foreach (Instruction item2 in cfhelper.Calc(block.ID))
										{
											methodDef.Body.Instructions.Add(item2);
										}
										methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
										Instruction instruction3 = Instruction.Create(OpCodes.Nop);
										methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction3));
										foreach (Instruction item3 in block.instructions)
										{
											methodDef.Body.Instructions.Add(item3);
										}
										foreach (Instruction item4 in cfhelper.Calc(block.nextBlock))
										{
											methodDef.Body.Instructions.Add(item4);
										}
										methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Stloc, local));
										methodDef.Body.Instructions.Add(instruction3);
									}
								}
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, local));
								foreach (Instruction item5 in cfhelper.Calc(blocks.blocks.Count - 1))
								{
									methodDef.Body.Instructions.Add(item5);
								}
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ceq));
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse, instruction2));
								methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Br, blocks.getBlock(blocks.blocks.Count - 1).instructions[0]));
								methodDef.Body.Instructions.Add(instruction2);
								foreach (Instruction item6 in blocks.getBlock(blocks.blocks.Count - 1).instructions)
								{
									methodDef.Body.Instructions.Add(item6);
								}
							}
						}
					}
				}
			}
		}
	}
}
