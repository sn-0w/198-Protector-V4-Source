using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000025 RID: 37
	internal abstract class BamlRecord
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000102 RID: 258
		public abstract BamlRecordType Type { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000103 RID: 259 RVA: 0x0000C68C File Offset: 0x0000A88C
		// (set) Token: 0x06000104 RID: 260 RVA: 0x0000C694 File Offset: 0x0000A894
		public long Position { get; internal set; }

		// Token: 0x06000105 RID: 261
		public abstract void Read(BamlBinaryReader reader);

		// Token: 0x06000106 RID: 262
		public abstract void Write(BamlBinaryWriter writer);
	}
}
