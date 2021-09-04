using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000C2 RID: 194
	public class ZeroReplacerPhase : ProtectionPhase
	{
		// Token: 0x0600033E RID: 830 RVA: 0x00002136 File Offset: 0x00000336
		public ZeroReplacerPhase(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x0600033F RID: 831 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000340 RID: 832 RVA: 0x000031F0 File Offset: 0x000013F0
		public override string Name
		{
			get
			{
				return "Zero Replacer";
			}
		}

		// Token: 0x06000341 RID: 833 RVA: 0x000177E8 File Offset: 0x000159E8
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId + ".ZeroReplacer");
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>())
			{
				bool isGlobalModuleType = methodDef.DeclaringType.IsGlobalModuleType;
				if (!isGlobalModuleType)
				{
					bool flag = methodDef.FullName.Contains("My.");
					if (!flag)
					{
						bool isConstructor = methodDef.IsConstructor;
						if (!isConstructor)
						{
							bool flag2 = !methodDef.HasBody;
							if (!flag2)
							{
								for (int i = 0; i < 2; i++)
								{
									for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
									{
										bool flag3 = methodDef.Body.Instructions[j].IsLdcI4();
										if (flag3)
										{
											bool flag4 = methodDef.Body.Instructions[j].GetLdcI4Value() == 0;
											if (flag4)
											{
												int num = randomGenerator.NextInt32(0, 2);
												int num2 = num;
												if (num2 != 0)
												{
													if (num2 == 1)
													{
														methodDef.Body.Instructions.Insert(j + 1, OpCodes.Sub.ToInstruction());
													}
												}
												else
												{
													methodDef.Body.Instructions.Insert(j + 1, OpCodes.Add.ToInstruction());
												}
												methodDef.Body.Instructions.Insert(j + 1, OpCodes.Ldsfld.ToInstruction(methodDef.Module.Import(typeof(Type).GetField("EmptyTypes"))));
												methodDef.Body.Instructions.Insert(j + 2, OpCodes.Ldlen.ToInstruction());
												j += 2;
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
