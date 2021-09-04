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
	// Token: 0x0200011E RID: 286
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.Constants",
		"Ki.ForceElevation"
	})]
	internal class AntiFiddlerProtection : Protection
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x00003A6D File Offset: 0x00001C6D
		public override string Name
		{
			get
			{
				return "Anti Fiddler Protection";
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x00003A74 File Offset: 0x00001C74
		public override string Description
		{
			get
			{
				return "This protection will remove the program if the user have fiddler on his computer.";
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x00003A7B File Offset: 0x00001C7B
		public override string Id
		{
			get
			{
				return "anti fiddler";
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060004D9 RID: 1241 RVA: 0x00003A82 File Offset: 0x00001C82
		public override string FullId
		{
			get
			{
				return "Ki.Fiddler";
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00003A89 File Offset: 0x00001C89
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiFiddlerProtection.AntiFiddlerPhase(this));
		}

		// Token: 0x04000269 RID: 617
		public const string _Id = "anti fiddler";

		// Token: 0x0400026A RID: 618
		public const string _FullId = "Ki.Fiddler";

		// Token: 0x0200011F RID: 287
		private class AntiFiddlerPhase : ProtectionPhase
		{
			// Token: 0x060004DE RID: 1246 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiFiddlerPhase(AntiFiddlerProtection parent) : base(parent)
			{
			}

			// Token: 0x17000149 RID: 329
			// (get) Token: 0x060004DF RID: 1247 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700014A RID: 330
			// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00003A98 File Offset: 0x00001C98
			public override string Name
			{
				get
				{
					return "Anti Fiddler";
				}
			}

			// Token: 0x060004E1 RID: 1249 RVA: 0x0001E244 File Offset: 0x0001C444
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiFiddler");
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
