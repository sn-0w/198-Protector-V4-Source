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
	// Token: 0x0200011B RID: 283
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.AntiTamper"
	})]
	internal class ModuleFlood : Protection
	{
		// Token: 0x060004C5 RID: 1221 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x00003A2F File Offset: 0x00001C2F
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new ModuleFlood.ModuleFloodPhase(this));
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x00003A3E File Offset: 0x00001C3E
		public override string Description
		{
			get
			{
				return "Protection flood for module.cctor.";
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00003A45 File Offset: 0x00001C45
		public override string FullId
		{
			get
			{
				return "Ki.ModuleFlood";
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060004CA RID: 1226 RVA: 0x00003A4C File Offset: 0x00001C4C
		public override string Id
		{
			get
			{
				return "module flood";
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x00003A53 File Offset: 0x00001C53
		public override string Name
		{
			get
			{
				return "Module Flood Protection";
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060004CC RID: 1228 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x04000265 RID: 613
		public const string _FullId = "Ki.ModuleFlood";

		// Token: 0x04000266 RID: 614
		public const string _Id = "module flood";

		// Token: 0x0200011C RID: 284
		private class ModuleFloodPhase : ProtectionPhase
		{
			// Token: 0x060004CE RID: 1230 RVA: 0x00002E01 File Offset: 0x00001001
			public ModuleFloodPhase(ModuleFlood parent) : base(parent)
			{
			}

			// Token: 0x060004CF RID: 1231 RVA: 0x0001E0E0 File Offset: 0x0001C2E0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.ModuleFlood");
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					Random random = new Random(DateTime.Now.Millisecond);
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					for (int i = 0; i < random.Next(100, 1000); i++)
					{
						methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Initialize")));
					}
					foreach (IDnlibDef def in enumerable)
					{
						service2.MarkHelper(def, service, (Protection)base.Parent);
					}
				}
			}

			// Token: 0x17000141 RID: 321
			// (get) Token: 0x060004D0 RID: 1232 RVA: 0x00003A5A File Offset: 0x00001C5A
			public override string Name
			{
				get
				{
					return "Module Flooding";
				}
			}

			// Token: 0x17000142 RID: 322
			// (get) Token: 0x060004D1 RID: 1233 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}
		}
	}
}
