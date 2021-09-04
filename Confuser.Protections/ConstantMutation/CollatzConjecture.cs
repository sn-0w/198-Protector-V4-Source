using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000BF RID: 191
	public class CollatzConjecture : ProtectionPhase
	{
		// Token: 0x06000332 RID: 818 RVA: 0x00002136 File Offset: 0x00000336
		public CollatzConjecture(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000333 RID: 819 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000334 RID: 820 RVA: 0x000031C4 File Offset: 0x000013C4
		public override string Name
		{
			get
			{
				return "Value Replacer Phase";
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00017140 File Offset: 0x00015340
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = !parameters.Targets.Any<IDnlibDef>();
			if (!flag)
			{
				RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId + ".ValueReplacer");
				IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.ConstantMutation"), context.CurrentModule.GlobalType, context.CurrentModule);
				MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "ConjetMe");
				foreach (IDnlibDef def in enumerable)
				{
					context.Registry.GetService<INameService>().MarkHelper(def, context.Registry.GetService<IMarkerService>(), base.Parent);
				}
				foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>())
				{
					bool isGlobalModuleType = methodDef.DeclaringType.IsGlobalModuleType;
					if (!isGlobalModuleType)
					{
						bool flag2 = methodDef.FullName.Contains("My.");
						if (!flag2)
						{
							bool isConstructor = methodDef.IsConstructor;
							if (!isConstructor)
							{
								bool flag3 = !methodDef.HasBody;
								if (!flag3)
								{
									for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
									{
										bool flag4 = methodDef.Body.Instructions[i].IsLdcI4();
										if (flag4)
										{
											bool flag5 = methodDef.Body.Instructions[i].GetLdcI4Value() == 1;
											if (flag5)
											{
												methodDef.Body.Instructions[i].Operand = randomGenerator.NextInt32(1, 15);
												methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, method2));
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
