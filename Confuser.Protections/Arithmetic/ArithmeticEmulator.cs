using System;
using System.Collections.Generic;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000D7 RID: 215
	public class ArithmeticEmulator
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000393 RID: 915 RVA: 0x000033C5 File Offset: 0x000015C5
		// (set) Token: 0x06000394 RID: 916 RVA: 0x000033CD File Offset: 0x000015CD
		public new ArithmeticTypes GetType { get; private set; }

		// Token: 0x06000395 RID: 917 RVA: 0x000033D6 File Offset: 0x000015D6
		public ArithmeticEmulator(double x, double y, ArithmeticTypes ArithmeticTypes)
		{
			this.x = x;
			this.y = y;
			this.ArithmeticTypes = ArithmeticTypes;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00018F84 File Offset: 0x00017184
		public double GetValue()
		{
			double result;
			switch (this.ArithmeticTypes)
			{
			case ArithmeticTypes.Add:
				result = this.x - this.y;
				break;
			case ArithmeticTypes.Sub:
				result = this.x + this.y;
				break;
			case ArithmeticTypes.Div:
				result = this.x * this.y;
				break;
			case ArithmeticTypes.Mul:
				result = this.x / this.y;
				break;
			case ArithmeticTypes.Xor:
				result = (double)((int)this.x ^ (int)this.y);
				break;
			default:
				result = -1.0;
				break;
			}
			return result;
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00019018 File Offset: 0x00017218
		public double GetValue(List<ArithmeticTypes> arithmetics)
		{
			Generator generator = new Generator();
			ArithmeticTypes arithmeticTypes = arithmetics[generator.Next(arithmetics.Count)];
			this.GetType = arithmeticTypes;
			double result;
			switch (this.ArithmeticTypes)
			{
			case ArithmeticTypes.Abs:
			{
				ArithmeticTypes arithmeticTypes2 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes3 = arithmeticTypes2;
				if (arithmeticTypes3 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes3 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x - Math.Abs(this.y) * -1.0;
					}
				}
				else
				{
					result = this.x + Math.Abs(this.y) * -1.0;
				}
				break;
			}
			case ArithmeticTypes.Log:
			{
				ArithmeticTypes arithmeticTypes4 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes5 = arithmeticTypes4;
				if (arithmeticTypes5 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes5 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Log(this.y);
					}
				}
				else
				{
					result = this.x - Math.Log(this.y);
				}
				break;
			}
			case ArithmeticTypes.Log10:
			{
				ArithmeticTypes arithmeticTypes6 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes7 = arithmeticTypes6;
				if (arithmeticTypes7 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes7 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Log10(this.y);
					}
				}
				else
				{
					result = this.x - Math.Log10(this.y);
				}
				break;
			}
			case ArithmeticTypes.Sin:
			{
				ArithmeticTypes arithmeticTypes8 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes9 = arithmeticTypes8;
				if (arithmeticTypes9 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes9 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Sin(this.y);
					}
				}
				else
				{
					result = this.x - Math.Sin(this.y);
				}
				break;
			}
			case ArithmeticTypes.Cos:
			{
				ArithmeticTypes arithmeticTypes10 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes11 = arithmeticTypes10;
				if (arithmeticTypes11 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes11 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Cos(this.y);
					}
				}
				else
				{
					result = this.x - Math.Cos(this.y);
				}
				break;
			}
			case ArithmeticTypes.Round:
			{
				ArithmeticTypes arithmeticTypes12 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes13 = arithmeticTypes12;
				if (arithmeticTypes13 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes13 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Round(this.y);
					}
				}
				else
				{
					result = this.x - Math.Round(this.y);
				}
				break;
			}
			case ArithmeticTypes.Sqrt:
			{
				ArithmeticTypes arithmeticTypes14 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes15 = arithmeticTypes14;
				if (arithmeticTypes15 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes15 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Sqrt(this.y);
					}
				}
				else
				{
					result = this.x - Math.Sqrt(this.y);
				}
				break;
			}
			case ArithmeticTypes.Ceiling:
			{
				ArithmeticTypes arithmeticTypes16 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes17 = arithmeticTypes16;
				if (arithmeticTypes17 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes17 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Ceiling(this.y);
					}
				}
				else
				{
					result = this.x - Math.Ceiling(this.y);
				}
				break;
			}
			case ArithmeticTypes.Floor:
			{
				ArithmeticTypes arithmeticTypes18 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes19 = arithmeticTypes18;
				if (arithmeticTypes19 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes19 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Floor(this.y);
					}
				}
				else
				{
					result = this.x - Math.Floor(this.y);
				}
				break;
			}
			case ArithmeticTypes.Tan:
			{
				ArithmeticTypes arithmeticTypes20 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes21 = arithmeticTypes20;
				if (arithmeticTypes21 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes21 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Tan(this.y);
					}
				}
				else
				{
					result = this.x - Math.Tan(this.y);
				}
				break;
			}
			case ArithmeticTypes.Tanh:
			{
				ArithmeticTypes arithmeticTypes22 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes23 = arithmeticTypes22;
				if (arithmeticTypes23 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes23 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Tanh(this.y);
					}
				}
				else
				{
					result = this.x - Math.Tanh(this.y);
				}
				break;
			}
			case ArithmeticTypes.Truncate:
			{
				ArithmeticTypes arithmeticTypes24 = arithmeticTypes;
				ArithmeticTypes arithmeticTypes25 = arithmeticTypes24;
				if (arithmeticTypes25 != ArithmeticTypes.Add)
				{
					if (arithmeticTypes25 != ArithmeticTypes.Sub)
					{
						result = -1.0;
					}
					else
					{
						result = this.x + Math.Truncate(this.y);
					}
				}
				else
				{
					result = this.x - Math.Truncate(this.y);
				}
				break;
			}
			default:
				result = -1.0;
				break;
			}
			return result;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x000033F5 File Offset: 0x000015F5
		public double GetX()
		{
			return this.x;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x000033FD File Offset: 0x000015FD
		public double GetY()
		{
			return this.y;
		}

		// Token: 0x040001C0 RID: 448
		private double x;

		// Token: 0x040001C1 RID: 449
		private double y;

		// Token: 0x040001C2 RID: 450
		private ArithmeticTypes ArithmeticTypes;
	}
}
