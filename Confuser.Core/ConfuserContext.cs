using System;
using System.Collections.Generic;
using System.Threading;
using Confuser.Core.Project;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Core
{
	// Token: 0x02000068 RID: 104
	public class ConfuserContext : IDisposable
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600026E RID: 622 RVA: 0x0000307D File Offset: 0x0000127D
		// (set) Token: 0x0600026F RID: 623 RVA: 0x00003085 File Offset: 0x00001285
		public ILogger Logger { get; internal set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0000308E File Offset: 0x0000128E
		// (set) Token: 0x06000271 RID: 625 RVA: 0x00003096 File Offset: 0x00001296
		public ConfuserProject Project { get; internal set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000309F File Offset: 0x0000129F
		// (set) Token: 0x06000273 RID: 627 RVA: 0x000030A7 File Offset: 0x000012A7
		internal bool PackerInitiated { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000274 RID: 628 RVA: 0x00011728 File Offset: 0x0000F928
		public Annotations Annotations
		{
			get
			{
				return this.annotations;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000275 RID: 629 RVA: 0x00011740 File Offset: 0x0000F940
		public ServiceRegistry Registry
		{
			get
			{
				return this.registry;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000276 RID: 630 RVA: 0x000030B0 File Offset: 0x000012B0
		// (set) Token: 0x06000277 RID: 631 RVA: 0x000030B8 File Offset: 0x000012B8
		public AssemblyResolver Resolver { get; internal set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000278 RID: 632 RVA: 0x000030C1 File Offset: 0x000012C1
		// (set) Token: 0x06000279 RID: 633 RVA: 0x000030C9 File Offset: 0x000012C9
		public IList<ModuleDefMD> Modules { get; internal set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600027A RID: 634 RVA: 0x000030D2 File Offset: 0x000012D2
		// (set) Token: 0x0600027B RID: 635 RVA: 0x000030DA File Offset: 0x000012DA
		public IList<byte[]> ExternalModules { get; internal set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600027C RID: 636 RVA: 0x000030E3 File Offset: 0x000012E3
		// (set) Token: 0x0600027D RID: 637 RVA: 0x000030EB File Offset: 0x000012EB
		public string BaseDirectory { get; internal set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600027E RID: 638 RVA: 0x000030F4 File Offset: 0x000012F4
		// (set) Token: 0x0600027F RID: 639 RVA: 0x000030FC File Offset: 0x000012FC
		public string OutputDirectory { get; internal set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000280 RID: 640 RVA: 0x00003105 File Offset: 0x00001305
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0000310D File Offset: 0x0000130D
		public string InputSymbolMap { get; internal set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000282 RID: 642 RVA: 0x00003116 File Offset: 0x00001316
		// (set) Token: 0x06000283 RID: 643 RVA: 0x0000311E File Offset: 0x0000131E
		public Packer Packer { get; internal set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000284 RID: 644 RVA: 0x00003127 File Offset: 0x00001327
		// (set) Token: 0x06000285 RID: 645 RVA: 0x0000312F File Offset: 0x0000132F
		public ProtectionPipeline Pipeline { get; internal set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00003138 File Offset: 0x00001338
		// (set) Token: 0x06000287 RID: 647 RVA: 0x00003140 File Offset: 0x00001340
		public IList<byte[]> OutputModules { get; internal set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00003149 File Offset: 0x00001349
		// (set) Token: 0x06000289 RID: 649 RVA: 0x00003151 File Offset: 0x00001351
		public IList<byte[]> OutputSymbols { get; internal set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000315A File Offset: 0x0000135A
		// (set) Token: 0x0600028B RID: 651 RVA: 0x00003162 File Offset: 0x00001362
		public IList<string> OutputPaths { get; internal set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000316B File Offset: 0x0000136B
		// (set) Token: 0x0600028D RID: 653 RVA: 0x00003173 File Offset: 0x00001373
		public int CurrentModuleIndex { get; internal set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600028E RID: 654 RVA: 0x00011758 File Offset: 0x0000F958
		public ModuleDefMD CurrentModule
		{
			get
			{
				return (this.CurrentModuleIndex == -1) ? null : this.Modules[this.CurrentModuleIndex];
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000317C File Offset: 0x0000137C
		// (set) Token: 0x06000290 RID: 656 RVA: 0x00003184 File Offset: 0x00001384
		public ModuleWriterOptionsBase CurrentModuleWriterOptions { get; internal set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000291 RID: 657 RVA: 0x0000318D File Offset: 0x0000138D
		// (set) Token: 0x06000292 RID: 658 RVA: 0x00003195 File Offset: 0x00001395
		public byte[] CurrentModuleOutput { get; internal set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000293 RID: 659 RVA: 0x0000319E File Offset: 0x0000139E
		// (set) Token: 0x06000294 RID: 660 RVA: 0x000031A6 File Offset: 0x000013A6
		public byte[] CurrentModuleSymbol { get; internal set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000295 RID: 661 RVA: 0x00011788 File Offset: 0x0000F988
		public CancellationToken CancellationToken
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000031AF File Offset: 0x000013AF
		public void CheckCancellation()
		{
			this.token.ThrowIfCancellationRequested();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000117A0 File Offset: 0x0000F9A0
		public NativeModuleWriterOptions RequestNative()
		{
			bool flag = this.CurrentModule == null;
			NativeModuleWriterOptions result;
			if (flag)
			{
				result = null;
			}
			else
			{
				bool flag2 = this.CurrentModuleWriterOptions == null;
				if (flag2)
				{
					this.CurrentModuleWriterOptions = new NativeModuleWriterOptions(this.CurrentModule, true);
				}
				bool flag3 = this.CurrentModuleWriterOptions is NativeModuleWriterOptions;
				if (flag3)
				{
					result = (NativeModuleWriterOptions)this.CurrentModuleWriterOptions;
				}
				else
				{
					NativeModuleWriterOptions nativeModuleWriterOptions = new NativeModuleWriterOptions(this.CurrentModule, true)
					{
						AddCheckSum = this.CurrentModuleWriterOptions.AddCheckSum,
						Cor20HeaderOptions = this.CurrentModuleWriterOptions.Cor20HeaderOptions,
						Logger = this.CurrentModuleWriterOptions.Logger,
						MetadataLogger = this.CurrentModuleWriterOptions.MetadataLogger,
						MetadataOptions = this.CurrentModuleWriterOptions.MetadataOptions,
						ModuleKind = this.CurrentModuleWriterOptions.ModuleKind,
						PEHeadersOptions = this.CurrentModuleWriterOptions.PEHeadersOptions,
						ShareMethodBodies = this.CurrentModuleWriterOptions.ShareMethodBodies,
						StrongNameKey = this.CurrentModuleWriterOptions.StrongNameKey,
						StrongNamePublicKey = this.CurrentModuleWriterOptions.StrongNamePublicKey,
						Win32Resources = this.CurrentModuleWriterOptions.Win32Resources
					};
					this.CurrentModuleWriterOptions = nativeModuleWriterOptions;
					result = nativeModuleWriterOptions;
				}
			}
			return result;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000118E8 File Offset: 0x0000FAE8
		private void Dispose(bool disposing)
		{
			bool flag = !disposing;
			if (!flag)
			{
				bool flag2 = this.Modules != null;
				if (flag2)
				{
					foreach (ModuleDefMD moduleDefMD in this.Modules)
					{
						moduleDefMD.Dispose();
					}
				}
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x000031BE File Offset: 0x000013BE
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x00011950 File Offset: 0x0000FB50
		~ConfuserContext()
		{
			this.Dispose(false);
		}

		// Token: 0x040001DE RID: 478
		private readonly Annotations annotations = new Annotations();

		// Token: 0x040001DF RID: 479
		private readonly ServiceRegistry registry = new ServiceRegistry();

		// Token: 0x040001E0 RID: 480
		internal CancellationToken token;
	}
}
