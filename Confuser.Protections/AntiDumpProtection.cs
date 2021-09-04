using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000018 RID: 24
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDumpProtection : Protection
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00005658 File Offset: 0x00003858
		public override string Name
		{
			get
			{
				return "Anti Dump Protection";
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00005670 File Offset: 0x00003870
		public override string Description
		{
			get
			{
				return "This protection prevents the assembly from being dumped from memory.";
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00005688 File Offset: 0x00003888
		public override string Id
		{
			get
			{
				return "anti dump";
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000056A0 File Offset: 0x000038A0
		public override string FullId
		{
			get
			{
				return "Ki.AntiDump";
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00004328 File Offset: 0x00002528
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000023E7 File Offset: 0x000005E7
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDumpProtection.AntiDumpPhase(this));
		}

		// Token: 0x04000023 RID: 35
		public const string _Id = "anti dump";

		// Token: 0x04000024 RID: 36
		public const string _FullId = "Ki.AntiDump";

		// Token: 0x02000019 RID: 25
		private class AntiDumpPhase : ProtectionPhase
		{
			// Token: 0x06000084 RID: 132 RVA: 0x00002136 File Offset: 0x00000336
			public AntiDumpPhase(AntiDumpProtection parent) : base(parent)
			{
			}

			// Token: 0x17000040 RID: 64
			// (get) Token: 0x06000085 RID: 133 RVA: 0x000040A4 File Offset: 0x000022A4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000041 RID: 65
			// (get) Token: 0x06000086 RID: 134 RVA: 0x000056B8 File Offset: 0x000038B8
			public override string Name
			{
				get
				{
					return "Anti-dump injection";
				}
			}

			// Token: 0x06000087 RID: 135 RVA: 0x000056D0 File Offset: 0x000038D0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDump");
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Initialize");
					methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method2));
					foreach (IDnlibDef def in enumerable)
					{
						service2.MarkHelper(def, service, (Protection)base.Parent);
					}
				}
			}
		}
	}
}
