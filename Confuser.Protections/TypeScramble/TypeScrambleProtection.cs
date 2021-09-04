using System;
using Confuser.Core;

namespace Confuser.Protections.TypeScramble
{
	// Token: 0x02000036 RID: 54
	internal class TypeScrambleProtection : Protection
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0000257A File Offset: 0x0000077A
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.None;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00002726 File Offset: 0x00000926
		public override string Name
		{
			get
			{
				return "Type Scrambler";
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600012C RID: 300 RVA: 0x0000272D File Offset: 0x0000092D
		public override string Description
		{
			get
			{
				return "Replaces types with generics";
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00002734 File Offset: 0x00000934
		public override string Id
		{
			get
			{
				return "typescrambler";
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000273B File Offset: 0x0000093B
		public override string FullId
		{
			get
			{
				return "Holly.TypeScrambler";
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600012F RID: 303 RVA: 0x00002742 File Offset: 0x00000942
		public override string Author
		{
			get
			{
				return "Holly";
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00006DB4 File Offset: 0x00004FB4
		protected override void Initialize(ConfuserContext context)
		{
			bool flag = context == null;
			if (flag)
			{
				throw new ArgumentNullException("context");
			}
			context.Registry.RegisterService(this.FullId, typeof(TypeService), new TypeService());
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00006DF8 File Offset: 0x00004FF8
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			bool flag = pipeline == null;
			if (flag)
			{
				throw new ArgumentNullException("pipeline");
			}
			pipeline.InsertPreStage(PipelineStage.Inspection, new AnalyzePhase(this));
			pipeline.InsertPostStage(PipelineStage.ProcessModule, new ScramblePhase(this));
		}
	}
}
