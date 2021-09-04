using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x02000042 RID: 66
	internal sealed class MethodSpecInstructionRewriter : InstructionRewriter<MethodSpec>
	{
		// Token: 0x06000177 RID: 375 RVA: 0x00007EDC File Offset: 0x000060DC
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, MethodSpec operand)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(body != null, "body != null");
			Debug.Assert(operand != null, "operand != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < body.Count, "index < body.Count");
			ScannedMethod current = service.GetItem(method);
			IMethodDefOrRef method2 = operand.Method;
			MethodDef methodDef = method2 as MethodDef;
			bool flag = methodDef != null;
			if (flag)
			{
				ScannedMethod item = service.GetItem(methodDef);
				bool flag2 = item != null && item.IsScambled;
				if (flag2)
				{
					operand.GenericInstMethodSig = item.CreateGenericMethodSig(current, operand.GenericInstMethodSig);
				}
			}
			else
			{
				ScannedMethod current2 = current;
				bool flag3 = current2 != null && current2.IsScambled;
				if (flag3)
				{
					IEnumerable<TypeSig> source = from x in operand.GenericInstMethodSig.GenericArguments
					select current.ConvertToGenericIfAvalible(x);
					operand.GenericInstMethodSig = new GenericInstMethodSig(source.ToArray<TypeSig>());
				}
			}
		}
	}
}
