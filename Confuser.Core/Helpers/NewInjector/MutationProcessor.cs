using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000C2 RID: 194
	public class MutationProcessor : IMethodInjectProcessor
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x00003C20 File Offset: 0x00001E20
		private ITraceService TraceService { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x00003C28 File Offset: 0x00001E28
		private ModuleDef TargetModule { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x00003C30 File Offset: 0x00001E30
		// (set) Token: 0x06000463 RID: 1123 RVA: 0x00003C38 File Offset: 0x00001E38
		public IReadOnlyDictionary<MutationField, int> KeyFieldValues { get; set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x00003C41 File Offset: 0x00001E41
		// (set) Token: 0x06000465 RID: 1125 RVA: 0x00003C49 File Offset: 0x00001E49
		public IReadOnlyDictionary<MutationField, LateMutationFieldUpdate> LateKeyFieldValues { get; set; }

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000466 RID: 1126 RVA: 0x00003C52 File Offset: 0x00001E52
		// (set) Token: 0x06000467 RID: 1127 RVA: 0x00003C5A File Offset: 0x00001E5A
		public PlaceholderProcessor PlaceholderProcessor { get; set; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00003C63 File Offset: 0x00001E63
		// (set) Token: 0x06000469 RID: 1129 RVA: 0x00003C6B File Offset: 0x00001E6B
		public CryptProcessor CryptProcessor { get; set; }

		// Token: 0x0600046A RID: 1130 RVA: 0x00003C74 File Offset: 0x00001E74
		public MutationProcessor(ConfuserContext context, ModuleDef targetModule)
		{
			if (targetModule == null)
			{
				throw new ArgumentNullException("targetModule");
			}
			this.TargetModule = targetModule;
			this.TraceService = context.Registry.GetService<ITraceService>();
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001A038 File Offset: 0x00018238
		void IMethodInjectProcessor.Process(MethodDef method)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(method.HasBody, "method.HasBody");
			bool flag = method == null || !method.HasBody || !method.Body.HasInstructions;
			if (!flag)
			{
				IList<Instruction> instructions = method.Body.Instructions;
				for (int i = 0; i < instructions.Count; i++)
				{
					Instruction instruction = instructions[i];
					bool flag2 = instruction.OpCode == OpCodes.Ldsfld;
					if (flag2)
					{
						object operand = instruction.Operand;
						IField field = operand as IField;
						bool flag3 = field != null && field.DeclaringType.FullName == "Mutation";
						if (flag3)
						{
							bool flag4 = !this.ProcessKeyField(method, instruction, field);
							if (flag4)
							{
								throw new InvalidOperationException("Unexpected load field operation to Mutation class!");
							}
						}
					}
					else
					{
						bool flag5 = instruction.OpCode == OpCodes.Call;
						if (flag5)
						{
							object operand2 = instruction.Operand;
							IMethod method2 = operand2 as IMethod;
							bool flag6 = method2 != null && method2.DeclaringType.FullName == "Mutation";
							if (flag6)
							{
								bool flag7 = !this.ReplacePlaceholder(method, instruction, method2, ref i) && !this.ReplaceCrypt(method, instruction, method2, ref i);
								if (flag7)
								{
									throw new InvalidOperationException("Unexpected call operation to Mutation class!");
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001A1A8 File Offset: 0x000183A8
		private bool ProcessKeyField(MethodDef method, Instruction instr, IField field)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instr != null, "instr != null");
			Debug.Assert(field != null, "field != null");
			UTF8String name = field.Name;
			bool flag = name != null && name.Length >= 5 && field.Name.StartsWith("KeyI");
			if (flag)
			{
				int num;
				bool flag2 = int.TryParse(MemoryExtensions.AsSpan(field.Name.String).Slice(4, (field.Name.Length == 5) ? 1 : 2).ToString(), out num);
				if (flag2)
				{
					MutationField key;
					switch (num)
					{
					case 0:
						key = MutationField.KeyI0;
						break;
					case 1:
						key = MutationField.KeyI1;
						break;
					case 2:
						key = MutationField.KeyI2;
						break;
					case 3:
						key = MutationField.KeyI3;
						break;
					case 4:
						key = MutationField.KeyI4;
						break;
					case 5:
						key = MutationField.KeyI5;
						break;
					case 6:
						key = MutationField.KeyI6;
						break;
					case 7:
						key = MutationField.KeyI7;
						break;
					case 8:
						key = MutationField.KeyI8;
						break;
					case 9:
						key = MutationField.KeyI9;
						break;
					case 10:
						key = MutationField.KeyI10;
						break;
					case 11:
						key = MutationField.KeyI11;
						break;
					case 12:
						key = MutationField.KeyI12;
						break;
					case 13:
						key = MutationField.KeyI13;
						break;
					case 14:
						key = MutationField.KeyI14;
						break;
					case 15:
						key = MutationField.KeyI15;
						break;
					default:
						return false;
					}
					int num2;
					bool flag3 = this.KeyFieldValues != null && this.KeyFieldValues.TryGetValue(key, out num2);
					if (flag3)
					{
						instr.OpCode = OpCodes.Ldc_I4;
						instr.Operand = num2;
						return true;
					}
					LateMutationFieldUpdate lateMutationFieldUpdate;
					bool flag4 = this.LateKeyFieldValues != null && this.LateKeyFieldValues.TryGetValue(key, out lateMutationFieldUpdate);
					if (flag4)
					{
						lateMutationFieldUpdate.AddUpdateInstruction(method, instr);
						instr.OpCode = OpCodes.Ldc_I4_0;
						instr.Operand = null;
						return true;
					}
					throw new InvalidOperationException(string.Format("Code contains request to mutation key {0}, but the value for this field is not set.", field.Name));
				}
			}
			return false;
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001A3B0 File Offset: 0x000185B0
		private bool ReplacePlaceholder(MethodDef method, Instruction instr, IMethod calledMethod, ref int index)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instr != null, "instr != null");
			Debug.Assert(calledMethod != null, "calledMethod != null");
			bool flag = calledMethod.Name == "Placeholder";
			bool result;
			if (flag)
			{
				bool flag2 = this.PlaceholderProcessor == null;
				if (flag2)
				{
					throw new InvalidOperationException("Found mutation placeholder, but there is no processor defined.");
				}
				MethodTrace methodTrace = this.TraceService.Trace(method);
				int[] array = methodTrace.TraceArguments(instr);
				bool flag3 = array == null;
				if (flag3)
				{
					throw new InvalidOperationException("Failed to trace placeholder argument.");
				}
				int num = array[0];
				IReadOnlyList<Instruction> readOnlyList = ImmutableArray.ToImmutableArray<Instruction>(method.Body.Instructions.Skip(num).Take(index - num));
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					method.Body.Instructions.RemoveAt(num);
				}
				method.Body.Instructions.RemoveAt(num);
				index -= readOnlyList.Count;
				readOnlyList = this.PlaceholderProcessor(this.TargetModule, method, readOnlyList);
				for (int j = readOnlyList.Count - 1; j >= 0; j--)
				{
					method.Body.Instructions.Insert(num, readOnlyList[j]);
				}
				index += readOnlyList.Count;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001A52C File Offset: 0x0001872C
		private bool ReplaceCrypt(MethodDef method, Instruction instr, IMethod calledMethod, ref int index)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instr != null, "instr != null");
			Debug.Assert(calledMethod != null, "calledMethod != null");
			bool flag = calledMethod.Name == "Crypt";
			bool result;
			if (flag)
			{
				bool flag2 = this.CryptProcessor == null;
				if (flag2)
				{
					throw new InvalidOperationException("Found mutation crypt, but not processor defined.");
				}
				int num = method.Body.Instructions.IndexOf(instr);
				Instruction instruction = method.Body.Instructions[num - 2];
				Instruction instruction2 = method.Body.Instructions[num - 1];
				Debug.Assert(instruction.OpCode == OpCodes.Ldloc && instruction2.OpCode == OpCodes.Ldloc);
				method.Body.Instructions.RemoveAt(num);
				method.Body.Instructions.RemoveAt(num - 1);
				method.Body.Instructions.RemoveAt(num - 2);
				IReadOnlyList<Instruction> readOnlyList = this.CryptProcessor(this.TargetModule, method, (Local)instruction.Operand, (Local)instruction2.Operand);
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					method.Body.Instructions.Insert(num - 2 + i, readOnlyList[i]);
				}
				index += readOnlyList.Count - 3;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x040002CB RID: 715
		private const string MutationClassName = "Mutation";
	}
}
