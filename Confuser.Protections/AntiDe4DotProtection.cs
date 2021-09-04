using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000002 RID: 2
	public class AntiDe4DotProtection : Protection
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x000020F7 File Offset: 0x000002F7
		public override string Name
		{
			get
			{
				return "Anti De4Dot Protection";
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020FE File Offset: 0x000002FE
		public override string Description
		{
			get
			{
				return "Prevents usage of De4Dot.";
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x0000210C File Offset: 0x0000030C
		public override string Id
		{
			get
			{
				return "anti de4dot";
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000006 RID: 6 RVA: 0x00002113 File Offset: 0x00000313
		public override string FullId
		{
			get
			{
				return "Wadu.AntiDe4Dot";
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000211D File Offset: 0x0000031D
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new AntiDe4DotProtection.AntiDe4DotPhase(this));
		}

		// Token: 0x02000003 RID: 3
		private class AntiDe4DotPhase : ProtectionPhase
		{
			// Token: 0x0600000A RID: 10 RVA: 0x00002136 File Offset: 0x00000336
			public AntiDe4DotPhase(AntiDe4DotProtection parent) : base(parent)
			{
			}

			// Token: 0x17000007 RID: 7
			// (get) Token: 0x0600000B RID: 11 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000008 RID: 8
			// (get) Token: 0x0600000C RID: 12 RVA: 0x00002145 File Offset: 0x00000345
			public override string Name
			{
				get
				{
					return "Anti De4Dot";
				}
			}

			// Token: 0x0600000D RID: 13 RVA: 0x00003E54 File Offset: 0x00002054
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId);
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					InterfaceImpl item = new InterfaceImplUser(moduleDef.GlobalType);
					TypeDef typeDef = new TypeDefUser("", service2.RandomName(), moduleDef.CorLibTypes.GetTypeRef("System", "Attribute"));
					InterfaceImpl item2 = new InterfaceImplUser(typeDef);
					moduleDef.Types.Add(typeDef);
					typeDef.Interfaces.Add(item2);
					typeDef.Interfaces.Add(item);
					service.Mark(typeDef, base.Parent);
					service2.SetCanRename(typeDef, false);
					for (int i = 0; i < randomGenerator.NextInt32(4, 15); i++)
					{
						TypeDef typeDef2 = new TypeDefUser("", service2.RandomName(), moduleDef.CorLibTypes.GetTypeRef("System", "Attribute"));
						InterfaceImpl item3 = new InterfaceImplUser(typeDef2);
						moduleDef.Types.Add(typeDef2);
						typeDef2.Interfaces.Add(item3);
						typeDef2.Interfaces.Add(item);
						typeDef2.Interfaces.Add(item2);
						service.Mark(typeDef2, base.Parent);
						service2.SetCanRename(typeDef2, false);
					}
				}
			}
		}
	}
}
