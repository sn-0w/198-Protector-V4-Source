using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x02000044 RID: 68
	internal sealed class TypeDefInstructionRewriter : InstructionRewriter<TypeDef>
	{
		// Token: 0x0600017B RID: 379 RVA: 0x00008008 File Offset: 0x00006208
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, TypeDef operand)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(body != null, "body != null");
			Debug.Assert(operand != null, "operand != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < body.Count, "index < body.Count");
			ScannedType item = service.GetItem(operand);
			bool flag = item != null && item.IsScambled;
			if (flag)
			{
				body[index].Operand = new TypeSpecUser(item.CreateGenericTypeSig(service.GetItem(method.DeclaringType)));
			}
		}
	}
}
