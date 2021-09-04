using System;
using Confuser.Core;
using Confuser.Protections.FakeObuscator;

namespace Confuser.Protections
{
	// Token: 0x0200001F RID: 31
	public class FakeObfuscatorProtection : Protection
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00002433 File Offset: 0x00000633
		public override string Name
		{
			get
			{
				return "Fake Obfuscator Protection";
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000243A File Offset: 0x0000063A
		public override string Description
		{
			get
			{
				return "Confuses deobfuscators like de4dot by adding types typical to other obfuscators.";
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00002441 File Offset: 0x00000641
		public override string Author
		{
			get
			{
				return "HoLLy";
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00002448 File Offset: 0x00000648
		public override string Id
		{
			get
			{
				return "fake obfuscator";
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000244F File Offset: 0x0000064F
		public override string FullId
		{
			get
			{
				return "HoLLy.FakeObfuscator";
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00002456 File Offset: 0x00000656
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new FakeObfuscatorTypesPhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new FakeObfuscatorAttributesPhase(this));
		}
	}
}
