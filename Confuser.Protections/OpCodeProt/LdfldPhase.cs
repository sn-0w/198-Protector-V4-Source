using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.OpCodeProt
{
	// Token: 0x02000078 RID: 120
	public class LdfldPhase : ProtectionPhase
	{
		// Token: 0x06000231 RID: 561 RVA: 0x00002136 File Offset: 0x00000336
		public LdfldPhase(OpCodeProtection parent) : base(parent)
		{
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types;
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000233 RID: 563 RVA: 0x00002D28 File Offset: 0x00000F28
		public override string Name
		{
			get
			{
				return "Ldfld Protection";
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000D2D4 File Offset: 0x0000B4D4
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId + ".Ldfld");
			foreach (TypeDef typeDef in parameters.Targets.OfType<TypeDef>())
			{
				bool flag = typeDef.InGlobalModuleType();
				if (!flag)
				{
					foreach (MethodDef methodDef in typeDef.Methods)
					{
						bool flag2 = methodDef.InGlobalModuleType();
						if (!flag2)
						{
							bool flag3 = methodDef.FullName.Contains("My.");
							if (!flag3)
							{
								bool isConstructor = methodDef.IsConstructor;
								if (!isConstructor)
								{
									bool flag4 = !methodDef.HasBody;
									if (!flag4)
									{
										for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
										{
											bool flag5 = methodDef.Body.Instructions[i].OpCode == OpCodes.Ldfld;
											if (flag5)
											{
												bool flag6 = i - 1 > 0 && methodDef.Body.Instructions[i - 1].IsLdarg();
												if (flag6)
												{
													Local local = new Local(methodDef.Module.CorLibTypes.Int32);
													methodDef.Body.Variables.Add(local);
													methodDef.Body.Instructions.Insert(i - 1, OpCodes.Ldc_I4.ToInstruction(randomGenerator.NextInt32()));
													methodDef.Body.Instructions.Insert(i, OpCodes.Stloc_S.ToInstruction(local));
													methodDef.Body.Instructions.Insert(i + 1, OpCodes.Ldloc_S.ToInstruction(local));
													methodDef.Body.Instructions.Insert(i + 2, OpCodes.Ldc_I4.ToInstruction(randomGenerator.NextInt32()));
													methodDef.Body.Instructions.Insert(i + 3, OpCodes.Ldarg_0.ToInstruction());
													methodDef.Body.Instructions.Insert(i + 4, OpCodes.Nop.ToInstruction());
													methodDef.Body.Instructions.Insert(i + 6, OpCodes.Nop.ToInstruction());
													methodDef.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Beq_S, methodDef.Body.Instructions[i + 4]));
													methodDef.Body.Instructions.Insert(i + 5, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[i + 8]));
													methodDef.Body.Instructions.Insert(i + 8, new Instruction(OpCodes.Br_S, methodDef.Body.Instructions[i + 9]));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
