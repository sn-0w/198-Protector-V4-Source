using System;
using dnlib.DotNet;

namespace Confuser.Core.Services
{
	// Token: 0x0200007C RID: 124
	public interface IMarkerService
	{
		// Token: 0x060002F7 RID: 759
		void Mark(IDnlibDef member, ConfuserComponent parentComp);

		// Token: 0x060002F8 RID: 760
		bool IsMarked(IDnlibDef def);

		// Token: 0x060002F9 RID: 761
		ConfuserComponent GetHelperParent(IDnlibDef def);
	}
}
