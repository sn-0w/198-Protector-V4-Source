using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.OpCodeProt
{
	// Token: 0x02000076 RID: 118
	public class CallvirtPhase : ProtectionPhase
	{
		// Token: 0x06000229 RID: 553 RVA: 0x00002136 File Offset: 0x00000336
		public CallvirtPhase(OpCodeProtection parent) : base(parent)
		{
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x0600022A RID: 554 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types;
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x0600022B RID: 555 RVA: 0x00002D1A File Offset: 0x00000F1A
		public override string Name
		{
			get
			{
				return "Callvirt Protection";
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000CB64 File Offset: 0x0000AD64
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId + ".Callvirt");
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
											bool flag5 = methodDef.Body.Instructions[i].OpCode == OpCodes.Callvirt;
											if (flag5)
											{
												bool flag6 = methodDef.Body.Instructions[i].Operand.ToString().ToLower().Contains("int32");
												if (flag6)
												{
													bool flag7 = i - 1 > 0 && methodDef.Body.Instructions[i - 1].IsLdloc();
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
}
