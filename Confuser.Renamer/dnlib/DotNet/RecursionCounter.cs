using System;

namespace dnlib.DotNet
{
	// Token: 0x0200007D RID: 125
	public struct RecursionCounter
	{
		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000316 RID: 790 RVA: 0x0002A3E0 File Offset: 0x000285E0
		public int Counter
		{
			get
			{
				return this.counter;
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0002A3F8 File Offset: 0x000285F8
		public bool Increment()
		{
			bool flag = this.counter >= 100;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.counter++;
				result = true;
			}
			return result;
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0002A430 File Offset: 0x00028630
		public void Decrement()
		{
			bool flag = this.counter <= 0;
			if (flag)
			{
				throw new InvalidOperationException("recursionCounter <= 0");
			}
			this.counter--;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0002A468 File Offset: 0x00028668
		public override string ToString()
		{
			return this.counter.ToString();
		}

		// Token: 0x04000541 RID: 1345
		public const int MAX_RECURSION_COUNT = 100;

		// Token: 0x04000542 RID: 1346
		private int counter;
	}
}
