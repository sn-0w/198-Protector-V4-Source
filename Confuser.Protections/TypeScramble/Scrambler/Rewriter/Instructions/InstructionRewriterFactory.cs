using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x0200003F RID: 63
	internal sealed class InstructionRewriterFactory : IEnumerable<InstructionRewriter>, IEnumerable
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x0600016D RID: 365 RVA: 0x0000291F File Offset: 0x00000B1F
		private IDictionary<Type, InstructionRewriter> RewriterDefinitions { get; } = new Dictionary<Type, InstructionRewriter>();

		// Token: 0x0600016E RID: 366 RVA: 0x00002927 File Offset: 0x00000B27
		internal void Add(InstructionRewriter i)
		{
			Debug.Assert(i != null, "i != null");
			this.RewriterDefinitions.Add(i.TargetType(), i);
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00007AF0 File Offset: 0x00005CF0
		internal void Process(TypeService service, MethodDef method, IList<Instruction> instructions, ref int index)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instructions != null, "instructions != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < instructions.Count, "index < instructions.Count");
			Instruction instruction = instructions[index];
			bool flag = instruction.Operand == null;
			if (!flag)
			{
				Type type = instruction.Operand.GetType();
				while (type != typeof(object))
				{
					InstructionRewriter instructionRewriter;
					bool flag2 = this.RewriterDefinitions.TryGetValue(type, out instructionRewriter);
					if (flag2)
					{
						instructionRewriter.ProcessInstruction(service, method, instructions, ref index, instruction);
						break;
					}
					type = type.BaseType;
				}
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000294C File Offset: 0x00000B4C
		public IEnumerator<InstructionRewriter> GetEnumerator()
		{
			return this.RewriterDefinitions.Values.GetEnumerator();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000295E File Offset: 0x00000B5E
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
