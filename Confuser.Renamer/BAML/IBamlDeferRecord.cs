using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000027 RID: 39
	internal interface IBamlDeferRecord
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600010E RID: 270
		// (set) Token: 0x0600010F RID: 271
		BamlRecord Record { get; set; }

		// Token: 0x06000110 RID: 272
		void ReadDefer(BamlDocument doc, int index, Func<long, BamlRecord> resolve);

		// Token: 0x06000111 RID: 273
		void WriteDefer(BamlDocument doc, int index, BinaryWriter wtr);
	}
}
