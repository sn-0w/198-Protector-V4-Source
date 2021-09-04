using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x0200008B RID: 139
	internal class NewControlFlowPhase : ProtectionPhase
	{
		// Token: 0x06000278 RID: 632 RVA: 0x00002136 File Offset: 0x00000336
		public NewControlFlowPhase(ControlFlowProtection parent) : base(parent)
		{
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000279 RID: 633 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600027A RID: 634 RVA: 0x00002EBA File Offset: 0x000010BA
		public override string Name
		{
			get
			{
				return "New Control Flow";
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000FA24 File Offset: 0x0000DC24
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = !methodDef.HasBody;
				if (!flag)
				{
					bool flag2 = !methodDef.Body.HasInstructions;
					if (!flag2)
					{
						for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
						{
							bool flag3 = methodDef.Body.Instructions[i].IsLdcI4();
							if (flag3)
							{
								int num = new Random(Guid.NewGuid().GetHashCode()).Next();
								int num2 = new Random(Guid.NewGuid().GetHashCode()).Next();
								int value = num ^ num2;
								Instruction instruction = OpCodes.Nop.ToInstruction();
								Local local = new Local(methodDef.Module.ImportAsTypeSig(typeof(int)));
								methodDef.Body.Variables.Add(local);
								methodDef.Body.Instructions.Insert(i + 1, OpCodes.Stloc.ToInstruction(local));
								methodDef.Body.Instructions.Insert(i + 2, Instruction.Create(OpCodes.Ldc_I4, methodDef.Body.Instructions[i].GetLdcI4Value() - 4));
								methodDef.Body.Instructions.Insert(i + 3, Instruction.Create(OpCodes.Ldc_I4, value));
								methodDef.Body.Instructions.Insert(i + 4, Instruction.Create(OpCodes.Ldc_I4, num2));
								methodDef.Body.Instructions.Insert(i + 5, Instruction.Create(OpCodes.Xor));
								methodDef.Body.Instructions.Insert(i + 6, Instruction.Create(OpCodes.Ldc_I4, num));
								methodDef.Body.Instructions.Insert(i + 7, Instruction.Create(OpCodes.Bne_Un, instruction));
								methodDef.Body.Instructions.Insert(i + 8, Instruction.Create(OpCodes.Ldc_I4, 2));
								methodDef.Body.Instructions.Insert(i + 9, OpCodes.Stloc.ToInstruction(local));
								methodDef.Body.Instructions.Insert(i + 10, Instruction.Create(OpCodes.Sizeof, methodDef.Module.Import(typeof(float))));
								methodDef.Body.Instructions.Insert(i + 11, Instruction.Create(OpCodes.Add));
								methodDef.Body.Instructions.Insert(i + 12, instruction);
								i += 12;
							}
						}
					}
				}
			}
		}
	}
}
