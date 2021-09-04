using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000032 RID: 50
	[BeforeProtection(new string[]
	{
		"Ki.Constants",
		"Ki.ControlFlow"
	})]
	public class StackUnderflowProtection : Protection
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Aggressive;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000116 RID: 278 RVA: 0x000026E5 File Offset: 0x000008E5
		public override string Name
		{
			get
			{
				return "Stack Underflow Protection";
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000026EC File Offset: 0x000008EC
		public override string Description
		{
			get
			{
				return "This protection will add a piece of code in front of a method and cause some decompilers to crash.";
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000118 RID: 280 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000119 RID: 281 RVA: 0x000026F3 File Offset: 0x000008F3
		public override string Id
		{
			get
			{
				return "stack underflow";
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000026FA File Offset: 0x000008FA
		public override string FullId
		{
			get
			{
				return "Wadu.StackUnderflow";
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00002701 File Offset: 0x00000901
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new StackUnderflowProtection.StackUnderflowPhase(this));
		}

		// Token: 0x02000033 RID: 51
		private class StackUnderflowPhase : ProtectionPhase
		{
			// Token: 0x0600011E RID: 286 RVA: 0x00002136 File Offset: 0x00000336
			public StackUnderflowPhase(StackUnderflowProtection parent) : base(parent)
			{
			}

			// Token: 0x1700008A RID: 138
			// (get) Token: 0x0600011F RID: 287 RVA: 0x000021DB File Offset: 0x000003DB
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x1700008B RID: 139
			// (get) Token: 0x06000120 RID: 288 RVA: 0x00002711 File Offset: 0x00000911
			public override string Name
			{
				get
				{
					return "Stack Underflow";
				}
			}

			// Token: 0x06000121 RID: 289 RVA: 0x000069BC File Offset: 0x00004BBC
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId);
				foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>())
				{
					bool flag = !methodDef.HasBody;
					if (!flag)
					{
						Instruction instruction = methodDef.Body.Instructions[0];
						Instruction instruction2 = Instruction.Create(OpCodes.Br, instruction);
						Instruction item;
						switch (randomGenerator.NextInt32(0, 3))
						{
						case 0:
							item = Instruction.Create(OpCodes.Ldnull);
							break;
						case 1:
							item = Instruction.Create(OpCodes.Ldc_I4_0);
							break;
						case 2:
							item = Instruction.Create(OpCodes.Ldstr, "");
							break;
						default:
							item = Instruction.Create(OpCodes.Ldc_I8, (long)randomGenerator.NextInt32());
							break;
						}
						methodDef.Body.Instructions.Insert(0, item);
						methodDef.Body.Instructions.Insert(1, Instruction.Create(OpCodes.Pop));
						methodDef.Body.Instructions.Insert(2, instruction2);
						foreach (ExceptionHandler exceptionHandler in methodDef.Body.ExceptionHandlers)
						{
							bool flag2 = exceptionHandler.TryStart == instruction;
							if (flag2)
							{
								exceptionHandler.TryStart = instruction2;
							}
							else
							{
								bool flag3 = exceptionHandler.HandlerStart == instruction;
								if (flag3)
								{
									exceptionHandler.HandlerStart = instruction2;
								}
								else
								{
									bool flag4 = exceptionHandler.FilterStart == instruction;
									if (flag4)
									{
										exceptionHandler.FilterStart = instruction2;
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
