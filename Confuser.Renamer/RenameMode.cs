using System;

namespace Confuser.Renamer
{
	// Token: 0x0200000C RID: 12
	public enum RenameMode
	{
		// Token: 0x04000023 RID: 35
		Empty,
		// Token: 0x04000024 RID: 36
		Unicode,
		// Token: 0x04000025 RID: 37
		ASCII,
		// Token: 0x04000026 RID: 38
		Letters,
		// Token: 0x04000027 RID: 39
		Decodable = 16,
		// Token: 0x04000028 RID: 40
		Sequential,
		// Token: 0x04000029 RID: 41
		Reversible,
		// Token: 0x0400002A RID: 42
		Debug = 32,
		// Token: 0x0400002B RID: 43
		MSCorLib = 64,
		// Token: 0x0400002C RID: 44
		CryptoObfuscator = 128,
		// Token: 0x0400002D RID: 45
		RealNames = 352
	}
}
