using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Elements;
using Confuser.DynCipher.Transforms;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000014 RID: 20
	internal class CipherGenerator
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00003EFC File Offset: 0x000020FC
		private static void Shuffle<T>(RandomGenerator random, IList<T> arr)
		{
			for (int i = 1; i < arr.Count; i++)
			{
				int index = random.NextInt32(i + 1);
				T value = arr[i];
				arr[i] = arr[index];
				arr[index] = value;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x0000235D File Offset: 0x0000055D
		private static void PostProcessStatements(StatementBlock block, RandomGenerator random)
		{
			MulToShiftTransform.Run(block);
			NormalizeBinOpTransform.Run(block);
			ExpansionTransform.Run(block);
			ShuffleTransform.Run(block, random);
			ConvertVariables.Run(block);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003F4C File Offset: 0x0000214C
		public static void GeneratePair(RandomGenerator random, out StatementBlock encrypt, out StatementBlock decrypt)
		{
			double num = 1.0 + (random.NextDouble() * 2.0 - 1.0) * 0.2;
			int num2 = (int)((random.NextDouble() + 1.0) * 35.0 * num);
			List<CryptoElement> list = new List<CryptoElement>();
			for (int i = 0; i < num2 * 4 / 35; i++)
			{
				list.Add(new Matrix());
			}
			for (int j = 0; j < num2 * 10 / 35; j++)
			{
				list.Add(new NumOp());
			}
			for (int k = 0; k < num2 * 6 / 35; k++)
			{
				list.Add(new Swap());
			}
			for (int l = 0; l < num2 * 9 / 35; l++)
			{
				list.Add(new BinOp());
			}
			for (int m = 0; m < num2 * 6 / 35; m++)
			{
				list.Add(new RotateBit());
			}
			for (int n = 0; n < 16; n++)
			{
				list.Add(new AddKey(n));
			}
			CipherGenerator.Shuffle<CryptoElement>(random, list);
			int[] array = Enumerable.Range(0, 16).ToArray<int>();
			int num3 = 16;
			bool flag = false;
			foreach (CryptoElement cryptoElement in list)
			{
				cryptoElement.Initialize(random);
				for (int num4 = 0; num4 < cryptoElement.DataCount; num4++)
				{
					bool flag2 = num3 == 16;
					if (flag2)
					{
						flag = true;
						num3 = 0;
					}
					cryptoElement.DataIndexes[num4] = array[num3++];
				}
				bool flag3 = flag;
				if (flag3)
				{
					CipherGenerator.Shuffle<int>(random, array);
					num3 = 0;
					flag = false;
				}
			}
			CipherGenContext cipherGenContext = new CipherGenContext(random, 16);
			foreach (CryptoElement cryptoElement2 in list)
			{
				cryptoElement2.Emit(cipherGenContext);
			}
			encrypt = cipherGenContext.Block;
			CipherGenerator.PostProcessStatements(encrypt, random);
			CipherGenContext cipherGenContext2 = new CipherGenContext(random, 16);
			foreach (CryptoElement cryptoElement3 in list.Reverse<CryptoElement>())
			{
				cryptoElement3.EmitInverse(cipherGenContext2);
			}
			decrypt = cipherGenContext2.Block;
			CipherGenerator.PostProcessStatements(decrypt, random);
		}

		// Token: 0x04000032 RID: 50
		private const int MAT_RATIO = 4;

		// Token: 0x04000033 RID: 51
		private const int NUMOP_RATIO = 10;

		// Token: 0x04000034 RID: 52
		private const int SWAP_RATIO = 6;

		// Token: 0x04000035 RID: 53
		private const int BINOP_RATIO = 9;

		// Token: 0x04000036 RID: 54
		private const int ROTATE_RATIO = 6;

		// Token: 0x04000037 RID: 55
		private const int RATIO_SUM = 35;

		// Token: 0x04000038 RID: 56
		private const double VARIANCE = 0.2;
	}
}
