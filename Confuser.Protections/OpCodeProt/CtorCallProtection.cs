using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.OpCodeProt
{
	// Token: 0x02000077 RID: 119
	public class CtorCallProtection : ProtectionPhase
	{
		// Token: 0x0600022D RID: 557 RVA: 0x00002136 File Offset: 0x00000336
		public CtorCallProtection(OpCodeProtection parent) : base(parent)
		{
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x0600022E RID: 558 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types;
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x0600022F RID: 559 RVA: 0x00002D21 File Offset: 0x00000F21
		public override string Name
		{
			get
			{
				return "Ctor Call Protection";
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000CF1C File Offset: 0x0000B11C
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId + ".CtorCall");
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
											bool flag5 = methodDef.Body.Instructions[i].OpCode == OpCodes.Call;
											if (flag5)
											{
												bool flag6 = methodDef.Body.Instructions[i].Operand.ToString().ToLower().Contains("void");
												if (flag6)
												{
													bool flag7 = i - 1 > 0 && methodDef.Body.Instructions[i - 1].IsLdarg();
													if (flag7)
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
														methodDef.Body.Instructions.Insert(i + 3, new Instruction(OpCodes.Bne_Un_S, methodDef.Body.Instructions[i + 4]));
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
}
