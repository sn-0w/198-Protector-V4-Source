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
	// Token: 0x0200012A RID: 298
	internal class AntiDnSpyProtection : Protection
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x00003BC1 File Offset: 0x00001DC1
		public override string Name
		{
			get
			{
				return "Anti DnSpy Protection";
			}
		}

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x00003BC8 File Offset: 0x00001DC8
		public override string Description
		{
			get
			{
				return "This protection prevents dnspy.";
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x00003BCF File Offset: 0x00001DCF
		public override string Id
		{
			get
			{
				return "anti dnspy";
			}
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x00003BD6 File Offset: 0x00001DD6
		public override string FullId
		{
			get
			{
				return "Ki.AntiDnSpy";
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x00003BDD File Offset: 0x00001DDD
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDnSpyProtection.AntiDnSpyPhase(this));
		}

		// Token: 0x04000277 RID: 631
		public const string _Id = "anti dnspy";

		// Token: 0x04000278 RID: 632
		public const string _FullId = "Ki.AntiDnSpy";

		// Token: 0x0200012B RID: 299
		private class AntiDnSpyPhase : ProtectionPhase
		{
			// Token: 0x06000520 RID: 1312 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiDnSpyPhase(AntiDnSpyProtection parent) : base(parent)
			{
			}

			// Token: 0x17000169 RID: 361
			// (get) Token: 0x06000521 RID: 1313 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700016A RID: 362
			// (get) Token: 0x06000522 RID: 1314 RVA: 0x00003BEC File Offset: 0x00001DEC
			public override string Name
			{
				get
				{
					return "Anti-dnspy injection";
				}
			}

			// Token: 0x06000523 RID: 1315 RVA: 0x0001E8F8 File Offset: 0x0001CAF8
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.AntiDnspy");
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
