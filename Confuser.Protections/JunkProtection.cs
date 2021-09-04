using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x0200002A RID: 42
	public class JunkProtection : Protection
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x0000257A File Offset: 0x0000077A
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.None;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060000DA RID: 218 RVA: 0x0000257D File Offset: 0x0000077D
		public override string Name
		{
			get
			{
				return "Junk Methods";
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00002584 File Offset: 0x00000784
		public override string Description
		{
			get
			{
				return "Adds useless stuff to the module.";
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060000DC RID: 220 RVA: 0x00002105 File Offset: 0x00000305
		public override string Author
		{
			get
			{
				return "Wadu";
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000258B File Offset: 0x0000078B
		public override string Id
		{
			get
			{
				return "junk";
			}
		}

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00002592 File Offset: 0x00000792
		public override string FullId
		{
			get
			{
				return "Wadu.Junk";
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00002599 File Offset: 0x00000799
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new JunkProtection.JunkAdditionPhase(this));
		}

		// Token: 0x0200002B RID: 43
		private class JunkAdditionPhase : ProtectionPhase
		{
			// Token: 0x060000E2 RID: 226 RVA: 0x00002136 File Offset: 0x00000336
			public JunkAdditionPhase(JunkProtection parent) : base(parent)
			{
			}

			// Token: 0x17000068 RID: 104
			// (get) Token: 0x060000E3 RID: 227 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000069 RID: 105
			// (get) Token: 0x060000E4 RID: 228 RVA: 0x000025A9 File Offset: 0x000007A9
			public override string Name
			{
				get
				{
					return "Junk Addition";
				}
			}

			// Token: 0x060000E5 RID: 229 RVA: 0x00006540 File Offset: 0x00004740
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				INameService service = context.Registry.GetService<INameService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator(base.Parent.FullId);
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					InterfaceImplUser item = new InterfaceImplUser(moduleDef.GlobalType);
					for (int i = 0; i < randomGenerator.NextInt32(20, 50); i++)
					{
						TypeDefUser typeDefUser = new TypeDefUser(moduleDef.GlobalType.Namespace, service.RandomName(), moduleDef.CorLibTypes.GetTypeRef("System", "Attribute"))
						{
							Attributes = TypeAttributes.Public
						};
						typeDefUser.Interfaces.Add(item);
						for (int j = 0; j < randomGenerator.NextInt32(20, 25); j++)
						{
							MethodDefUser methodDefUser = new MethodDefUser(service.RandomName(RenameMode.MSCorLib), MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static);
							methodDefUser.Body = new CilBody();
							methodDefUser.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
							typeDefUser.Methods.Add(methodDefUser);
						}
						moduleDef.Types.Add(typeDefUser);
						service2.Mark(typeDefUser, base.Parent);
						service.SetCanRename(typeDefUser, false);
						foreach (MethodDef methodDef in typeDefUser.Methods)
						{
							service2.Mark(methodDef, base.Parent);
							service.SetCanRename(methodDef, false);
						}
					}
				}
			}
		}
	}
}
