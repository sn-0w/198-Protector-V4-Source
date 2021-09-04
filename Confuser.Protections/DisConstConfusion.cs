using System;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000119 RID: 281
	[AfterProtection(new string[]
	{
		"Ki.Constants"
	})]
	internal class DisConstConfusion : Protection
	{
		// Token: 0x060004B8 RID: 1208 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x0000239E File Offset: 0x0000059E
		public override string Author
		{
			get
			{
				return "Ki (yck1509) & Wadu";
			}
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00003A04 File Offset: 0x00001C04
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new DisConstConfusion.DisConstConfusionPhase(this));
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x00003A13 File Offset: 0x00001C13
		public override string Description
		{
			get
			{
				return "This protection disintegrate the constants in the code into expressions.";
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x00003A1A File Offset: 0x00001C1A
		public override string FullId
		{
			get
			{
				return "Ki.disconstant";
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x00003A21 File Offset: 0x00001C21
		public override string Id
		{
			get
			{
				return "Const disint";
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x00003A28 File Offset: 0x00001C28
		public override string Name
		{
			get
			{
				return "Constant Disintegration";
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x04000263 RID: 611
		public const string _FullId = "Ki.disconstant";

		// Token: 0x04000264 RID: 612
		public const string _Id = "Const disint";

		// Token: 0x0200011A RID: 282
		private class DisConstConfusionPhase : ProtectionPhase
		{
			// Token: 0x060004C1 RID: 1217 RVA: 0x00002E01 File Offset: 0x00001001
			public DisConstConfusionPhase(DisConstConfusion parent) : base(parent)
			{
			}

			// Token: 0x060004C2 RID: 1218 RVA: 0x0001DBE4 File Offset: 0x0001BDE4
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				for (int i = 1; i < 1; i++)
				{
					foreach (IDnlibDef dnlibDef in parameters.Targets)
					{
						MethodDef methodDef = (MethodDef)dnlibDef;
						CilBody body = methodDef.Body;
						body.SimplifyBranches();
						Random random = new Random();
						int j = 0;
						while (j < body.Instructions.Count)
						{
							if (body.Instructions[j].IsLdcI4())
							{
								int ldcI4Value = body.Instructions[j].GetLdcI4Value();
								int num = random.Next(5, 40);
								body.Instructions[j].OpCode = OpCodes.Ldc_I4;
								body.Instructions[j].Operand = num * ldcI4Value;
								body.Instructions.Insert(j + 1, Instruction.Create(OpCodes.Ldc_I4, num));
								body.Instructions.Insert(j + 2, Instruction.Create(OpCodes.Div));
								j += 3;
							}
							else
							{
								j++;
							}
						}
						Random random2 = new Random();
						int num2 = 0;
						ITypeDefOrRef type = null;
						for (int k = 0; k < methodDef.Body.Instructions.Count; k++)
						{
							Instruction instruction = methodDef.Body.Instructions[k];
							if (instruction.IsLdcI4())
							{
								switch (random2.Next(1, 8))
								{
								case 1:
									type = methodDef.Module.Import(typeof(int));
									num2 = 4;
									break;
								case 2:
									type = methodDef.Module.Import(typeof(sbyte));
									num2 = 1;
									break;
								case 3:
									type = methodDef.Module.Import(typeof(byte));
									num2 = 1;
									break;
								case 4:
									type = methodDef.Module.Import(typeof(bool));
									num2 = 1;
									break;
								case 5:
									type = methodDef.Module.Import(typeof(decimal));
									num2 = 16;
									break;
								case 6:
									type = methodDef.Module.Import(typeof(short));
									num2 = 2;
									break;
								case 7:
									type = methodDef.Module.Import(typeof(long));
									num2 = 8;
									break;
								}
								int num3 = random2.Next(1, 1000);
								bool flag = Convert.ToBoolean(random2.Next(0, 2));
								switch ((num2 != 0) ? ((Convert.ToInt32(instruction.Operand) % num2 == 0) ? random2.Next(1, 5) : random2.Next(1, 4)) : random2.Next(1, 4))
								{
								case 1:
									methodDef.Body.Instructions.Insert(k + 1, Instruction.Create(OpCodes.Sizeof, type));
									methodDef.Body.Instructions.Insert(k + 2, Instruction.Create(OpCodes.Add));
									instruction.Operand = Convert.ToInt32(instruction.Operand) - num2 + (flag ? (-num3) : num3);
									goto IL_440;
								case 2:
									methodDef.Body.Instructions.Insert(k + 1, Instruction.Create(OpCodes.Sizeof, type));
									methodDef.Body.Instructions.Insert(k + 2, Instruction.Create(OpCodes.Sub));
									instruction.Operand = Convert.ToInt32(instruction.Operand) + num2 + (flag ? (-num3) : num3);
									goto IL_440;
								case 3:
									methodDef.Body.Instructions.Insert(k + 1, Instruction.Create(OpCodes.Sizeof, type));
									methodDef.Body.Instructions.Insert(k + 2, Instruction.Create(OpCodes.Add));
									instruction.Operand = Convert.ToInt32(instruction.Operand) - num2 + (flag ? (-num3) : num3);
									goto IL_440;
								case 4:
									methodDef.Body.Instructions.Insert(k + 1, Instruction.Create(OpCodes.Sizeof, type));
									methodDef.Body.Instructions.Insert(k + 2, Instruction.Create(OpCodes.Mul));
									instruction.Operand = Convert.ToInt32(instruction.Operand) / num2;
									break;
								default:
									goto IL_440;
								}
								IL_438:
								k += 2;
								goto IL_48C;
								IL_440:
								methodDef.Body.Instructions.Insert(k + 3, Instruction.CreateLdcI4(num3));
								methodDef.Body.Instructions.Insert(k + 4, Instruction.Create(flag ? OpCodes.Add : OpCodes.Sub));
								k += 2;
								goto IL_438;
							}
							IL_48C:;
						}
						body.OptimizeBranches();
					}
				}
			}

			// Token: 0x17000139 RID: 313
			// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00003A28 File Offset: 0x00001C28
			public override string Name
			{
				get
				{
					return "Constant Disintegration";
				}
			}

			// Token: 0x1700013A RID: 314
			// (get) Token: 0x060004C4 RID: 1220 RVA: 0x000021DB File Offset: 0x000003DB
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}
		}
	}
}
