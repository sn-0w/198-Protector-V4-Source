using System;
using System.Collections.Generic;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000021 RID: 33
	internal class BamlDocument : List<BamlRecord>
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000C188 File Offset: 0x0000A388
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x0000C190 File Offset: 0x0000A390
		public string DocumentName { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x0000C199 File Offset: 0x0000A399
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x0000C1A1 File Offset: 0x0000A3A1
		public string Signature { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000C1AA File Offset: 0x0000A3AA
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000C1B2 File Offset: 0x0000A3B2
		public BamlDocument.BamlVersion ReaderVersion { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000C1BB File Offset: 0x0000A3BB
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x0000C1C3 File Offset: 0x0000A3C3
		public BamlDocument.BamlVersion UpdaterVersion { get; set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000C1CC File Offset: 0x0000A3CC
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x0000C1D4 File Offset: 0x0000A3D4
		public BamlDocument.BamlVersion WriterVersion { get; set; }

		// Token: 0x02000093 RID: 147
		public struct BamlVersion
		{
			// Token: 0x04000584 RID: 1412
			public ushort Major;

			// Token: 0x04000585 RID: 1413
			public ushort Minor;
		}
	}
}
