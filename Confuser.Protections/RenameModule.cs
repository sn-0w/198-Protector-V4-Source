using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000146 RID: 326
	internal class RenameModule : Protection
	{
		// Token: 0x06000598 RID: 1432 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x00003DE9 File Offset: 0x00001FE9
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new RenameModule.AntiILDasmPhase(this));
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x00003D74 File Offset: 0x00001F74
		public override string Author
		{
			get
			{
				return "Aptitude";
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600059B RID: 1435 RVA: 0x00003DF8 File Offset: 0x00001FF8
		public override string Description
		{
			get
			{
				return "Renames the module and assembly.";
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x00003DFF File Offset: 0x00001FFF
		public override string FullId
		{
			get
			{
				return "Ki.RenameModule";
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x0600059D RID: 1437 RVA: 0x00003E06 File Offset: 0x00002006
		public override string Id
		{
			get
			{
				return "Rename Module";
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00003E06 File Offset: 0x00002006
		public override string Name
		{
			get
			{
				return "Rename Module";
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600059F RID: 1439 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0001FD74 File Offset: 0x0001DF74
		public static string Random(int len)
		{
			string text = "ﻖﻘﻕабцдефгчийклмнопярстуﻛﻗﻛﻯﻭﻄﻂﺳﺯﺯﻖﺳﻞﻀ﴾ﺄﺞﻂﻍ①②③⓱రఞభఔఙఇચઑઌઊઊﭺﭽﭯﭫﭖ";
			char[] array = new char[len - 1 + 1];
			int num = len - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = text[(int)Math.Round(Conversion.Int(VBMath.Rnd() * (float)text.Length))];
			}
			return new string(array);
		}

		// Token: 0x040002A4 RID: 676
		public const string _FullId = "Ki.RenameModule";

		// Token: 0x040002A5 RID: 677
		public const string _Id = "Junk";

		// Token: 0x02000147 RID: 327
		private class AntiILDasmPhase : ProtectionPhase
		{
			// Token: 0x060005A2 RID: 1442 RVA: 0x00003E0D File Offset: 0x0000200D
			public AntiILDasmPhase(RenameModule parent) : base(parent)
			{
			}

			// Token: 0x060005A3 RID: 1443 RVA: 0x0001FDD0 File Offset: 0x0001DFD0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					ModuleDefMD moduleDefMD = (ModuleDefMD)moduleDef;
					moduleDefMD.Name = ".иⓔ†ƒµƧʗⓐ†ѺяρяѺ†ⓔʗ†Ѻя_1939 - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
					moduleDefMD.Assembly.Name = ".иⓔ†ƒµƧʗⓐ†ѺяρяѺ†ⓔʗ†Ѻя_1945 - " + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString() + this.cheekycunt.Next(100000, 1000000000).ToString();
				}
			}

			// Token: 0x170001AD RID: 429
			// (get) Token: 0x060005A4 RID: 1444 RVA: 0x00003E21 File Offset: 0x00002021
			public override string Name
			{
				get
				{
					return "Renaming";
				}
			}

			// Token: 0x170001AE RID: 430
			// (get) Token: 0x060005A5 RID: 1445 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x040002A6 RID: 678
			private Random cheekycunt = new Random();
		}
	}
}
