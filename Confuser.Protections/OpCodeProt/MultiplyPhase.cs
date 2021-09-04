using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.OpCodeProt
{
	// Token: 0x02000079 RID: 121
	public class MultiplyPhase : ProtectionPhase
	{
		// Token: 0x06000235 RID: 565 RVA: 0x00002136 File Offset: 0x00000336
		public MultiplyPhase(OpCodeProtection parent) : base(parent)
		{
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000236 RID: 566 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000237 RID: 567 RVA: 0x00002D2F File Offset: 0x00000F2F
		public override string Name
		{
			get
			{
				return "Multiply Protection";
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000D654 File Offset: 0x0000B854
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>())
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
							for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
							{
								bool flag3 = methodDef.Body.Instructions[i].OpCode == OpCodes.Mul;
								if (flag3)
								{
									bool flag4 = methodDef.Body.Instructions[i - 1].IsLdcI4() && methodDef.Body.Instructions[i - 2].IsLdcI4();
									if (flag4)
									{
										int ldcI4Value = methodDef.Body.Instructions[i - 2].GetLdcI4Value();
										int ldcI4Value2 = methodDef.Body.Instructions[i - 1].GetLdcI4Value();
										bool flag5 = ldcI4Value2 >= 3;
										if (flag5)
										{
											Local local = new Local(methodDef.Module.CorLibTypes.Int32);
											methodDef.Body.Variables.Add(local);
											methodDef.Body.Instructions.Insert(0, new Instruction(OpCodes.Stloc, local));
											methodDef.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4, ldcI4Value));
											i += 2;
											methodDef.Body.Instructions[i - 2].OpCode = OpCodes.Ldloc;
											methodDef.Body.Instructions[i - 2].Operand = local;
											methodDef.Body.Instructions[i - 1].OpCode = OpCodes.Nop;
											methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
											int num = 0;
											for (int j = ldcI4Value2; j > 0; j >>= 1)
											{
												bool flag6 = (j & 1) == 1;
												if (flag6)
												{
													bool flag7 = num != 0;
													if (flag7)
													{
														methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldloc, local));
														methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldc_I4, num));
														methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Shl));
														methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Add));
													}
												}
												num++;
											}
											bool flag8 = (ldcI4Value2 & 1) == 0;
											if (flag8)
											{
												methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Ldloc, local));
												methodDef.Body.Instructions.Insert(++i, new Instruction(OpCodes.Sub));
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
