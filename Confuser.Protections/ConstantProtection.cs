using System;
using Confuser.Core;
using Confuser.Protections.Constants;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000015 RID: 21
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	[AfterProtection(new string[]
	{
		"Ki.RefProxy"
	})]
	internal class ConstantProtection : Protection, IConstantService
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000065 RID: 101 RVA: 0x00002306 File Offset: 0x00000506
		public override string Name
		{
			get
			{
				return "Constants Protection";
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000066 RID: 102 RVA: 0x0000230D File Offset: 0x0000050D
		public override string Description
		{
			get
			{
				return "This protection encodes and compresses constants in the code.";
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000231B File Offset: 0x0000051B
		public override string Id
		{
			get
			{
				return "constants";
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00002322 File Offset: 0x00000522
		public override string FullId
		{
			get
			{
				return "Ki.Constants";
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00002329 File Offset: 0x00000529
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002339 File Offset: 0x00000539
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.Constants", typeof(IConstantService), this);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00002357 File Offset: 0x00000557
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new InjectPhase(this));
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new LocalToFieldPhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new EncodePhase(this));
		}

		// Token: 0x0400001C RID: 28
		public const string _Id = "constants";

		// Token: 0x0400001D RID: 29
		public const string _FullId = "Ki.Constants";

		// Token: 0x0400001E RID: 30
		public const string _ServiceId = "Ki.Constants";

		// Token: 0x0400001F RID: 31
		internal static readonly object ContextKey = new object();
	}
}
