using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007B RID: 123
	internal class MarkerService : IMarkerService
	{
		// Token: 0x060002F3 RID: 755 RVA: 0x0000343B File Offset: 0x0000163B
		public MarkerService(ConfuserContext context, Marker marker)
		{
			this.context = context;
			this.marker = marker;
			this.helperParents = new Dictionary<IDnlibDef, ConfuserComponent>();
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x000133BC File Offset: 0x000115BC
		public void Mark(IDnlibDef member, ConfuserComponent parentComp)
		{
			bool flag = member == null;
			if (flag)
			{
				throw new ArgumentNullException("member");
			}
			bool flag2 = member is ModuleDef;
			if (flag2)
			{
				throw new ArgumentException("New ModuleDef cannot be marked.");
			}
			bool flag3 = this.IsMarked(member);
			if (!flag3)
			{
				this.marker.MarkMember(member, this.context);
				bool flag4 = parentComp != null;
				if (flag4)
				{
					this.helperParents[member] = parentComp;
				}
			}
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0001342C File Offset: 0x0001162C
		public bool IsMarked(IDnlibDef def)
		{
			return ProtectionParameters.GetParameters(this.context, def) != null;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00013450 File Offset: 0x00011650
		public ConfuserComponent GetHelperParent(IDnlibDef def)
		{
			ConfuserComponent confuserComponent;
			bool flag = !this.helperParents.TryGetValue(def, out confuserComponent);
			ConfuserComponent result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = confuserComponent;
			}
			return result;
		}

		// Token: 0x04000231 RID: 561
		private readonly ConfuserContext context;

		// Token: 0x04000232 RID: 562
		private readonly Marker marker;

		// Token: 0x04000233 RID: 563
		private readonly Dictionary<IDnlibDef, ConfuserComponent> helperParents;
	}
}
