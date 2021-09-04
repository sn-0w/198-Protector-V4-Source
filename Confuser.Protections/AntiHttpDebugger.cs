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
	// Token: 0x02000121 RID: 289
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow",
		"Ki.Constants",
		"Ki.ForceElevation"
	})]
	internal class AntiHttpDebugger : Protection
	{
		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x00003AAB File Offset: 0x00001CAB
		public override string Name
		{
			get
			{
				return "Anti Http Debugger Protection";
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060004E7 RID: 1255 RVA: 0x00003AB2 File Offset: 0x00001CB2
		public override string Description
		{
			get
			{
				return "This protection will remove the program if the user have http debugger on his computer.";
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x00003AB9 File Offset: 0x00001CB9
		public override string Id
		{
			get
			{
				return "anti http debugger";
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00003AC0 File Offset: 0x00001CC0
		public override string FullId
		{
			get
			{
				return "Ki.HttpDebugger";
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00003AC7 File Offset: 0x00001CC7
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiHttpDebugger.AntiHttpDebuggerPhase(this));
		}

		// Token: 0x0400026D RID: 621
		public const string _Id = "anti http debugger";

		// Token: 0x0400026E RID: 622
		public const string _FullId = "Ki.HttpDebugger";

		// Token: 0x02000122 RID: 290
		private class AntiHttpDebuggerPhase : ProtectionPhase
		{
			// Token: 0x060004EE RID: 1262 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiHttpDebuggerPhase(AntiHttpDebugger parent) : base(parent)
			{
			}

			// Token: 0x17000151 RID: 337
			// (get) Token: 0x060004EF RID: 1263 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000152 RID: 338
			// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00003AD6 File Offset: 0x00001CD6
			public override string Name
			{
				get
				{
					return "Anti Http Debugger";
				}
			}

			// Token: 0x060004F1 RID: 1265 RVA: 0x0001E374 File Offset: 0x0001C574
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiHttpDebugger");
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
