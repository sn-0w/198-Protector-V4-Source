using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Core.Helpers
{
	// Token: 0x020000AE RID: 174
	public static class MutationHelper
	{
		// Token: 0x06000403 RID: 1027 RVA: 0x00017EF0 File Offset: 0x000160F0
		public static void InjectKey(MethodDef method, int keyId, int key)
		{
			foreach (Instruction instruction in method.Body.Instructions)
			{
				bool flag = instruction.OpCode == OpCodes.Ldsfld;
				if (flag)
				{
					IField field = (IField)instruction.Operand;
					int num;
					bool flag2 = field.DeclaringType.FullName == "Mutation" && MutationHelper.field2index.TryGetValue(field.Name, out num) && num == keyId;
					if (flag2)
					{
						instruction.OpCode = OpCodes.Ldc_I4;
						instruction.Operand = key;
					}
				}
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00017FBC File Offset: 0x000161BC
		public static void InjectKeys(MethodDef method, int[] keyIds, int[] keys)
		{
			foreach (Instruction instruction in method.Body.Instructions)
			{
				bool flag = instruction.OpCode == OpCodes.Ldsfld;
				if (flag)
				{
					IField field = (IField)instruction.Operand;
					int num;
					bool flag2 = field.DeclaringType.FullName == "Mutation" && MutationHelper.field2index.TryGetValue(field.Name, out num) && (num = Array.IndexOf<int>(keyIds, num)) != -1;
					if (flag2)
					{
						instruction.OpCode = OpCodes.Ldc_I4;
						instruction.Operand = keys[num];
					}
				}
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00018098 File Offset: 0x00016298
		public static void ReplacePlaceholder(MethodDef method, Func<Instruction[], Instruction[]> repl)
		{
			MethodTrace methodTrace = new MethodTrace(method).Trace();
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				bool flag = instruction.OpCode == OpCodes.Call;
				if (flag)
				{
					IMethod method2 = (IMethod)instruction.Operand;
					bool flag2 = method2.DeclaringType.FullName == "Mutation" && method2.Name == "Placeholder";
					if (flag2)
					{
						int[] array = methodTrace.TraceArguments(instruction);
						bool flag3 = array == null;
						if (flag3)
						{
							throw new ArgumentException("Failed to trace placeholder argument.");
						}
						int num = array[0];
						Instruction[] array2 = method.Body.Instructions.Skip(num).Take(i - num).ToArray<Instruction>();
						for (int j = 0; j < array2.Length; j++)
						{
							method.Body.Instructions.RemoveAt(num);
						}
						method.Body.Instructions.RemoveAt(num);
						array2 = repl(array2);
						for (int k = array2.Length - 1; k >= 0; k--)
						{
							method.Body.Instructions.Insert(num, array2[k]);
						}
						break;
					}
				}
			}
		}

		// Token: 0x0400029A RID: 666
		private const string mutationType = "Mutation";

		// Token: 0x0400029B RID: 667
		private static readonly Dictionary<string, int> field2index = new Dictionary<string, int>
		{
			{
				"KeyI0",
				0
			},
			{
				"KeyI1",
				1
			},
			{
				"KeyI2",
				2
			},
			{
				"KeyI3",
				3
			},
			{
				"KeyI4",
				4
			},
			{
				"KeyI5",
				5
			},
			{
				"KeyI6",
				6
			},
			{
				"KeyI7",
				7
			},
			{
				"KeyI8",
				8
			},
			{
				"KeyI9",
				9
			},
			{
				"KeyI10",
				10
			},
			{
				"KeyI11",
				11
			},
			{
				"KeyI12",
				12
			},
			{
				"KeyI13",
				13
			},
			{
				"KeyI14",
				14
			},
			{
				"KeyI15",
				15
			}
		};
	}
}
