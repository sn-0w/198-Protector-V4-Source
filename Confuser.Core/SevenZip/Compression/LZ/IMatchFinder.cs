using System;

namespace SevenZip.Compression.LZ
{
	// Token: 0x02000012 RID: 18
	internal interface IMatchFinder : IInWindowStream
	{
		// Token: 0x06000043 RID: 67
		void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter);

		// Token: 0x06000044 RID: 68
		uint GetMatches(uint[] distances);

		// Token: 0x06000045 RID: 69
		void Skip(uint num);
	}
}
