using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200000B RID: 11
	public class AntiWatermarkProtection : Protection
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000021B8 File Offset: 0x000003B8
		public override string Name
		{
			get
			{
				return "Anti Watermark";
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000021BF File Offset: 0x000003BF
		public override string Description
		{
			get
			{
				return "Removes the ProtectedBy watermark to prevent ElektroProtector detection.";
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000031 RID: 49 RVA: 0x000021C6 File Offset: 0x000003C6
		public override string Author
		{
			get
			{
				return "HoLLy ";
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000021CD File Offset: 0x000003CD
		public override string Id
		{
			get
			{
				return "anti watermark";
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000033 RID: 51 RVA: 0x000021D4 File Offset: 0x000003D4
		public override string FullId
		{
			get
			{
				return "HoLLy.AntiWatermark";
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000034 RID: 52 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000036 RID: 54 RVA: 0x000021DE File Offset: 0x000003DE
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.Inspection, new AntiWatermarkProtection.AntiWatermarkPhase(this));
		}

		// Token: 0x0200000C RID: 12
		public class AntiWatermarkPhase : ProtectionPhase
		{
			// Token: 0x17000021 RID: 33
			// (get) Token: 0x06000038 RID: 56 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x06000039 RID: 57 RVA: 0x000021EF File Offset: 0x000003EF
			public override string Name
			{
				get
				{
					return "ProtectedBy attribute removal";
				}
			}

			// Token: 0x0600003A RID: 58 RVA: 0x00002136 File Offset: 0x00000336
			public AntiWatermarkPhase(ConfuserComponent parent) : base(parent)
			{
			}

			// Token: 0x0600003B RID: 59 RVA: 0x0000446C File Offset: 0x0000266C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger))
				{
					CustomAttribute customAttribute = moduleDef.CustomAttributes.Find("ProtectedByAttribute");
					bool flag = customAttribute != null;
					if (flag)
					{
						moduleDef.CustomAttributes.Remove(customAttribute);
						moduleDef.Types.Remove((TypeDef)customAttribute.AttributeType);
					}
				}
			}
		}
	}
}
