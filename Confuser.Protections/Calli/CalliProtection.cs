using System;
using Confuser.Core;

namespace Confuser.Protections.Calli
{
	// Token: 0x020000D4 RID: 212
	[BeforeProtection(new string[]
	{
		"Ki.Constants",
		"Ki.AntiDebug",
		"Ki.AntiDump"
	})]
	internal class CalliProtection : Protection
	{
		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000382 RID: 898 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000383 RID: 899 RVA: 0x00003359 File Offset: 0x00001559
		public override string Name
		{
			get
			{
				return "Calli Protection";
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000384 RID: 900 RVA: 0x00003360 File Offset: 0x00001560
		public override string Description
		{
			get
			{
				return "Replaces Calls with Calli \nNOTE: NOT compatible with cflow";
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000385 RID: 901 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000386 RID: 902 RVA: 0x00003367 File Offset: 0x00001567
		public override string Id
		{
			get
			{
				return "calli";
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000336E File Offset: 0x0000156E
		public override string FullId
		{
			get
			{
				return "Wadu.Calli";
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00003375 File Offset: 0x00001575
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new CalliPhase(this));
		}

		// Token: 0x040001BD RID: 445
		internal static readonly object ContextKey = new object();
	}
}
