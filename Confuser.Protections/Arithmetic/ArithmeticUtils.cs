using System;
using System.Collections.Generic;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DE RID: 222
	public class ArithmeticUtils
	{
		// Token: 0x060003AC RID: 940 RVA: 0x000194A8 File Offset: 0x000176A8
		public static bool CheckArithmetic(Instruction instruction)
		{
			bool flag = !instruction.IsLdcI4();
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = instruction.GetLdcI4Value() == 1;
				if (flag2)
				{
					result = false;
				}
				else
				{
					bool flag3 = instruction.GetLdcI4Value() == 0;
					result = !flag3;
				}
			}
			return result;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x000034A4 File Offset: 0x000016A4
		public static double GetY(double x)
		{
			return x / 2.0;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x000194F0 File Offset: 0x000176F0
		public static MethodInfo GetMethod(ArithmeticTypes mathType)
		{
			MethodInfo result;
			switch (mathType)
			{
			case ArithmeticTypes.Abs:
				result = typeof(Math).GetMethod("Abs", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Log:
				result = typeof(Math).GetMethod("Log", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Log10:
				result = typeof(Math).GetMethod("Log10", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Sin:
				result = typeof(Math).GetMethod("Sin", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Cos:
				result = typeof(Math).GetMethod("Cos", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Round:
				result = typeof(Math).GetMethod("Round", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Sqrt:
				result = typeof(Math).GetMethod("Sqrt", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Ceiling:
				result = typeof(Math).GetMethod("Ceiling", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Floor:
				result = typeof(Math).GetMethod("Floor", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Tan:
				result = typeof(Math).GetMethod("Tan", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Tanh:
				result = typeof(Math).GetMethod("Tanh", new Type[]
				{
					typeof(double)
				});
				break;
			case ArithmeticTypes.Truncate:
				result = typeof(Math).GetMethod("Truncate", new Type[]
				{
					typeof(double)
				});
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00019758 File Offset: 0x00017958
		public static OpCode GetOpCode(ArithmeticTypes arithmetic)
		{
			OpCode result;
			if (arithmetic != ArithmeticTypes.Add)
			{
				if (arithmetic != ArithmeticTypes.Sub)
				{
					result = null;
				}
				else
				{
					result = OpCodes.Sub;
				}
			}
			else
			{
				result = OpCodes.Add;
			}
			return result;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x0001978C File Offset: 0x0001798C
		public static List<Instruction> GenerateBody(ArithmeticVT arithmeticVTs, ModuleDef module)
		{
			List<Instruction> list = new List<Instruction>();
			bool flag = ArithmeticUtils.IsArithmetic(arithmeticVTs.GetArithmetic());
			if (flag)
			{
				list.Add(new Instruction(OpCodes.Ldc_R8, arithmeticVTs.GetValue().GetX()));
				list.Add(new Instruction(OpCodes.Ldc_R8, arithmeticVTs.GetValue().GetY()));
				bool flag2 = arithmeticVTs.GetToken().GetOperand() != null;
				if (flag2)
				{
					list.Add(new Instruction(OpCodes.Call, arithmeticVTs.GetToken().GetOperand()));
				}
				list.Add(new Instruction(arithmeticVTs.GetToken().GetOpCode()));
				list.Add(new Instruction(OpCodes.Call, module.Import(typeof(Convert).GetMethod("ToInt32", new Type[]
				{
					typeof(double)
				}))));
			}
			else
			{
				bool flag3 = ArithmeticUtils.IsXor(arithmeticVTs.GetArithmetic());
				if (flag3)
				{
					list.Add(new Instruction(OpCodes.Ldc_I4, (int)arithmeticVTs.GetValue().GetX()));
					list.Add(new Instruction(OpCodes.Ldc_I4, (int)arithmeticVTs.GetValue().GetY()));
					list.Add(new Instruction(arithmeticVTs.GetToken().GetOpCode()));
					list.Add(new Instruction(OpCodes.Conv_I4));
				}
			}
			return list;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00019904 File Offset: 0x00017B04
		public static bool IsArithmetic(ArithmeticTypes arithmetic)
		{
			return arithmetic == ArithmeticTypes.Add || arithmetic == ArithmeticTypes.Sub || arithmetic == ArithmeticTypes.Div || arithmetic == ArithmeticTypes.Mul || arithmetic == ArithmeticTypes.Abs || arithmetic == ArithmeticTypes.Log || arithmetic == ArithmeticTypes.Log10 || arithmetic == ArithmeticTypes.Truncate || arithmetic == ArithmeticTypes.Sin || arithmetic == ArithmeticTypes.Cos || arithmetic == ArithmeticTypes.Floor || arithmetic == ArithmeticTypes.Round || arithmetic == ArithmeticTypes.Tan || arithmetic == ArithmeticTypes.Tanh || arithmetic == ArithmeticTypes.Sqrt || arithmetic == ArithmeticTypes.Ceiling;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00019960 File Offset: 0x00017B60
		public static bool IsXor(ArithmeticTypes arithmetic)
		{
			return arithmetic == ArithmeticTypes.Xor;
		}
	}
}
