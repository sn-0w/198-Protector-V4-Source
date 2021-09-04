using System;
using System.IO;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005B RID: 91
	internal class BamlBinaryWriter : BinaryWriter
	{
		// Token: 0x06000260 RID: 608 RVA: 0x0000DD90 File Offset: 0x0000BF90
		public BamlBinaryWriter(Stream stream) : base(stream)
		{
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000DD9B File Offset: 0x0000BF9B
		public void WriteEncodedInt(int val)
		{
			base.Write7BitEncodedInt(val);
		}
	}
}
