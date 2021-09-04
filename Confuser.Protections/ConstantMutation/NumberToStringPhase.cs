using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000BD RID: 189
	public class NumberToStringPhase : ProtectionPhase
	{
		// Token: 0x06000328 RID: 808 RVA: 0x00002136 File Offset: 0x00000336
		public NumberToStringPhase(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000329 RID: 809 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x0600032A RID: 810 RVA: 0x000031B6 File Offset: 0x000013B6
		public override string Name
		{
			get
			{
				return "Number To String";
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x000167BC File Offset: 0x000149BC
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
							for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
							{
								bool flag3 = methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4;
								if (flag3)
								{
									string s = methodDef.Body.Instructions[i].Operand.ToString();
									methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
									methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldstr, s));
									methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, methodDef.Module.Import(typeof(int).GetMethod("Parse", new Type[]
									{
										typeof(string)
									}))));
								}
								else
								{
									bool flag4 = methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I4_S;
									if (flag4)
									{
										string s2 = methodDef.Body.Instructions[i].Operand.ToString();
										methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
										methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldstr, s2));
										methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, methodDef.Module.Import(typeof(short).GetMethod("Parse", new Type[]
										{
											typeof(string)
										}))));
									}
									else
									{
										bool flag5 = methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_I8;
										if (flag5)
										{
											string s3 = methodDef.Body.Instructions[i].Operand.ToString();
											methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
											methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldstr, s3));
											methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, methodDef.Module.Import(typeof(long).GetMethod("Parse", new Type[]
											{
												typeof(string)
											}))));
										}
										else
										{
											bool flag6 = methodDef.Body.Instructions[i].OpCode == OpCodes.Ldc_R4;
											if (flag6)
											{
												string s4 = methodDef.Body.Instructions[i].Operand.ToString();
												methodDef.Body.Instructions[i].OpCode = OpCodes.Nop;
												methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Ldstr, s4));
												methodDef.Body.Instructions.Insert(++i, Instruction.Create(OpCodes.Call, methodDef.Module.Import(typeof(float).GetMethod("Parse", new Type[]
												{
													typeof(string)
												}))));
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
