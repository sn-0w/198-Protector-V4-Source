using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000135 RID: 309
	internal class AntiUnsafeValue : Protection
	{
		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x00003D04 File Offset: 0x00001F04
		public override string Name
		{
			get
			{
				return "Anti UnsafeValue Protection";
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000558 RID: 1368 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000559 RID: 1369 RVA: 0x00003D0B File Offset: 0x00001F0B
		public override string Description
		{
			get
			{
				return "Specifies that a type contains an unmanaged array that might potentially overflow. This class cannot be inherited.";
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x00003D12 File Offset: 0x00001F12
		public override string Id
		{
			get
			{
				return "anti unsafe";
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x00003D19 File Offset: 0x00001F19
		public override string FullId
		{
			get
			{
				return "Ki.Antiunsafe";
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00003D20 File Offset: 0x00001F20
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiUnsafeValue.AntiUnsafe(this));
		}

		// Token: 0x04000286 RID: 646
		public const string _Id = "anti unsafe";

		// Token: 0x04000287 RID: 647
		public const string _FullId = "Ki.Antiunsafe";

		// Token: 0x02000136 RID: 310
		private class AntiUnsafe : ProtectionPhase
		{
			// Token: 0x06000560 RID: 1376 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiUnsafe(AntiUnsafeValue parent) : base(parent)
			{
			}

			// Token: 0x1700018D RID: 397
			// (get) Token: 0x06000561 RID: 1377 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06000562 RID: 1378 RVA: 0x00003D2F File Offset: 0x00001F2F
			public override string Name
			{
				get
				{
					return "Anti-UnsafeValue marking";
				}
			}

			// Token: 0x06000563 RID: 1379 RVA: 0x0001F39C File Offset: 0x0001D59C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					TypeRef typeRef = moduleDef.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "UnsafeValueTypeAttribute");
					CustomAttribute item = new CustomAttribute(new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef));
					moduleDef.CustomAttributes.Add(item);
				}
			}
		}
	}
}
