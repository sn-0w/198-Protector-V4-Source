using System;
using System.Text;

namespace Confuser.Core.Services
{
	// Token: 0x0200007E RID: 126
	internal class RandomService : IRandomService
	{
		// Token: 0x0600030A RID: 778 RVA: 0x000034B5 File Offset: 0x000016B5
		public RandomService(string seed)
		{
			this.seed = RandomGenerator.Seed(seed);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00013890 File Offset: 0x00011A90
		public RandomGenerator GetRandomGenerator(string id)
		{
			bool flag = string.IsNullOrEmpty(id);
			if (flag)
			{
				throw new ArgumentNullException("id");
			}
			byte[] array = this.seed;
			byte[] array2 = Utils.SHA256(Encoding.UTF8.GetBytes(id));
			for (int i = 0; i < 32; i++)
			{
				byte[] array3 = array;
				int num = i;
				array3[num] ^= array2[i];
			}
			return new RandomGenerator(Utils.SHA256(array));
		}

		// Token: 0x04000239 RID: 569
		private readonly byte[] seed;
	}
}
