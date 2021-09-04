using System;
using Confuser.Core.Project;

namespace Confuser.Core
{
	// Token: 0x0200003B RID: 59
	public class ConfuserParameters
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00002865 File Offset: 0x00000A65
		// (set) Token: 0x0600013A RID: 314 RVA: 0x0000286D File Offset: 0x00000A6D
		public ConfuserProject Project { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00002876 File Offset: 0x00000A76
		// (set) Token: 0x0600013C RID: 316 RVA: 0x0000287E File Offset: 0x00000A7E
		public ILogger Logger { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00002887 File Offset: 0x00000A87
		// (set) Token: 0x0600013E RID: 318 RVA: 0x0000288F File Offset: 0x00000A8F
		internal bool PackerInitiated { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00002898 File Offset: 0x00000A98
		// (set) Token: 0x06000140 RID: 320 RVA: 0x000028A0 File Offset: 0x00000AA0
		public PluginDiscovery PluginDiscovery { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000028A9 File Offset: 0x00000AA9
		// (set) Token: 0x06000142 RID: 322 RVA: 0x000028B1 File Offset: 0x00000AB1
		public Marker Marker { get; set; }

		// Token: 0x06000143 RID: 323 RVA: 0x0000C360 File Offset: 0x0000A560
		internal ILogger GetLogger()
		{
			return this.Logger ?? NullLogger.Instance;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000C384 File Offset: 0x0000A584
		internal PluginDiscovery GetPluginDiscovery()
		{
			return this.PluginDiscovery ?? PluginDiscovery.Instance;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000C3A8 File Offset: 0x0000A5A8
		internal Marker GetMarker()
		{
			return this.Marker ?? new ObfAttrMarker();
		}
	}
}
