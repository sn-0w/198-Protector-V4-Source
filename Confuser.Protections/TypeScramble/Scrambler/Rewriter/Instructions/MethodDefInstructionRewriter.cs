using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x02000041 RID: 65
	internal sealed class MethodDefInstructionRewriter : InstructionRewriter<MethodDef>
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00007DF4 File Offset: 0x00005FF4
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, MethodDef operand)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(body != null, "body != null");
			Debug.Assert(operand != null, "operand != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < body.Count, "index < body.Count");
			ScannedMethod item = service.GetItem(operand);
			bool flag = item != null && item.IsScambled;
			if (flag)
			{
				ScannedMethod item2 = service.GetItem(method);
				MethodSpecUser methodSpecUser = new MethodSpecUser(item.TargetMethod, item.CreateGenericMethodSig(item2, null));
				Debug.Assert(methodSpecUser.GenericInstMethodSig.GenericArguments.Count == item.TargetMethod.GenericParameters.Count, "newSpec.GenericInstMethodSig.GenericArguments.Count == targetMethod.TargetMethod.GenericParameters.Count");
				body[index].Operand = methodSpecUser;
			}
		}
	}
}
