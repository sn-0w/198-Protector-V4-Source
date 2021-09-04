using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x02000107 RID: 263
	public class MemoryEditApplyPhase : ProtectionPhase
	{
		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Fields;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x000037DA File Offset: 0x000019DA
		public override string Name
		{
			get
			{
				return "Apply memory protection";
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00002136 File Offset: 0x00000336
		public MemoryEditApplyPhase(ConfuserComponent parent) : base(parent)
		{
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001D484 File Offset: 0x0001B684
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			ModuleDefMD currentModule = context.CurrentModule;
			IMemoryEditService service = context.Registry.GetService<IMemoryEditService>();
			TypeDef wrapperType = service.GetWrapperType(currentModule);
			bool flag = wrapperType == null;
			if (!flag)
			{
				IMethod readMethod = service.GetReadMethod(currentModule);
				IMethod writeMethod = service.GetWriteMethod(currentModule);
				foreach (FieldDef fieldDef in service.GetFields())
				{
					fieldDef.FieldType = new GenericInstSig((ClassOrValueTypeSig)wrapperType.ToTypeSig(true), fieldDef.FieldType.ToTypeDefOrRefSig());
				}
				context.Logger.Debug("Looping through all types to patch access to obfuscated values");
				foreach (MethodDef methodDef in context.CurrentModule.GetTypes().SelectMany((TypeDef a) => a.Methods).WithProgress(context.Logger))
				{
					bool flag2 = !methodDef.HasBody || !methodDef.Body.HasInstructions;
					if (!flag2)
					{
						for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
						{
							Instruction instruction = methodDef.Body.Instructions[i];
							bool flag3 = instruction.Operand == null;
							if (!flag3)
							{
								object operand = instruction.Operand;
								FieldDef fieldDef2 = operand as FieldDef;
								GenericInstSig genericInstSig;
								bool flag4;
								if (fieldDef2 != null)
								{
									TypeSig fieldType = fieldDef2.FieldType;
									genericInstSig = (fieldType as GenericInstSig);
									flag4 = (genericInstSig == null);
								}
								else
								{
									flag4 = true;
								}
								bool flag5 = flag4;
								if (!flag5)
								{
									bool flag6 = !default(SigComparer).Equals(genericInstSig.GenericType.TypeDef, wrapperType);
									if (!flag6)
									{
										switch (instruction.OpCode.Code)
										{
										case Code.Ldfld:
										case Code.Ldsfld:
											methodDef.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, MemoryEditApplyPhase.FindReadMethod(currentModule, genericInstSig, readMethod)));
											i++;
											break;
										case Code.Stfld:
										case Code.Stsfld:
											methodDef.Body.Instructions.Insert(i, new Instruction(OpCodes.Call, MemoryEditApplyPhase.FindReadMethod(currentModule, genericInstSig, writeMethod)));
											i++;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001D754 File Offset: 0x0001B954
		private static MemberRefUser FindReadMethod(ModuleDef m, GenericInstSig sig, IMethod method)
		{
			return new MemberRefUser(m, method.Name, method.MethodSig)
			{
				Class = new TypeSpecUser(sig)
			};
		}
	}
}
