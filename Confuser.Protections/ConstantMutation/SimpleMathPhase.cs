using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000BE RID: 190
	public class SimpleMathPhase : ProtectionPhase
	{
		// Token: 0x0600032C RID: 812 RVA: 0x00002136 File Offset: 0x00000336
		public SimpleMathPhase(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600032D RID: 813 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x0600032E RID: 814 RVA: 0x000031BD File Offset: 0x000013BD
		public override string Name
		{
			get
			{
				return "Simple Math Mutation";
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00016C10 File Offset: 0x00014E10
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = methodDef.FullName.Contains("My.");
				if (!flag)
				{
					bool isConstructor = methodDef.IsConstructor;
					if (!isConstructor)
					{
						bool flag2 = !methodDef.HasBody;
						if (!flag2)
						{
							int i = 0;
							while (i < methodDef.Body.Instructions.Count)
							{
								bool flag3 = methodDef.Body.Instructions[i].IsLdcI4();
								if (flag3)
								{
									int ldcI4Value = methodDef.Body.Instructions[i].GetLdcI4Value();
									bool flag4 = ldcI4Value <= 1;
									if (!flag4)
									{
										int num = this.NextInt(1, (int)((double)ldcI4Value / 1.5));
										int num2 = ldcI4Value / num;
										while (num * num2 != ldcI4Value)
										{
											num = this.NextInt(1, (int)((double)ldcI4Value / 1.5));
											num2 = ldcI4Value / num;
										}
										methodDef.Body.Instructions[i].OpCode = OpCodes.Ldc_I4;
										methodDef.Body.Instructions[i].Operand = num2;
										methodDef.Body.Instructions.Insert(++i, OpCodes.Ldc_I4.ToInstruction(num));
										methodDef.Body.Instructions.Insert(++i, OpCodes.Mul.ToInstruction());
									}
								}
								IL_191:
								i++;
								continue;
								goto IL_191;
							}
							for (int j = 0; j < methodDef.Body.Instructions.Count; j++)
							{
								bool flag5 = methodDef.Body.Instructions[j].IsLdcI4();
								if (flag5)
								{
									List<Instruction> list = this.Calc(Convert.ToInt32(methodDef.Body.Instructions[j].Operand));
									methodDef.Body.Instructions[j].OpCode = OpCodes.Nop;
									foreach (Instruction item in list)
									{
										methodDef.Body.Instructions.Insert(++j, item);
									}
								}
							}
							int k = 0;
							while (k < methodDef.Body.Instructions.Count)
							{
								bool flag6 = methodDef.Body.Instructions[k].IsLdcI4();
								if (flag6)
								{
									int ldcI4Value2 = methodDef.Body.Instructions[k].GetLdcI4Value();
									bool flag7 = ldcI4Value2 <= 1;
									if (!flag7)
									{
										int num3 = this.NextInt(1, 10);
										methodDef.Body.Instructions[k].OpCode = OpCodes.Ldc_I4;
										methodDef.Body.Instructions[k].Operand = ldcI4Value2 * num3;
										methodDef.Body.Instructions.Insert(++k, OpCodes.Ldc_I4.ToInstruction(num3));
										methodDef.Body.Instructions.Insert(++k, OpCodes.Div.ToInstruction());
									}
								}
								IL_372:
								k++;
								continue;
								goto IL_372;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00017000 File Offset: 0x00015200
		private List<Instruction> Calc(int value)
		{
			List<Instruction> list = new List<Instruction>();
			int num = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
			bool flag = Convert.ToBoolean(new Random(Guid.NewGuid().GetHashCode()).Next(0, 2));
			int num2 = new Random(Guid.NewGuid().GetHashCode()).Next(0, 100000);
			list.Add(OpCodes.Ldc_I4.ToInstruction(value - num + (flag ? (0 - num2) : num2)));
			list.Add(OpCodes.Ldc_I4.ToInstruction(num));
			list.Add(OpCodes.Add.ToInstruction());
			list.Add(OpCodes.Ldc_I4.ToInstruction(num2));
			list.Add(flag ? OpCodes.Add.ToInstruction() : OpCodes.Sub.ToInstruction());
			return list;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00017100 File Offset: 0x00015300
		public int NextInt(int minValue, int maxValue)
		{
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			byte[] array = new byte[4];
			randomNumberGenerator.GetBytes(array);
			return (int)Math.Floor(BitConverter.ToUInt32(array, 0) / uint.MaxValue * (double)(maxValue - minValue)) + minValue;
		}
	}
}
