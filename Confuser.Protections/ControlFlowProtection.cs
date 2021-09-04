using System;
using Confuser.Core;
using Confuser.Protections.ControlFlow;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000017 RID: 23
	internal class ControlFlowProtection : Protection, IControlFlowService
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00002390 File Offset: 0x00000590
		public override string Name
		{
			get
			{
				return "Control Flow Custom Protection";
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00002397 File Offset: 0x00000597
		public override string Description
		{
			get
			{
				return "This protection mangles the code in the methods so that decompilers cannot decompile the methods.";
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000073 RID: 115 RVA: 0x0000239E File Offset: 0x0000059E
		public override string Author
		{
			get
			{
				return "Ki (yck1509) & Wadu";
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000074 RID: 116 RVA: 0x000023A5 File Offset: 0x000005A5
		public override string Id
		{
			get
			{
				return "ctrl flow";
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000075 RID: 117 RVA: 0x000023AC File Offset: 0x000005AC
		public override string FullId
		{
			get
			{
				return "Ki.ControlFlow";
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00002329 File Offset: 0x00000529
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000023B3 File Offset: 0x000005B3
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.ControlFlow", typeof(IControlFlowService), this);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000023D0 File Offset: 0x000005D0
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new ControlFlowPhase(this));
		}

		// Token: 0x04000020 RID: 32
		public const string _Id = "ctrl flow";

		// Token: 0x04000021 RID: 33
		public const string _FullId = "Ki.ControlFlow";

		// Token: 0x04000022 RID: 34
		public const string _ServiceId = "Ki.ControlFlow";
	}
}
