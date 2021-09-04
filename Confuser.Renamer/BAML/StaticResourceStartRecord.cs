using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000053 RID: 83
	internal class StaticResourceStartRecord : ElementStartRecord
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000DB4C File Offset: 0x0000BD4C
		public override BamlRecordType Type
		{
			get
			{
				return BamlRecordType.StaticResourceStart;
			}
		}
	}
}
