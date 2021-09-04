using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Calli
{
	// Token: 0x020000D5 RID: 213
	internal class InjectPhase : ProtectionPhase
	{
		// Token: 0x0600038C RID: 908 RVA: 0x00002136 File Offset: 0x00000336
		public InjectPhase(CalliProtection parent) : base(parent)
		{
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600038D RID: 909 RVA: 0x00002141 File Offset: 0x00000341
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Modules;
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600038E RID: 910 RVA: 0x000033A0 File Offset: 0x000015A0
		public override string Name
		{
			get
			{
				return "Calli Helper Injection";
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00018DF4 File Offset: 0x00016FF4
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			IRuntimeService service = context.Registry.GetService<IRuntimeService>();
			IMarkerService service2 = context.Registry.GetService<IMarkerService>();
			INameService service3 = context.Registry.GetService<INameService>();
			CalliContext calliContext = new CalliContext();
			foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
			{
				CalliContext.CalliMode parameter = parameters.GetParameter<CalliContext.CalliMode>(context, moduleDef, "mode", CalliContext.CalliMode.Normal);
				bool flag = parameter == CalliContext.CalliMode.Normal;
				if (flag)
				{
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(service.GetRuntimeType("Confuser.Runtime.Calli"), moduleDef.GlobalType, moduleDef);
					calliContext.CalliMethod = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "ResolveToken");
					foreach (IDnlibDef def in enumerable)
					{
						service3.MarkHelper(def, service2, (Protection)base.Parent);
					}
					context.CurrentModuleWriterOptions.MetadataOptions.Flags |= MetadataFlags.PreserveAllMethodRids;
				}
				context.Annotations.Set<CalliContext>(context.CurrentModule, CalliProtection.ContextKey, calliContext);
			}
		}
	}
}
