using System;
using Confuser.Core;
using Confuser.Protections.OpCodeProt;

namespace Confuser.Protections
{
	// Token: 0x0200002E RID: 46
	[AfterProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	public class OpCodeProtection : Protection
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Aggressive;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000025E6 File Offset: 0x000007E6
		public override string Name
		{
			get
			{
				return "OpCode Protection";
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000025ED File Offset: 0x000007ED
		public override string Description
		{
			get
			{
				return "Protects OpCodes such as Ldlfd.";
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x000025F4 File Offset: 0x000007F4
		public override string Id
		{
			get
			{
				return "opcode prot";
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000025FB File Offset: 0x000007FB
		public override string FullId
		{
			get
			{
				return "Wadu.OpCode";
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00002602 File Offset: 0x00000802
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new LdfldPhase(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new CallvirtPhase(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new CtorCallProtection(this));
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new MultiplyPhase(this));
		}
	}
}
