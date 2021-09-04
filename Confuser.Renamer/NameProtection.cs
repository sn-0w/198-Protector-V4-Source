using System;
using System.Collections.Generic;
using System.IO;
using Confuser.Core;

namespace Confuser.Renamer
{
	// Token: 0x02000007 RID: 7
	internal class NameProtection : Protection
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000070B4 File Offset: 0x000052B4
		public override string Name
		{
			get
			{
				return "Name Protection";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000070CC File Offset: 0x000052CC
		public override string Description
		{
			get
			{
				return "This protection obfuscate the symbols' name so the decompiled source code can neither be compiled nor read.";
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000070E4 File Offset: 0x000052E4
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000070FC File Offset: 0x000052FC
		public override string Id
		{
			get
			{
				return "rename";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00007114 File Offset: 0x00005314
		public override string FullId
		{
			get
			{
				return "Ki.Rename";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000023 RID: 35 RVA: 0x0000712C File Offset: 0x0000532C
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000713F File Offset: 0x0000533F
		protected override void Initialize(ConfuserContext context)
		{
			context.Registry.RegisterService("Ki.Rename", typeof(INameService), new NameService(context));
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00007163 File Offset: 0x00005363
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPostStage(PipelineStage.Inspection, new AnalyzePhase(this));
			pipeline.InsertPostStage(PipelineStage.BeginModule, new RenamePhase(this));
			pipeline.InsertPreStage(PipelineStage.EndModule, new PostRenamePhase(this));
			pipeline.InsertPostStage(PipelineStage.SaveModules, new NameProtection.ExportMapPhase(this));
		}

		// Token: 0x04000006 RID: 6
		public const string _Id = "rename";

		// Token: 0x04000007 RID: 7
		public const string _FullId = "Ki.Rename";

		// Token: 0x04000008 RID: 8
		public const string _ServiceId = "Ki.Rename";

		// Token: 0x02000080 RID: 128
		private class ExportMapPhase : ProtectionPhase
		{
			// Token: 0x06000320 RID: 800 RVA: 0x00002050 File Offset: 0x00000250
			public ExportMapPhase(NameProtection parent) : base(parent)
			{
			}

			// Token: 0x170000BB RID: 187
			// (get) Token: 0x06000321 RID: 801 RVA: 0x0002A4D0 File Offset: 0x000286D0
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170000BC RID: 188
			// (get) Token: 0x06000322 RID: 802 RVA: 0x0002A4E4 File Offset: 0x000286E4
			public override string Name
			{
				get
				{
					return "Export symbol map";
				}
			}

			// Token: 0x170000BD RID: 189
			// (get) Token: 0x06000323 RID: 803 RVA: 0x0002A4FC File Offset: 0x000286FC
			public override bool ProcessAll
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06000324 RID: 804 RVA: 0x0002A510 File Offset: 0x00028710
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				NameService nameService = (NameService)context.Registry.GetService<INameService>();
				ICollection<KeyValuePair<string, string>> nameMap = nameService.GetNameMap();
				bool flag = nameMap.Count == 0;
				if (!flag)
				{
					string fullPath = Path.GetFullPath(Path.Combine(context.OutputDirectory, "symbols.map"));
					string directoryName = Path.GetDirectoryName(fullPath);
					bool flag2 = !Directory.Exists(directoryName);
					if (flag2)
					{
						Directory.CreateDirectory(directoryName);
					}
					using (StreamWriter streamWriter = new StreamWriter(File.OpenWrite(fullPath)))
					{
						foreach (KeyValuePair<string, string> keyValuePair in nameMap)
						{
							streamWriter.WriteLine("{0}\t{1}", keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
		}
	}
}
