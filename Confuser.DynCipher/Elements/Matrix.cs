using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000026 RID: 38
	internal class Matrix : CryptoElement
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00002571 File Offset: 0x00000771
		public Matrix() : base(4)
		{
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000257C File Offset: 0x0000077C
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00002584 File Offset: 0x00000784
		public uint[,] Key { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000BB RID: 187 RVA: 0x0000258D File Offset: 0x0000078D
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00002595 File Offset: 0x00000795
		public uint[,] InverseKey { get; private set; }

		// Token: 0x060000BD RID: 189 RVA: 0x000063E8 File Offset: 0x000045E8
		private static uint[,] GenerateUnimodularMatrix(RandomGenerator random)
		{
			Func<uint> func = () => (uint)random.NextInt32(4);
			uint[,] array = new uint[,]
			{
				{
					1U,
					0U,
					0U,
					0U
				},
				{
					0U,
					1U,
					0U,
					0U
				},
				{
					0U,
					0U,
					1U,
					0U
				},
				{
					0U,
					0U,
					0U,
					1U
				}
			};
			array[1, 0] = func();
			array[2, 0] = func();
			array[2, 1] = func();
			array[3, 0] = func();
			array[3, 1] = func();
			array[3, 2] = func();
			uint[,] a = array;
			uint[,] array2 = new uint[,]
			{
				{
					1U,
					0U,
					0U,
					0U
				},
				{
					0U,
					1U,
					0U,
					0U
				},
				{
					0U,
					0U,
					1U,
					0U
				},
				{
					0U,
					0U,
					0U,
					1U
				}
			};
			array2[0, 1] = func();
			array2[0, 2] = func();
			array2[0, 3] = func();
			array2[1, 2] = func();
			array2[1, 3] = func();
			array2[2, 3] = func();
			uint[,] b = array2;
			return Matrix.mul(a, b);
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000064EC File Offset: 0x000046EC
		private static uint[,] mul(uint[,] a, uint[,] b)
		{
			int length = a.GetLength(0);
			int length2 = b.GetLength(1);
			int length3 = a.GetLength(1);
			bool flag = b.GetLength(0) != length3;
			uint[,] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				uint[,] array = new uint[length, length2];
				for (int i = 0; i < length; i++)
				{
					for (int j = 0; j < length2; j++)
					{
						array[i, j] = 0U;
						for (int k = 0; k < length3; k++)
						{
							array[i, j] += a[i, k] * b[k, j];
						}
					}
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000065AC File Offset: 0x000047AC
		private static uint cofactor4(uint[,] mat, int i, int j)
		{
			uint[,] array = new uint[3, 3];
			int k = 0;
			int num = 0;
			while (k < 4)
			{
				bool flag = k == i;
				if (flag)
				{
					num--;
				}
				else
				{
					int l = 0;
					int num2 = 0;
					while (l < 4)
					{
						bool flag2 = l == j;
						if (flag2)
						{
							num2--;
						}
						else
						{
							array[num, num2] = mat[k, l];
						}
						l++;
						num2++;
					}
				}
				k++;
				num++;
			}
			uint num3 = Matrix.det3(array);
			bool flag3 = (i + j) % 2 == 0;
			uint result;
			if (flag3)
			{
				result = num3;
			}
			else
			{
				result = (uint)(-(uint)((ulong)num3));
			}
			return result;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006658 File Offset: 0x00004858
		private static uint det3(uint[,] mat)
		{
			return mat[0, 0] * mat[1, 1] * mat[2, 2] + mat[0, 1] * mat[1, 2] * mat[2, 0] + mat[0, 2] * mat[1, 0] * mat[2, 1] - mat[0, 2] * mat[1, 1] * mat[2, 0] - mat[0, 1] * mat[1, 0] * mat[2, 2] - mat[0, 0] * mat[1, 2] * mat[2, 1];
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000670C File Offset: 0x0000490C
		private static uint[,] transpose4(uint[,] mat)
		{
			uint[,] array = new uint[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[j, i] = mat[i, j];
				}
			}
			return array;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000675C File Offset: 0x0000495C
		public override void Initialize(RandomGenerator random)
		{
			this.InverseKey = Matrix.mul(Matrix.transpose4(Matrix.GenerateUnimodularMatrix(random)), Matrix.GenerateUnimodularMatrix(random));
			uint[,] array = new uint[4, 4];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					array[i, j] = Matrix.cofactor4(this.InverseKey, i, j);
				}
			}
			this.Key = Matrix.transpose4(array);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000067D4 File Offset: 0x000049D4
		private void EmitCore(CipherGenContext context, uint[,] k)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			Expression dataExpression2 = context.GetDataExpression(base.DataIndexes[1]);
			Expression dataExpression3 = context.GetDataExpression(base.DataIndexes[2]);
			Expression dataExpression4 = context.GetDataExpression(base.DataIndexes[3]);
			Func<uint, LiteralExpression> func = (uint v) => v;
			VariableExpression variableExpression;
			using (context.AcquireTempVar(out variableExpression))
			{
				VariableExpression variableExpression2;
				using (context.AcquireTempVar(out variableExpression2))
				{
					VariableExpression variableExpression3;
					using (context.AcquireTempVar(out variableExpression3))
					{
						VariableExpression variableExpression4;
						using (context.AcquireTempVar(out variableExpression4))
						{
							context.Emit(new AssignmentStatement
							{
								Value = dataExpression * func(k[0, 0]) + dataExpression2 * func(k[0, 1]) + dataExpression3 * func(k[0, 2]) + dataExpression4 * func(k[0, 3]),
								Target = variableExpression
							}).Emit(new AssignmentStatement
							{
								Value = dataExpression * func(k[1, 0]) + dataExpression2 * func(k[1, 1]) + dataExpression3 * func(k[1, 2]) + dataExpression4 * func(k[1, 3]),
								Target = variableExpression2
							}).Emit(new AssignmentStatement
							{
								Value = dataExpression * func(k[2, 0]) + dataExpression2 * func(k[2, 1]) + dataExpression3 * func(k[2, 2]) + dataExpression4 * func(k[2, 3]),
								Target = variableExpression3
							}).Emit(new AssignmentStatement
							{
								Value = dataExpression * func(k[3, 0]) + dataExpression2 * func(k[3, 1]) + dataExpression3 * func(k[3, 2]) + dataExpression4 * func(k[3, 3]),
								Target = variableExpression4
							}).Emit(new AssignmentStatement
							{
								Value = variableExpression,
								Target = dataExpression
							}).Emit(new AssignmentStatement
							{
								Value = variableExpression2,
								Target = dataExpression2
							}).Emit(new AssignmentStatement
							{
								Value = variableExpression3,
								Target = dataExpression3
							}).Emit(new AssignmentStatement
							{
								Value = variableExpression4,
								Target = dataExpression4
							});
						}
					}
				}
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000259E File Offset: 0x0000079E
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context, this.Key);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000025AF File Offset: 0x000007AF
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context, this.InverseKey);
		}
	}
}
