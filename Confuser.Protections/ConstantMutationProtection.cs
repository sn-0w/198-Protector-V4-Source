using System;
using Confuser.Core;
using Confuser.Protections.ConstantMutation;

namespace Confuser.Protections
{
	// Token: 0x02000013 RID: 19
	[BeforeProtection(new string[]
	{
		"Ki.Constants",
		"Ki.ControlFlow"
	})]
	public class ConstantMutationProtection : Protection
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600005B RID: 91 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Aggressive;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600005C RID: 92 RVA: 0x000022EA File Offset: 0x000004EA
		public override string Name
		{
			get
			{
				return "Constant Mutation";
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600005D RID: 93 RVA: 0x000022F1 File Offset: 0x000004F1
		public override string Description
		{
			get
			{
				return "Mutates Constants into Expressions";
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000022F8 File Offset: 0x000004F8
		public override string Id
		{
			get
			{
				return "constant mutate";
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000022FF File Offset: 0x000004FF
		public override string FullId
		{
			get
			{
				return "Wadu.ConstantMutation";
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005604 File Offset: 0x00003804
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ZeroReplacerPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new SimpleMathPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ArithemticPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new CollatzConjecture(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new NumberToStringPhase(this));
		}
	}
}
