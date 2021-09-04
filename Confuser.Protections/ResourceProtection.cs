using System;
using Confuser.Core;
using Confuser.Protections.Resources;

namespace Confuser.Protections
{
	// Token: 0x02000031 RID: 49
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	internal class ResourceProtection : Protection
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00006948 File Offset: 0x00004B48
		public override string Name
		{
			get
			{
				return "Resources Protection";
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00006960 File Offset: 0x00004B60
		public override string Description
		{
			get
			{
				return "This protection encodes and compresses the embedded resources.";
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600010E RID: 270 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00006978 File Offset: 0x00004B78
		public override string Id
		{
			get
			{
				return "resources";
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000110 RID: 272 RVA: 0x00006990 File Offset: 0x00004B90
		public override string FullId
		{
			get
			{
				return "Ki.Resources";
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000111 RID: 273 RVA: 0x000069A8 File Offset: 0x00004BA8
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000113 RID: 275 RVA: 0x000026D4 File Offset: 0x000008D4
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
		}

		// Token: 0x0400003C RID: 60
		public const string _Id = "resources";

		// Token: 0x0400003D RID: 61
		public const string _FullId = "Ki.Resources";

		// Token: 0x0400003E RID: 62
		public const string _ServiceId = "Ki.Resources";
	}
}
