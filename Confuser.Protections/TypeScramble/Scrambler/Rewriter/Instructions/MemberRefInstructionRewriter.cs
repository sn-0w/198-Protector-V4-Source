using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions
{
	// Token: 0x02000040 RID: 64
	internal sealed class MemberRefInstructionRewriter : InstructionRewriter<MemberRef>
	{
		// Token: 0x06000173 RID: 371 RVA: 0x00007BC0 File Offset: 0x00005DC0
		internal override void ProcessOperand(TypeService service, MethodDef method, IList<Instruction> body, ref int index, MemberRef operand)
		{
			Debug.Assert(service != null, "service != null");
			Debug.Assert(method != null, "method != null");
			Debug.Assert(body != null, "body != null");
			Debug.Assert(operand != null, "operand != null");
			Debug.Assert(index >= 0, "index >= 0");
			Debug.Assert(index < body.Count, "index < body.Count");
			ScannedMethod item = service.GetItem(method);
			bool flag = operand.MethodSig == null;
			if (!flag)
			{
				bool flag2 = operand.MethodSig.Params.Count > 0 || body[index].OpCode != OpCodes.Newobj;
				if (!flag2)
				{
					ModuleDef module = method.Module;
					MethodInfo method2 = typeof(Type).GetMethod("GetTypeFromHandle");
					MethodInfo method3 = typeof(Activator).GetMethod("CreateInstance", new Type[]
					{
						typeof(Type)
					});
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
						int count = operand.MethodSig.Params.Count;
						body[index].OpCode = OpCodes.Ldtoken;
						GenericMVar genericMVar = (item != null) ? item.GetGeneric(typeSig) : null;
						bool flag6 = genericMVar != null;
						TypeSpecUser operand2;
						if (flag6)
						{
							operand2 = new TypeSpecUser(new GenericMVar(genericMVar.Number));
						}
						else
						{
							operand2 = new TypeSpecUser(typeSig);
						}
						body[index].Operand = operand2;
						int num = index + 1;
						index = num;
						body.Insert(num, Instruction.Create(OpCodes.Call, module.Import(method2)));
						num = index + 1;
						index = num;
						body.Insert(num, Instruction.Create(OpCodes.Call, module.Import(method3)));
					}
				}
			}
		}
	}
}
