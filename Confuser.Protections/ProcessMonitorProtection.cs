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
	// Token: 0x02000112 RID: 274
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.Constants"
	})]
	internal class ProcessMonitorProtection : Protection
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00003976 File Offset: 0x00001B76
		public override string Name
		{
			get
			{
				return "Process Monitor Protection";
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x0000397D File Offset: 0x00001B7D
		public override string Description
		{
			get
			{
				return "This protection prevents malicious processes from running.";
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x00003984 File Offset: 0x00001B84
		public override string Id
		{
			get
			{
				return "process monitor";
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x0000398B File Offset: 0x00001B8B
		public override string FullId
		{
			get
			{
				return "Ki.ProcessMonitor";
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600049E RID: 1182 RVA: 0x00003992 File Offset: 0x00001B92
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ProcessMonitorProtection.ProcessMonitorPhase(this));
		}

		// Token: 0x0400025B RID: 603
		public const string _Id = "process monitor";

		// Token: 0x0400025C RID: 604
		public const string _FullId = "Ki.ProcessMonitor";

		// Token: 0x02000113 RID: 275
		private class ProcessMonitorPhase : ProtectionPhase
		{
			// Token: 0x060004A0 RID: 1184 RVA: 0x00002E01 File Offset: 0x00001001
			public ProcessMonitorPhase(ProcessMonitorProtection parent) : base(parent)
			{
			}

			// Token: 0x17000129 RID: 297
			// (get) Token: 0x060004A1 RID: 1185 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x060004A2 RID: 1186 RVA: 0x000039A1 File Offset: 0x00001BA1
			public override string Name
			{
				get
				{
					return "Process Monitor";
				}
			}

			// Token: 0x060004A3 RID: 1187 RVA: 0x0001D984 File Offset: 0x0001BB84
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.ProcessMonitor");
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Init");
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
