using System;
using System.Collections.Generic;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000C6 RID: 198
	internal class ExtractPhase : ProtectionPhase
	{
		// Token: 0x0600034A RID: 842 RVA: 0x00002136 File Offset: 0x00000336
		public ExtractPhase(Compressor parent) : base(parent)
		{
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x0600034B RID: 843 RVA: 0x000040A4 File Offset: 0x000022A4
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Modules;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x0600034C RID: 844 RVA: 0x00017CFC File Offset: 0x00015EFC
		public override string Name
		{
			get
			{
				return "Packer info extraction";
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00017D14 File Offset: 0x00015F14
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = context.Packer == null;
			if (!flag)
			{
				bool flag2 = context.CurrentModule.Kind == ModuleKind.Windows || context.CurrentModule.Kind == ModuleKind.Console;
				bool flag3 = context.Annotations.Get<CompressorContext>(context, Compressor.ContextKey, null) != null;
				if (flag3)
				{
					bool flag4 = flag2;
					if (flag4)
					{
						context.Logger.Error("Too many executable modules!");
						throw new ConfuserException(null);
					}
				}
				else
				{
					bool flag5 = flag2;
					if (flag5)
					{
						CompressorContext compressorContext = new CompressorContext
						{
							ModuleIndex = context.CurrentModuleIndex,
							Assembly = context.CurrentModule.Assembly,
							CompatMode = parameters.GetParameter<bool>(context, null, "compat", false)
						};
						context.Annotations.Set<CompressorContext>(context, Compressor.ContextKey, compressorContext);
						compressorContext.ModuleName = context.CurrentModule.Name;
						compressorContext.EntryPoint = context.CurrentModule.EntryPoint;
						compressorContext.Kind = context.CurrentModule.Kind;
						bool flag6 = !compressorContext.CompatMode;
						if (flag6)
						{
							context.CurrentModule.Name = "What_A_Fucking_Joke_Am_I_Right";
							context.CurrentModule.EntryPoint = null;
							context.CurrentModule.Kind = ModuleKind.NetModule;
						}
						context.CurrentModuleWriterOptions.WriterEvent += new ExtractPhase.ResourceRecorder(compressorContext, context.CurrentModule).WriterEvent;
					}
				}
			}
		}

		// Token: 0x020000C7 RID: 199
		private class ResourceRecorder
		{
			// Token: 0x0600034E RID: 846 RVA: 0x00003212 File Offset: 0x00001412
			public ResourceRecorder(CompressorContext ctx, ModuleDef module)
			{
				this.ctx = ctx;
				this.targetModule = module;
			}

			// Token: 0x0600034F RID: 847 RVA: 0x00017E8C File Offset: 0x0001608C
			public void WriterEvent(object sender, ModuleWriterEventArgs e)
			{
				bool flag = e.Event == ModuleWriterEvent.MDEndAddResources;
				if (flag)
				{
					ModuleWriterBase writer = e.Writer;
					this.ctx.ManifestResources = new List<ValueTuple<uint, uint, UTF8String>>();
					foreach (Resource resource in writer.Module.Resources)
					{
						uint manifestResourceRid = writer.Metadata.GetManifestResourceRid(resource);
						bool flag2 = manifestResourceRid > 0U;
						if (flag2)
						{
							RawManifestResourceRow rawManifestResourceRow = writer.Metadata.TablesHeap.ManifestResourceTable[manifestResourceRid];
							this.ctx.ManifestResources.Add(new ValueTuple<uint, uint, UTF8String>(rawManifestResourceRow.Offset, rawManifestResourceRow.Flags, resource.Name));
						}
					}
					this.ctx.EntryPointToken = writer.Metadata.GetToken(this.ctx.EntryPoint).Raw;
				}
			}

			// Token: 0x04000199 RID: 409
			private readonly CompressorContext ctx;

			// Token: 0x0400019A RID: 410
			private ModuleDef targetModule;
		}
	}
}
