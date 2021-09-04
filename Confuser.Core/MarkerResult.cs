using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core
{
	// Token: 0x0200004F RID: 79
	public class MarkerResult
	{
		// Token: 0x060001F5 RID: 501 RVA: 0x00002CC3 File Offset: 0x00000EC3
		public MarkerResult(IList<ModuleDefMD> modules, Packer packer, IList<byte[]> extModules)
		{
			this.Modules = modules;
			this.Packer = packer;
			this.ExternalModules = extModules;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00002CE5 File Offset: 0x00000EE5
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x00002CED File Offset: 0x00000EED
		public IList<ModuleDefMD> Modules { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x00002CF6 File Offset: 0x00000EF6
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x00002CFE File Offset: 0x00000EFE
		public IList<byte[]> ExternalModules { get; private set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001FA RID: 506 RVA: 0x00002D07 File Offset: 0x00000F07
		// (set) Token: 0x060001FB RID: 507 RVA: 0x00002D0F File Offset: 0x00000F0F
		public Packer Packer { get; private set; }
	}
}
