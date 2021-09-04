using System;
using Confuser.Core;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x0200010C RID: 268
	public class MemoryEditProtection : Protection
	{
		// Token: 0x1700011D RID: 285
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x00003852 File Offset: 0x00001A52
		public override string Name
		{
			get
			{
				return "Anti Memory Editing";
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600047D RID: 1149 RVA: 0x00003859 File Offset: 0x00001A59
		public override string Description
		{
			get
			{
				return "Prevent memory editing on selected variables.";
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600047E RID: 1150 RVA: 0x00002441 File Offset: 0x00000641
		public override string Author
		{
			get
			{
				return "HoLLy";
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x00003860 File Offset: 0x00001A60
		public override string Id
		{
			get
			{
				return "memory protection";
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000480 RID: 1152 RVA: 0x00003867 File Offset: 0x00001A67
		public override string FullId
		{
			get
			{
				return "HoLLy.MemoryProtection";
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06000481 RID: 1153 RVA: 0x0000257A File Offset: 0x0000077A
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.None;
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0000386E File Offset: 0x00001A6E
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService(this.Id, typeof(IMemoryEditService), new MemoryEditService());
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00003892 File Offset: 0x00001A92
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.Inspection, new MemoryEditAnalyzePhase(this));
			pipeline.InsertPostStage(PipelineStage.BeginModule, new MemoryEditInjectPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new MemoryEditApplyPhase(this));
		}
	}
}
