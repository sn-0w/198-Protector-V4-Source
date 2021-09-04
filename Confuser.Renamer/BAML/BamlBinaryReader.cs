using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005A RID: 90
	internal class BamlBinaryReader : BinaryReader
	{
		// Token: 0x0600025E RID: 606 RVA: 0x0000DD6B File Offset: 0x0000BF6B
		public BamlBinaryReader(Stream stream) : base(stream)
		{
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000DD78 File Offset: 0x0000BF78
		public int ReadEncodedInt()
		{
			return base.Read7BitEncodedInt();
		}
	}
}
