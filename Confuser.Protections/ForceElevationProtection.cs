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
	// Token: 0x02000115 RID: 277
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.Constants"
	})]
	internal class ForceElevationProtection : Protection
	{
		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x000039C6 File Offset: 0x00001BC6
		public override string Name
		{
			get
			{
				return "Force Admin Priviliges Protection";
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x000039CD File Offset: 0x00001BCD
		public override string Description
		{
			get
			{
				return "This protection will force your file to run under Administrative priviliges to ensure that the process isn't easily tampered with.";
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x000039D4 File Offset: 0x00001BD4
		public override string Id
		{
			get
			{
				return "force elevation";
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x000039DB File Offset: 0x00001BDB
		public override string FullId
		{
			get
			{
				return "Ki.ForceElevation";
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x000039E2 File Offset: 0x00001BE2
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ForceElevationProtection.ForceElevationPhase(this));
		}

		// Token: 0x0400025F RID: 607
		public const string _Id = "force elevation";

		// Token: 0x04000260 RID: 608
		public const string _FullId = "Ki.ForceElevation";

		// Token: 0x02000116 RID: 278
		private class ForceElevationPhase : ProtectionPhase
		{
			// Token: 0x060004B0 RID: 1200 RVA: 0x00002E01 File Offset: 0x00001001
			public ForceElevationPhase(ForceElevationProtection parent) : base(parent)
			{
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x060004B1 RID: 1201 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x060004B2 RID: 1202 RVA: 0x000039F1 File Offset: 0x00001BF1
			public override string Name
			{
				get
				{
					return "Force Admin Priviliges";
				}
			}

			// Token: 0x060004B3 RID: 1203 RVA: 0x0001DAB4 File Offset: 0x0001BCB4
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.ForceElevation");
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
