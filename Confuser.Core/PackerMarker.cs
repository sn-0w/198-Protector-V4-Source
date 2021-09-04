using System;
using Confuser.Core.Project;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x02000060 RID: 96
	internal class PackerMarker : Marker
	{
		// Token: 0x06000258 RID: 600 RVA: 0x00002FCC File Offset: 0x000011CC
		public PackerMarker(StrongNameKey snKey)
		{
			this.snKey = snKey;
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00011610 File Offset: 0x0000F810
		protected internal override MarkerResult MarkProject(ConfuserProject proj, ConfuserContext context)
		{
			MarkerResult markerResult = base.MarkProject(proj, context);
			foreach (ModuleDefMD obj in markerResult.Modules)
			{
				context.Annotations.Set<StrongNameKey>(obj, Marker.SNKey, this.snKey);
			}
			return markerResult;
		}

		// Token: 0x040001D0 RID: 464
		private readonly StrongNameKey snKey;
	}
}
