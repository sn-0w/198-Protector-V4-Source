using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x0200002C RID: 44
	public class ModulePropertiesProtection : Protection
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x000025B3 File Offset: 0x000007B3
		public override string Name
		{
			get
			{
				return "Module Properties Protection";
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x000025BA File Offset: 0x000007BA
		public override string Description
		{
			get
			{
				return "Changes the properties of the Module.";
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060000EA RID: 234 RVA: 0x000025C1 File Offset: 0x000007C1
		public override string Id
		{
			get
			{
				return "module properties";
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060000EB RID: 235 RVA: 0x000025C8 File Offset: 0x000007C8
		public override string FullId
		{
			get
			{
				return "Wadu.ModuleProperties";
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000025CF File Offset: 0x000007CF
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new ModulePropertiesProtection.ModulePropertiesPhase(this));
		}

		// Token: 0x0200002D RID: 45
		private class ModulePropertiesPhase : ProtectionPhase
		{
			// Token: 0x060000EF RID: 239 RVA: 0x00002136 File Offset: 0x00000336
			public ModulePropertiesPhase(ModulePropertiesProtection parent) : base(parent)
			{
			}

			// Token: 0x17000070 RID: 112
			// (get) Token: 0x060000F0 RID: 240 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000071 RID: 113
			// (get) Token: 0x060000F1 RID: 241 RVA: 0x000025DF File Offset: 0x000007DF
			public override string Name
			{
				get
				{
					return "Module Properties";
				}
			}

			// Token: 0x060000F2 RID: 242 RVA: 0x00006768 File Offset: 0x00004968
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				INameService service = context.Registry.GetService<INameService>();
				RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId);
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					bool flag = false;
					foreach (AssemblyRef assemblyRef in moduleDef.GetAssemblyRefs())
					{
						bool flag2 = assemblyRef.Name == "WindowsBase" || assemblyRef.Name == "PresentationCore" || assemblyRef.Name == "PresentationFramework" || assemblyRef.Name == "System.Xaml";
						if (flag2)
						{
							flag = true;
						}
					}
					bool flag3 = !flag;
					if (flag3)
					{
						moduleDef.Name = service.RandomName(RenameMode.ASCII);
						moduleDef.Mvid = new Guid?(Guid.NewGuid());
						moduleDef.Assembly.CustomAttributes.Clear();
						moduleDef.Assembly.Name = service.RandomName();
						moduleDef.Assembly.Version = new Version(randomGenerator.NextInt32(1, 9), randomGenerator.NextInt32(1, 9), randomGenerator.NextInt32(1, 9), randomGenerator.NextInt32(1, 9));
					}
					else
					{
						context.Logger.WarnFormat("Module Properties is not compatible with {0}.", new object[]
						{
							moduleDef.Name
						});
					}
				}
			}
		}
	}
}
