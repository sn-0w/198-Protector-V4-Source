using System;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000072 RID: 114
	internal class LdtokenEnumAnalyzer : IRenamer
	{
		// Token: 0x060002C9 RID: 713 RVA: 0x00026388 File Offset: 0x00024588
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef methodDef = def as MethodDef;
			bool flag = methodDef == null || !methodDef.HasBody;
			if (!flag)
			{
				for (int i = 0; i < methodDef.Body.Instructions.Count; i++)
				{
					Instruction instruction = methodDef.Body.Instructions[i];
					bool flag2 = instruction.OpCode.Code == Code.Ldtoken;
					if (flag2)
					{
						bool flag3 = instruction.Operand is MemberRef;
						if (flag3)
						{
							IMemberForwarded memberForwarded = ((MemberRef)instruction.Operand).ResolveThrow();
							bool flag4 = context.Modules.Contains((ModuleDefMD)memberForwarded.Module);
							if (flag4)
							{
								service.SetCanRename(memberForwarded, false);
							}
						}
						else
						{
							bool flag5 = instruction.Operand is IField;
							if (flag5)
							{
								FieldDef fieldDef = ((IField)instruction.Operand).ResolveThrow();
								bool flag6 = context.Modules.Contains((ModuleDefMD)fieldDef.Module);
								if (flag6)
								{
									service.SetCanRename(fieldDef, false);
								}
							}
							else
							{
								bool flag7 = instruction.Operand is IMethod;
								if (flag7)
								{
									IMethod method = (IMethod)instruction.Operand;
									bool flag8 = !method.IsArrayAccessors();
									if (flag8)
									{
										MethodDef methodDef2 = method.ResolveThrow();
										bool flag9 = context.Modules.Contains((ModuleDefMD)methodDef2.Module);
										if (flag9)
										{
											service.SetCanRename(methodDef, false);
										}
									}
								}
								else
								{
									bool flag10 = instruction.Operand is ITypeDefOrRef;
									if (!flag10)
									{
										throw new UnreachableException();
									}
									bool flag11 = !(instruction.Operand is TypeSpec);
									if (flag11)
									{
										TypeDef typeDef = ((ITypeDefOrRef)instruction.Operand).ResolveTypeDefThrow();
										bool flag12 = context.Modules.Contains((ModuleDefMD)typeDef.Module) && this.HandleTypeOf(context, service, methodDef, i);
										if (flag12)
										{
											TypeDef typeDef2 = typeDef;
											do
											{
												this.DisableRename(service, typeDef2, false);
												typeDef2 = typeDef2.DeclaringType;
											}
											while (typeDef2 != null);
										}
									}
								}
							}
						}
					}
					else
					{
						bool flag13 = (instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt) && ((IMethod)instruction.Operand).Name == "ToString";
						if (flag13)
						{
							this.HandleEnum(context, service, methodDef, i);
						}
						else
						{
							bool flag14 = instruction.OpCode.Code == Code.Ldstr;
							if (flag14)
							{
								TypeDef typeDef3 = methodDef.Module.FindReflection((string)instruction.Operand);
								bool flag15 = typeDef3 != null;
								if (flag15)
								{
									service.AddReference<TypeDef>(typeDef3, new StringTypeReference(instruction, typeDef3));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002CC RID: 716 RVA: 0x00026660 File Offset: 0x00024860
		private void HandleEnum(ConfuserContext context, INameService service, MethodDef method, int index)
		{
			IMethod method2 = (IMethod)method.Body.Instructions[index].Operand;
			bool flag = method2.FullName == "System.String System.Object::ToString()" || method2.FullName == "System.String System.Enum::ToString(System.String)";
			if (flag)
			{
				int num = index - 1;
				while (num >= 0 && method.Body.Instructions[num].OpCode.Code == Code.Nop)
				{
					num--;
				}
				bool flag2 = num < 0;
				if (!flag2)
				{
					Instruction instruction = method.Body.Instructions[num];
					bool flag3 = instruction.Operand is MemberRef;
					TypeSig typeSig;
					if (flag3)
					{
						MemberRef memberRef = (MemberRef)instruction.Operand;
						typeSig = (memberRef.IsFieldRef ? memberRef.FieldSig.Type : memberRef.MethodSig.RetType);
					}
					else
					{
						bool flag4 = instruction.Operand is IField;
						if (flag4)
						{
							typeSig = ((IField)instruction.Operand).FieldSig.Type;
						}
						else
						{
							bool flag5 = instruction.Operand is IMethod;
							if (flag5)
							{
								typeSig = ((IMethod)instruction.Operand).MethodSig.RetType;
							}
							else
							{
								bool flag6 = instruction.Operand is ITypeDefOrRef;
								if (flag6)
								{
									typeSig = ((ITypeDefOrRef)instruction.Operand).ToTypeSig(true);
								}
								else
								{
									bool flag7 = instruction.GetParameter(method.Parameters) != null;
									if (flag7)
									{
										typeSig = instruction.GetParameter(method.Parameters).Type;
									}
									else
									{
										bool flag8 = instruction.GetLocal(method.Body.Variables) != null;
										if (!flag8)
										{
											return;
										}
										typeSig = instruction.GetLocal(method.Body.Variables).Type;
									}
								}
							}
						}
					}
					ITypeDefOrRef typeDefOrRef = typeSig.ToBasicTypeDefOrRef();
					bool flag9 = typeDefOrRef == null;
					if (!flag9)
					{
						TypeDef typeDef = typeDefOrRef.ResolveTypeDefThrow();
						bool flag10 = typeDef != null && typeDef.IsEnum && context.Modules.Contains((ModuleDefMD)typeDef.Module);
						if (flag10)
						{
							this.DisableRename(service, typeDef, true);
						}
					}
				}
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x000268A0 File Offset: 0x00024AA0
		private bool HandleTypeOf(ConfuserContext context, INameService service, MethodDef method, int index)
		{
			bool flag = index + 1 >= method.Body.Instructions.Count;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				IMethod method2 = method.Body.Instructions[index + 1].Operand as IMethod;
				bool flag2 = method2 == null || method2.FullName != "System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)";
				if (flag2)
				{
					result = true;
				}
				else
				{
					bool flag3 = index + 2 < method.Body.Instructions.Count;
					if (flag3)
					{
						Instruction instruction = method.Body.Instructions[index + 2];
						IMethod method3 = instruction.Operand as IMethod;
						bool flag4 = instruction.OpCode == OpCodes.Newobj && method3.FullName == "System.Void System.ComponentModel.ComponentResourceManager::.ctor(System.Type)";
						if (flag4)
						{
							return false;
						}
						bool flag5 = instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt;
						if (flag5)
						{
							string fullName = method3.DeclaringType.FullName;
							string text = fullName;
							if (text != null)
							{
								if (text == "System.Runtime.InteropServices.Marshal")
								{
									return false;
								}
								if (!(text == "System.Type"))
								{
									if (text == "System.Reflection.MemberInfo")
									{
										return method3.Name == "get_Name";
									}
									if (text == "System.Object")
									{
										return method3.Name == "ToString";
									}
								}
								else
								{
									bool flag6 = method3.Name.StartsWith("Get") || method3.Name == "InvokeMember";
									if (flag6)
									{
										return true;
									}
									return method3.Name == "get_AssemblyQualifiedName" || method3.Name == "get_FullName" || method3.Name == "get_Namespace";
								}
							}
						}
					}
					bool flag7 = index + 3 < method.Body.Instructions.Count;
					if (flag7)
					{
						Instruction instruction2 = method.Body.Instructions[index + 3];
						IMethod method4 = instruction2.Operand as IMethod;
						bool flag8 = instruction2.OpCode == OpCodes.Call || instruction2.OpCode == OpCodes.Callvirt;
						if (flag8)
						{
							string fullName2 = method4.DeclaringType.FullName;
							string text2 = fullName2;
							if (text2 != null)
							{
								if (text2 == "System.Runtime.InteropServices.Marshal")
								{
									return false;
								}
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00026B5C File Offset: 0x00024D5C
		private void DisableRename(INameService service, TypeDef typeDef, bool memberOnly = true)
		{
			service.SetCanRename(typeDef, false);
			foreach (MethodDef obj in typeDef.Methods)
			{
				service.SetCanRename(obj, false);
			}
			foreach (FieldDef obj2 in typeDef.Fields)
			{
				service.SetCanRename(obj2, false);
			}
			foreach (PropertyDef obj3 in typeDef.Properties)
			{
				service.SetCanRename(obj3, false);
			}
			foreach (EventDef obj4 in typeDef.Events)
			{
				service.SetCanRename(obj4, false);
			}
			foreach (TypeDef typeDef2 in typeDef.NestedTypes)
			{
				this.DisableRename(service, typeDef2, false);
			}
		}
	}
}
