using System;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.AntiTamper;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000007 RID: 7
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	internal class AntiTamperProtection : Protection, IAntiTamperService
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000042C8 File Offset: 0x000024C8
		public override string Name
		{
			get
			{
				return "Anti Tamper Protection";
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000042E0 File Offset: 0x000024E0
		public override string Description
		{
			get
			{
				return "This protection ensures the integrity of application.";
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600001E RID: 30 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000042F8 File Offset: 0x000024F8
		public override string Id
		{
			get
			{
				return "anti tamper";
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00004310 File Offset: 0x00002510
		public override string FullId
		{
			get
			{
				return "Ki.AntiTamper";
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00004328 File Offset: 0x00002528
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000215D File Offset: 0x0000035D
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.AntiTamper", typeof(IAntiTamperService), this);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000217C File Offset: 0x0000037C
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.OptimizeMethods, new AntiTamperProtection.InjectPhase(this));
			pipeline.InsertPreStage(PipelineStage.EndModule, new AntiTamperProtection.MDPhase(this));
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000219B File Offset: 0x0000039B
		public void ExcludeMethod(ConfuserContext context, MethodDef method)
		{
			ProtectionParameters.GetParameters(context, method).Remove(this);
		}

		// Token: 0x04000003 RID: 3
		public const string _Id = "anti tamper";

		// Token: 0x04000004 RID: 4
		public const string _FullId = "Ki.AntiTamper";

		// Token: 0x04000005 RID: 5
		public const string _ServiceId = "Ki.AntiTamper";

		// Token: 0x04000006 RID: 6
		private static readonly object HandlerKey = new object();

		// Token: 0x02000008 RID: 8
		private class InjectPhase : ProtectionPhase
		{
			// Token: 0x06000027 RID: 39 RVA: 0x00002136 File Offset: 0x00000336
			public InjectPhase(AntiTamperProtection parent) : base(parent)
			{
			}

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x06000028 RID: 40 RVA: 0x0000433C File Offset: 0x0000253C
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x06000029 RID: 41 RVA: 0x00004350 File Offset: 0x00002550
			public override string Name
			{
				get
				{
					return "Anti-tamper helpers injection";
				}
			}

			// Token: 0x0600002A RID: 42 RVA: 0x00004368 File Offset: 0x00002568
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = !parameters.Targets.Any<IDnlibDef>();
				if (!flag)
				{
					IModeHandler modeHandler;
					switch (parameters.GetParameter<AntiTamperProtection.Mode>(context, context.CurrentModule, "mode", AntiTamperProtection.Mode.Anti))
					{
					case AntiTamperProtection.Mode.Normal:
						modeHandler = new NormalMode();
						break;
					case AntiTamperProtection.Mode.Anti:
						modeHandler = new AntiMode();
						break;
					case AntiTamperProtection.Mode.JIT:
						modeHandler = new JITMode();
						break;
					default:
						throw new UnreachableException();
					}
					modeHandler.HandleInject((AntiTamperProtection)base.Parent, context, parameters);
					context.Annotations.Set<IModeHandler>(context.CurrentModule, AntiTamperProtection.HandlerKey, modeHandler);
				}
			}
		}

		// Token: 0x02000009 RID: 9
		private class MDPhase : ProtectionPhase
		{
			// Token: 0x0600002B RID: 43 RVA: 0x00002136 File Offset: 0x00000336
			public MDPhase(AntiTamperProtection parent) : base(parent)
			{
			}

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x0600002C RID: 44 RVA: 0x0000433C File Offset: 0x0000253C
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x0600002D RID: 45 RVA: 0x00004404 File Offset: 0x00002604
			public override string Name
			{
				get
				{
					return "Anti-tamper metadata preparation";
				}
			}

			// Token: 0x0600002E RID: 46 RVA: 0x0000441C File Offset: 0x0000261C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = !parameters.Targets.Any<IDnlibDef>();
				if (!flag)
				{
					IModeHandler modeHandler = context.Annotations.Get<IModeHandler>(context.CurrentModule, AntiTamperProtection.HandlerKey, null);
					modeHandler.HandleMD((AntiTamperProtection)base.Parent, context, parameters);
				}
			}
		}

		// Token: 0x0200000A RID: 10
		private enum Mode
		{
			// Token: 0x04000008 RID: 8
			Normal,
			// Token: 0x04000009 RID: 9
			Anti,
			// Token: 0x0400000A RID: 10
			JIT
		}
	}
}
