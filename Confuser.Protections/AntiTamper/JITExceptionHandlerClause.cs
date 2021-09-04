using System;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F8 RID: 248
	internal struct JITExceptionHandlerClause
	{
		// Token: 0x040001F4 RID: 500
		public uint ClassTokenOrFilterOffset;

		// Token: 0x040001F5 RID: 501
		public uint Flags;

		// Token: 0x040001F6 RID: 502
		public uint HandlerLength;

		// Token: 0x040001F7 RID: 503
		public uint HandlerOffset;

		// Token: 0x040001F8 RID: 504
		public uint TryLength;

		// Token: 0x040001F9 RID: 505
		public uint TryOffset;
	}
}
