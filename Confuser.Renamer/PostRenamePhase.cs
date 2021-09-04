using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000005 RID: 5
	internal class PostRenamePhase : ProtectionPhase
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002050 File Offset: 0x00000250
		public PostRenamePhase(NameProtection parent) : base(parent)
		{
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00006FCC File Offset: 0x000051CC
		public override bool ProcessAll
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00006FE0 File Offset: 0x000051E0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.AllDefinitions;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00006FF4 File Offset: 0x000051F4
		public override string Name
		{
			get
			{
				return "Post-renaming";
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000700C File Offset: 0x0000520C
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			NameService nameService = (NameService)context.Registry.GetService<INameService>();
			foreach (IRenamer renamer in nameService.Renamers)
			{
				foreach (IDnlibDef def in parameters.Targets)
				{
					renamer.PostRename(context, nameService, parameters, def);
				}
				context.CheckCancellation();
			}
		}
	}
}
