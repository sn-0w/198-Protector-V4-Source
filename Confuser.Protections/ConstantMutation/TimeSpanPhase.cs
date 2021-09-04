using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ConstantMutation
{
	// Token: 0x020000C1 RID: 193
	public class TimeSpanPhase : ProtectionPhase
	{
		// Token: 0x06000339 RID: 825 RVA: 0x00002136 File Offset: 0x00000336
		public TimeSpanPhase(ConstantMutationProtection parent) : base(parent)
		{
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600033A RID: 826 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600033B RID: 827 RVA: 0x000031E9 File Offset: 0x000013E9
		public override string Name
		{
			get
			{
				return "TimeSpan Mutations";
			}
		}

		// Token: 0x0600033C RID: 828 RVA: 0x000173BC File Offset: 0x000155BC
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
							Importer importer = new Importer(methodDef.Module);
							int i = 0;
							while (i < methodDef.Body.Instructions.Count)
							{
								bool flag3 = methodDef.Body.Instructions[i].IsLdcI4();
								if (flag3)
								{
									bool flag4 = !TimeSpanPhase.CanMutateLDCI4(methodDef.Body.Instructions, i);
									if (!flag4)
									{
										int ldcI4Value = methodDef.Body.Instructions[i].GetLdcI4Value();
										bool flag5 = ldcI4Value < 1 || ldcI4Value > 10099999;
										if (!flag5)
										{
											TypeRef type = new TypeRefUser(methodDef.Module, "System", "TimeSpan", methodDef.Module.CorLibTypes.AssemblyRef);
											int num = new Random().Next(0, 10);
											int num2 = new Random().Next(0, 10);
											int value = methodDef.Body.Instructions[i].GetLdcI4Value() - num - num2;
											Local local = new Local(importer.Import(type.ToTypeSig(true)));
											methodDef.Body.Variables.Add(local);
											methodDef.Body.Instructions[i] = Instruction.CreateLdcI4(value);
											methodDef.Body.Instructions.Insert(++i, Instruction.CreateLdcI4(num * 24));
											methodDef.Body.Instructions.Insert(++i, Instruction.CreateLdcI4(num2 * 1440));
											methodDef.Body.Instructions.Insert(++i, Instruction.CreateLdcI4(new Random().Next(0, 59)));
											methodDef.Body.Instructions.Insert(++i, OpCodes.Newobj.ToInstruction(importer.Import(typeof(TimeSpan).GetConstructor(new Type[]
											{
												typeof(int),
												typeof(int),
												typeof(int),
												typeof(int)
											}))));
											methodDef.Body.Instructions.Insert(++i, OpCodes.Stloc.ToInstruction(local));
											methodDef.Body.Instructions.Insert(++i, OpCodes.Ldloca.ToInstruction(local));
											methodDef.Body.Instructions.Insert(++i, OpCodes.Call.ToInstruction(importer.Import(typeof(TimeSpan).GetMethod("get_TotalDays"))));
											methodDef.Body.Instructions.Insert(++i, OpCodes.Conv_I4.ToInstruction());
										}
									}
								}
								IL_332:
								i++;
								continue;
								goto IL_332;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00017754 File Offset: 0x00015954
		public static bool CanMutateLDCI4(IList<Instruction> instructions, int i)
		{
			bool flag = instructions[i + 1].GetOperand() != null;
			if (flag)
			{
				bool flag2 = instructions[i + 1].Operand.ToString().Contains("bool");
				if (flag2)
				{
					return false;
				}
			}
			bool flag3 = instructions[i + 1].OpCode == OpCodes.Newobj;
			bool result;
			if (flag3)
			{
				result = false;
			}
			else
			{
				bool flag4 = instructions[i].GetLdcI4Value() == 0 || instructions[i].GetLdcI4Value() == 1;
				result = !flag4;
			}
			return result;
		}
	}
}
