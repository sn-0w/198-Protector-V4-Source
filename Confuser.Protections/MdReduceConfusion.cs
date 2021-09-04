using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200014A RID: 330
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class MdReduceConfusion : Protection
	{
		// Token: 0x170001AF RID: 431
		// (get) Token: 0x060005A7 RID: 1447 RVA: 0x00003E28 File Offset: 0x00002028
		public override string Name
		{
			get
			{
				return "Reduce Metadata Confusion";
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x060005A9 RID: 1449 RVA: 0x00003E2F File Offset: 0x0000202F
		public override string Description
		{
			get
			{
				return "This confusion reduce the metadata carried by the assembly by removing unnecessary metadata.\r\n***If your application relys on Reflection, you should not apply this confusion***";
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x060005AA RID: 1450 RVA: 0x00003E36 File Offset: 0x00002036
		public override string Id
		{
			get
			{
				return "reduce md";
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x060005AB RID: 1451 RVA: 0x00003E3D File Offset: 0x0000203D
		public override string FullId
		{
			get
			{
				return "Ki.redmd";
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060005AC RID: 1452 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00003E44 File Offset: 0x00002044
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new MdReduceConfusion.MdReduceConfusionPhase(this));
		}

		// Token: 0x040002AB RID: 683
		public const string _Id = "reduce md";

		// Token: 0x040002AC RID: 684
		public const string _FullId = "Ki.redmd";

		// Token: 0x0200014B RID: 331
		private class MdReduceConfusionPhase : ProtectionPhase
		{
			// Token: 0x060005B0 RID: 1456 RVA: 0x00002E01 File Offset: 0x00001001
			public MdReduceConfusionPhase(MdReduceConfusion parent) : base(parent)
			{
			}

			// Token: 0x170001B5 RID: 437
			// (get) Token: 0x060005B1 RID: 1457 RVA: 0x000021DB File Offset: 0x000003DB
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x170001B6 RID: 438
			// (get) Token: 0x060005B2 RID: 1458 RVA: 0x00003E28 File Offset: 0x00002028
			public override string Name
			{
				get
				{
					return "Reduce Metadata Confusion";
				}
			}

			// Token: 0x060005B3 RID: 1459 RVA: 0x0001FF04 File Offset: 0x0001E104
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IMemberDef memberDef = parameters.Targets as IMemberDef;
				TypeDef typeDef;
				if ((typeDef = (memberDef as TypeDef)) != null && !this.IsTypePublic(typeDef))
				{
					if (typeDef.IsEnum)
					{
						int num = 0;
						while (typeDef.Fields.Count != 1)
						{
							if (typeDef.Fields[num].Name != "value__")
							{
								typeDef.Fields.RemoveAt(num);
							}
							else
							{
								num++;
							}
						}
						return;
					}
				}
				else if (memberDef is EventDef)
				{
					if (memberDef.DeclaringType != null)
					{
						memberDef.DeclaringType.Events.Remove(memberDef as EventDef);
						return;
					}
				}
				else if (memberDef is PropertyDef && memberDef.DeclaringType != null)
				{
					memberDef.DeclaringType.Properties.Remove(memberDef as PropertyDef);
				}
			}

			// Token: 0x060005B4 RID: 1460 RVA: 0x0001FFE0 File Offset: 0x0001E1E0
			private bool IsTypePublic(TypeDef type)
			{
				while (type.IsPublic || type.IsNestedFamily || type.IsNestedFamilyAndAssembly || type.IsNestedFamilyOrAssembly || type.IsNestedPublic || type.IsPublic)
				{
					type = type.DeclaringType;
					if (type == null)
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
