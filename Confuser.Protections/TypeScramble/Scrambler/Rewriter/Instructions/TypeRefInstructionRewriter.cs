using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x02000045 RID: 69
	internal sealed class TypeRefInstructionRewriter : InstructionRewriter<TypeRef>
	{
		// Token: 0x0600017D RID: 381 RVA: 0x000080BC File Offset: 0x000062BC
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, TypeRef operand)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(body != null, "body != null");
			Debug.Assert(operand != null, "operand != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < body.Count, "index < body.Count");
			ScannedMethod item = service.GetItem(method);
			bool flag = item != null && item.IsScambled;
			if (flag)
			{
				body[index].Operand = new TypeSpecUser(item.ConvertToGenericIfAvalible(operand.ToTypeSig(true)));
			}
		}
	}
}
