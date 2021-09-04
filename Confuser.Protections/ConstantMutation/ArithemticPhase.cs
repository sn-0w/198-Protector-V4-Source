using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Protections.Arithmetic;
using Confuser.Protections.Arithmetic.Functions;
using Confuser.Protections.Arithmetic.Functions.Maths;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000BC RID: 188
	public class ArithemticPhase : ProtectionPhase
	{
		// Token: 0x06000324 RID: 804 RVA: 0x0001640C File Offset: 0x0001460C
		public ArithemticPhase(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000325 RID: 805 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000326 RID: 806 RVA: 0x000031AF File Offset: 0x000013AF
		public override string Name
		{
			get
			{
				return "Arithemtic Mutation";
			}
		}

		// Token: 0x06000327 RID: 807 RVA: 0x000164FC File Offset: 0x000146FC
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			Generator generator = new Generator();
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = methodDef.FullName.Contains("My.");
				if (!flag)
				{
					bool flag2 = !methodDef.HasBody;
					if (!flag2)
					{
						int i = 0;
						while (i < methodDef.Body.Instructions.Count)
						{
							bool flag3 = ArithmeticUtils.CheckArithmetic(methodDef.Body.Instructions[i]);
							if (flag3)
							{
								bool flag4 = methodDef.Body.Instructions[i].GetLdcI4Value() < 0;
								if (flag4)
								{
									iFunction iFunction = this.Tasks[generator.Next(5)];
									List<Instruction> list = ArithmeticUtils.GenerateBody(iFunction.Arithmetic(methodDef.Body.Instructions[i], context.CurrentModule), context.CurrentModule);
									bool flag5 = list == null;
									if (!flag5)
									{
										methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
										foreach (Instruction item in list)
										{
											methodDef.Body.Instructions.Insert(++i, item);
										}
									}
								}
								else
								{
									iFunction iFunction2 = this.Tasks[generator.Next(this.Tasks.Count)];
									List<Instruction> list2 = ArithmeticUtils.GenerateBody(iFunction2.Arithmetic(methodDef.Body.Instructions[i], context.CurrentModule), context.CurrentModule);
									bool flag6 = list2 == null;
									if (!flag6)
									{
										methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
										foreach (Instruction item2 in list2)
										{
											methodDef.Body.Instructions.Insert(++i, item2);
										}
									}
								}
							}
							IL_22B:
							i++;
							continue;
							goto IL_22B;
						}
					}
				}
			}
		}

		// Token: 0x04000184 RID: 388
		private List<iFunction> Tasks = new List<iFunction>
		{
			new Add(),
			new Sub(),
			new Div(),
			new Mul(),
			new Xor(),
			new Abs(),
			new Log(),
			new Log10(),
			new Sin(),
			new Cos(),
			new Floor(),
			new Round(),
			new Tan(),
			new Tanh(),
			new Sqrt(),
			new Ceiling(),
			new Truncate()
		};
	}
}
