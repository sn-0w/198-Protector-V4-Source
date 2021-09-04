using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Confuser.Core;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000CC RID: 204
	internal class StubProtection : Protection
	{
		// Token: 0x0600035F RID: 863 RVA: 0x000032BB File Offset: 0x000014BB
		internal StubProtection(CompressorContext ctx, ModuleDef originModule)
		{
			this.ctx = ctx;
			this.originModule = originModule;
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000360 RID: 864 RVA: 0x00018534 File Offset: 0x00016734
		public override string Name
		{
			get
			{
				return "Compressor Stub Protection";
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000361 RID: 865 RVA: 0x0001854C File Offset: 0x0001674C
		public override string Description
		{
			get
			{
				return "Do some extra works on the protected stub.";
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000363 RID: 867 RVA: 0x00018564 File Offset: 0x00016764
		public override string Id
		{
			get
			{
				return "Ki.Compressor.Protection";
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000364 RID: 868 RVA: 0x00018564 File Offset: 0x00016764
		public override string FullId
		{
			get
			{
				return "Ki.Compressor.Protection";
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0001857C File Offset: 0x0001677C
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.None;
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00018590 File Offset: 0x00016790
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			bool flag = !this.ctx.CompatMode;
			if (flag)
			{
				pipeline.InsertPreStage(PipelineStage.Inspection, new StubProtection.InjPhase(this));
			}
			pipeline.InsertPostStage(PipelineStage.BeginModule, new StubProtection.SigPhase(this));
		}

		// Token: 0x040001B2 RID: 434
		private readonly CompressorContext ctx;

		// Token: 0x040001B3 RID: 435
		private readonly ModuleDef originModule;

		// Token: 0x020000CD RID: 205
		private class InjPhase : ProtectionPhase
		{
			// Token: 0x06000368 RID: 872 RVA: 0x00002136 File Offset: 0x00000336
			public InjPhase(StubProtection parent) : base(parent)
			{
			}

			// Token: 0x170000E2 RID: 226
			// (get) Token: 0x06000369 RID: 873 RVA: 0x000040A4 File Offset: 0x000022A4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170000E3 RID: 227
			// (get) Token: 0x0600036A RID: 874 RVA: 0x000185CC File Offset: 0x000167CC
			public override bool ProcessAll
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170000E4 RID: 228
			// (get) Token: 0x0600036B RID: 875 RVA: 0x000185E0 File Offset: 0x000167E0
			public override string Name
			{
				get
				{
					return "Module injection";
				}
			}

			// Token: 0x0600036C RID: 876 RVA: 0x000185F8 File Offset: 0x000167F8
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				ModuleDef originModule = ((StubProtection)base.Parent).originModule;
				originModule.Assembly.Modules.Remove(originModule);
				context.Modules[0].Assembly.Modules.Add(((StubProtection)base.Parent).originModule);
			}
		}

		// Token: 0x020000CE RID: 206
		private class SigPhase : ProtectionPhase
		{
			// Token: 0x0600036D RID: 877 RVA: 0x00002136 File Offset: 0x00000336
			public SigPhase(StubProtection parent) : base(parent)
			{
			}

			// Token: 0x170000E5 RID: 229
			// (get) Token: 0x0600036E RID: 878 RVA: 0x000040A4 File Offset: 0x000022A4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170000E6 RID: 230
			// (get) Token: 0x0600036F RID: 879 RVA: 0x00018658 File Offset: 0x00016858
			public override string Name
			{
				get
				{
					return "Packer info encoding";
				}
			}

			// Token: 0x06000370 RID: 880 RVA: 0x00018670 File Offset: 0x00016870
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				FieldDef fieldDef = context.CurrentModule.Types[0].FindField("DataField");
				Debug.Assert(fieldDef != null);
				context.Registry.GetService<INameService>().SetCanRename(fieldDef, true);
				context.CurrentModuleWriterOptions.WriterEvent += delegate(object sender, ModuleWriterEventArgs e)
				{
					bool flag = e.Event == ModuleWriterEvent.MDBeginCreateTables;
					if (flag)
					{
						ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
						StubProtection stubProtection = (StubProtection)base.Parent;
						uint signature = moduleWriterBase.Metadata.BlobHeap.Add(stubProtection.ctx.KeySig);
						uint num = moduleWriterBase.Metadata.TablesHeap.StandAloneSigTable.Add(new RawStandAloneSigRow(signature));
						Debug.Assert((285212672U | num) == stubProtection.ctx.KeyToken);
						bool compatMode = stubProtection.ctx.CompatMode;
						if (!compatMode)
						{
							byte[] data = SHA1.Create().ComputeHash(stubProtection.ctx.OriginModule.ToArray<byte>());
							uint hashValue = moduleWriterBase.Metadata.BlobHeap.Add(data);
							MDTable<RawFileRow> fileTable = moduleWriterBase.Metadata.TablesHeap.FileTable;
							uint num2 = fileTable.Add(new RawFileRow(0U, moduleWriterBase.Metadata.StringsHeap.Add("What_A_Fucking_Joke_Am_I_Right"), hashValue));
						}
					}
				};
			}
		}
	}
}
