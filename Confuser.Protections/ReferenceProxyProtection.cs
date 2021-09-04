using System;
using Confuser.Core;
using Confuser.Protections.ReferenceProxy;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000030 RID: 48
	[AfterProtection(new string[]
	{
		"Ki.AntiDebug",
		"Ki.AntiDump"
	})]
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Wadu.ConstantMutation"
	})]
	internal class ReferenceProxyProtection : Protection, IReferenceProxyService
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000263D File Offset: 0x0000083D
		public override string Name
		{
			get
			{
				return "Reference Proxy Protection";
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000100 RID: 256 RVA: 0x00002644 File Offset: 0x00000844
		public override string Description
		{
			get
			{
				return "This protection encodes and hides references to type/method/fields.";
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00002652 File Offset: 0x00000852
		public override string Id
		{
			get
			{
				return "ref proxy";
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00002659 File Offset: 0x00000859
		public override string FullId
		{
			get
			{
				return "Ki.RefProxy";
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000104 RID: 260 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00002329 File Offset: 0x00000529
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00002660 File Offset: 0x00000860
		public void ExcludeTarget(ConfuserContext context, MethodDef method)
		{
			context.Annotations.Set<object>(method, ReferenceProxyProtection.TargetExcluded, ReferenceProxyProtection.TargetExcluded);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00002679 File Offset: 0x00000879
		public bool IsTargeted(ConfuserContext context, MethodDef method)
		{
			return context.Annotations.Get<object>(method, ReferenceProxyProtection.Targeted, null) != null;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00002690 File Offset: 0x00000890
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.RefProxy", typeof(IReferenceProxyService), this);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x000026AE File Offset: 0x000008AE
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ReferenceProxyPhase(this));
		}

		// Token: 0x04000037 RID: 55
		public const string _Id = "ref proxy";

		// Token: 0x04000038 RID: 56
		public const string _FullId = "Ki.RefProxy";

		// Token: 0x04000039 RID: 57
		public const string _ServiceId = "Ki.RefProxy";

		// Token: 0x0400003A RID: 58
		internal static object TargetExcluded = new object();

		// Token: 0x0400003B RID: 59
		internal static object Targeted = new object();
	}
}
