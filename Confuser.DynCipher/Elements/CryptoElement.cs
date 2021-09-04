using System;
using Confuser.Core.Services;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000025 RID: 37
	internal abstract class CryptoElement
	{
		// Token: 0x060000B0 RID: 176 RVA: 0x00002530 File Offset: 0x00000730
		public CryptoElement(int count)
		{
			this.DataCount = count;
			this.DataIndexes = new int[count];
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x0000254F File Offset: 0x0000074F
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00002557 File Offset: 0x00000757
		public int DataCount { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00002560 File Offset: 0x00000760
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00002568 File Offset: 0x00000768
		public int[] DataIndexes { get; private set; }

		// Token: 0x060000B5 RID: 181
		public abstract void Initialize(RandomGenerator random);

		// Token: 0x060000B6 RID: 182
		public abstract void Emit(CipherGenContext context);

		// Token: 0x060000B7 RID: 183
		public abstract void EmitInverse(CipherGenContext context);
	}
}
