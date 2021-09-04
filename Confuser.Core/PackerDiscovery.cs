using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x02000061 RID: 97
	internal class PackerDiscovery : PluginDiscovery
	{
		// Token: 0x0600025A RID: 602 RVA: 0x00002FDD File Offset: 0x000011DD
		public PackerDiscovery(Protection prot)
		{
			this.prot = prot;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00002FEE File Offset: 0x000011EE
		protected override void GetPluginsInternal(ConfuserContext context, IList<Protection> protections, IList<Packer> packers, IList<ConfuserComponent> components)
		{
			base.GetPluginsInternal(context, protections, packers, components);
			protections.Add(this.prot);
		}

		// Token: 0x040001D1 RID: 465
		private readonly Protection prot;
	}
}
