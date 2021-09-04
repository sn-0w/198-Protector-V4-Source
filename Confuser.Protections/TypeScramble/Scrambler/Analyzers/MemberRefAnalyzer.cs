using System;
using System.Diagnostics;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Analyzers
{
	// Token: 0x0200004A RID: 74
	internal sealed class MemberRefAnalyzer : ContextAnalyzer<MemberRef>
	{
		// Token: 0x060001A3 RID: 419 RVA: 0x00008D78 File Offset: 0x00006F78
		internal override void Process(ScannedMethod method, Instruction instruction, MemberRef operand)
		{
			Debug.Assert(method != null, "method != null");
			Debug.Assert(instruction != null, "instruction != null");
			Debug.Assert(operand != null, "operand != null");
			bool flag = instruction.OpCode != OpCodes.Newobj;
			if (!flag)
			{
				bool flag2 = operand.MethodSig.Params.Count > 0;
				if (!flag2)
				{
					TypeSig typeSig = null;
					IMemberRefParent @class = operand.Class;
					TypeRef typeRef = @class as TypeRef;
					bool flag3 = typeRef != null;
					if (flag3)
					{
						typeSig = typeRef.ToTypeSig(true);
					}
					IMemberRefParent class2 = operand.Class;
					TypeSpec typeSpec = class2 as TypeSpec;
					bool flag4 = typeSpec != null;
					if (flag4)
					{
						typeSig = typeSpec.ToTypeSig(true);
					}
					bool flag5 = typeSig != null;
					if (flag5)
					{
						method.RegisterGeneric(typeSig);
					}
				}
			}
		}
	}
}
