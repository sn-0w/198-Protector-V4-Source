using System;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections
{
	// Token: 0x02000028 RID: 40
	internal class InvalidMetadataProtection : Protection
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00002538 File Offset: 0x00000738
		public override string Name
		{
			get
			{
				return "Invalid Metadata Protection";
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000CB RID: 203 RVA: 0x0000253F File Offset: 0x0000073F
		public override string Description
		{
			get
			{
				return "This protection adds invalid metadata to modules to prevent disassembler/decompiler from opening them.";
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000CC RID: 204 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00002546 File Offset: 0x00000746
		public override string Id
		{
			get
			{
				return "invalid metadata";
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000254D File Offset: 0x0000074D
		public override string FullId
		{
			get
			{
				return "Ki.InvalidMD";
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000CF RID: 207 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Aggressive;
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00002554 File Offset: 0x00000754
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.BeginModule, new InvalidMetadataProtection.InvalidMDPhase(this));
		}

		// Token: 0x04000035 RID: 53
		public const string _FullId = "Ki.InvalidMD";

		// Token: 0x02000029 RID: 41
		private class InvalidMDPhase : ProtectionPhase
		{
			// Token: 0x060000D3 RID: 211 RVA: 0x00002136 File Offset: 0x00000336
			public InvalidMDPhase(InvalidMetadataProtection parent) : base(parent)
			{
			}

			// Token: 0x17000060 RID: 96
			// (get) Token: 0x060000D4 RID: 212 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000061 RID: 97
			// (get) Token: 0x060000D5 RID: 213 RVA: 0x00002564 File Offset: 0x00000764
			public override string Name
			{
				get
				{
					return "Invalid metadata addition";
				}
			}

			// Token: 0x060000D6 RID: 214 RVA: 0x0000627C File Offset: 0x0000447C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				bool flag = parameters.Targets.Contains(context.CurrentModule);
				if (flag)
				{
					this.random = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.InvalidMD");
					context.CurrentModuleWriterOptions.WriterEvent += this.OnWriterEvent;
				}
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x0000256B File Offset: 0x0000076B
			private void Randomize<T>(MDTable<T> table) where T : struct
			{
				this.random.Shuffle<T>(table);
			}

			// Token: 0x060000D8 RID: 216 RVA: 0x000062D4 File Offset: 0x000044D4
			private void OnWriterEvent(object sender, ModuleWriterEventArgs e)
			{
				ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
				bool flag = e.Event == ModuleWriterEvent.MDEndCreateTables;
				if (flag)
				{
					moduleWriterBase.Metadata.TablesHeap.ModuleTable.Add(new RawModuleRow(0, 2147450879U, 0U, 0U, 0U));
					moduleWriterBase.Metadata.TablesHeap.AssemblyTable.Add(new RawAssemblyRow(0U, 0, 0, 0, 0, 0U, 0U, 2147450879U, 0U));
					int num = this.random.NextInt32(8, 16);
					for (int i = 0; i < num; i++)
					{
						moduleWriterBase.Metadata.TablesHeap.ENCLogTable.Add(new RawENCLogRow(this.random.NextUInt32(), this.random.NextUInt32()));
					}
					num = this.random.NextInt32(8, 16);
					for (int j = 0; j < num; j++)
					{
						moduleWriterBase.Metadata.TablesHeap.ENCMapTable.Add(new RawENCMapRow(this.random.NextUInt32()));
					}
					this.Randomize<RawManifestResourceRow>(moduleWriterBase.Metadata.TablesHeap.ManifestResourceTable);
					moduleWriterBase.TheOptions.MetadataOptions.TablesHeapOptions.ExtraData = new uint?(this.random.NextUInt32());
					moduleWriterBase.TheOptions.MetadataOptions.TablesHeapOptions.UseENC = new bool?(false);
					MetadataHeaderOptions metadataHeaderOptions = moduleWriterBase.TheOptions.MetadataOptions.MetadataHeaderOptions;
					metadataHeaderOptions.VersionString += "\0\0\0\0";
					moduleWriterBase.TheOptions.MetadataOptions.CustomHeaps.Add(new DnlibUtils.RawHeap("#GUID", Guid.NewGuid().ToByteArray()));
					moduleWriterBase.TheOptions.MetadataOptions.CustomHeaps.Add(new DnlibUtils.RawHeap("#Strings", new byte[1]));
					moduleWriterBase.TheOptions.MetadataOptions.CustomHeaps.Add(new DnlibUtils.RawHeap("#Blob", new byte[1]));
					moduleWriterBase.TheOptions.MetadataOptions.CustomHeaps.Add(new DnlibUtils.RawHeap("#Schema", new byte[1]));
				}
				else
				{
					bool flag2 = e.Event == ModuleWriterEvent.MDOnAllTablesSorted;
					if (flag2)
					{
						moduleWriterBase.Metadata.TablesHeap.DeclSecurityTable.Add(new RawDeclSecurityRow(short.MaxValue, 4294934527U, 4294934527U));
					}
				}
			}

			// Token: 0x04000036 RID: 54
			private RandomGenerator random;
		}
	}
}
