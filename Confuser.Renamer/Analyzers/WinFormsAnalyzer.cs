using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000079 RID: 121
	public class WinFormsAnalyzer : IRenamer
	{
		// Token: 0x060002FD RID: 765 RVA: 0x00028900 File Offset: 0x00026B00
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			bool flag = def is ModuleDef;
			if (flag)
			{
				foreach (TypeDef typeDef in ((ModuleDef)def).GetTypes())
				{
					foreach (PropertyDef propertyDef in typeDef.Properties)
					{
						this.properties.AddListEntry(propertyDef.Name, propertyDef);
					}
				}
			}
			else
			{
				MethodDef methodDef = def as MethodDef;
				bool flag2 = methodDef == null || !methodDef.HasBody;
				if (!flag2)
				{
					this.AnalyzeMethod(context, service, methodDef);
				}
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x000289E0 File Offset: 0x00026BE0
		private void AnalyzeMethod(ConfuserContext context, INameService service, MethodDef method)
		{
			List<Tuple<bool, Instruction>> list = new List<Tuple<bool, Instruction>>();
			List<Instruction> list2 = new List<Instruction>();
			foreach (Instruction instruction in method.Body.Instructions)
			{
				bool flag = instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt;
				if (flag)
				{
					IMethod method2 = instruction.Operand as IMethod;
					Code code = instruction.OpCode.Code;
					Code code2 = code;
					if (code2 != Code.Call && code2 != Code.Callvirt)
					{
						if (code2 == Code.Newobj)
						{
							Debug.Assert(method2 != null);
							bool flag2 = method2.DeclaringType.FullName == "System.Windows.Forms.Binding" && method2.Name.String == ".ctor";
							if (flag2)
							{
								list.Add(Tuple.Create<bool, Instruction>(false, instruction));
							}
						}
					}
					else
					{
						Debug.Assert(method2 != null);
						bool flag3 = (method2.DeclaringType.FullName == "System.Windows.Forms.ControlBindingsCollection" || method2.DeclaringType.FullName == "System.Windows.Forms.BindingsCollection") && method2.Name == "Add" && method2.MethodSig.Params.Count != 1;
						if (flag3)
						{
							list.Add(Tuple.Create<bool, Instruction>(true, instruction));
						}
						else
						{
							bool flag4 = method2.DeclaringType.FullName == "System.Windows.Forms.DataGridViewColumn" && method2.Name == "set_DataPropertyName" && method2.MethodSig.Params.Count == 1;
							if (flag4)
							{
								list2.Add(instruction);
							}
						}
					}
				}
			}
			bool flag5 = list.Count == 0 && list2.Count == 0;
			if (!flag5)
			{
				ITraceService service2 = context.Registry.GetService<ITraceService>();
				MethodTrace methodTrace = service2.Trace(method);
				bool flag6 = false;
				foreach (Tuple<bool, Instruction> tuple in list)
				{
					int[] array = methodTrace.TraceArguments(tuple.Item2);
					bool flag7 = array == null;
					if (flag7)
					{
						bool flag8 = !flag6;
						if (flag8)
						{
							context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						flag6 = true;
					}
					else
					{
						int num = tuple.Item1 ? 1 : 0;
						Instruction instruction2 = WinFormsAnalyzer.ResolveNameInstruction(method, array, ref num);
						bool flag9 = instruction2.OpCode.Code != Code.Ldstr;
						if (flag9)
						{
							bool flag10 = !flag6;
							if (flag10)
							{
								context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							flag6 = true;
						}
						else
						{
							List<PropertyDef> list3;
							bool flag11 = !this.properties.TryGetValue((string)instruction2.Operand, out list3);
							if (flag11)
							{
								bool flag12 = !flag6;
								if (flag12)
								{
									context.Logger.WarnFormat("Failed to extract target property in '{0}'.", new object[]
									{
										method.FullName
									});
								}
								flag6 = true;
							}
							else
							{
								foreach (PropertyDef obj in list3)
								{
									service.SetCanRename(obj, false);
								}
							}
						}
						num += 2;
						Instruction instruction3 = WinFormsAnalyzer.ResolveNameInstruction(method, array, ref num);
						bool flag13 = instruction3.OpCode.Code != Code.Ldstr;
						if (flag13)
						{
							bool flag14 = !flag6;
							if (flag14)
							{
								context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							flag6 = true;
						}
						else
						{
							List<PropertyDef> list4;
							bool flag15 = !this.properties.TryGetValue((string)instruction3.Operand, out list4);
							if (flag15)
							{
								bool flag16 = !flag6;
								if (flag16)
								{
									context.Logger.WarnFormat("Failed to extract target property in '{0}'.", new object[]
									{
										method.FullName
									});
								}
								flag6 = true;
							}
							else
							{
								foreach (PropertyDef obj2 in list4)
								{
									service.SetCanRename(obj2, false);
								}
							}
						}
					}
				}
				foreach (Instruction instr in list2)
				{
					int[] array2 = methodTrace.TraceArguments(instr);
					bool flag17 = array2 == null;
					if (flag17)
					{
						bool flag18 = !flag6;
						if (flag18)
						{
							context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
							{
								method.FullName
							});
						}
						flag6 = true;
					}
					else
					{
						int num2 = 1;
						Instruction instruction4 = WinFormsAnalyzer.ResolveNameInstruction(method, array2, ref num2);
						bool flag19 = instruction4.OpCode.Code != Code.Ldstr;
						if (flag19)
						{
							bool flag20 = !flag6;
							if (flag20)
							{
								context.Logger.WarnFormat("Failed to extract binding property name in '{0}'.", new object[]
								{
									method.FullName
								});
							}
							flag6 = true;
						}
						else
						{
							List<PropertyDef> list5;
							bool flag21 = !this.properties.TryGetValue((string)instruction4.Operand, out list5);
							if (flag21)
							{
								bool flag22 = !flag6;
								if (flag22)
								{
									context.Logger.WarnFormat("Failed to extract target property in '{0}'.", new object[]
									{
										method.FullName
									});
								}
								flag6 = true;
							}
							else
							{
								foreach (PropertyDef obj3 in list5)
								{
									service.SetCanRename(obj3, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00029070 File Offset: 0x00027270
		private static Instruction ResolveNameInstruction(MethodDef method, int[] tracedArguments, ref int argumentIndex)
		{
			Instruction instruction;
			for (;;)
			{
				instruction = method.Body.Instructions[tracedArguments[argumentIndex]];
				bool flag = instruction.OpCode.Code == Code.Dup;
				if (!flag)
				{
					break;
				}
				argumentIndex++;
			}
			return instruction;
		}

		// Token: 0x04000536 RID: 1334
		private Dictionary<string, List<PropertyDef>> properties = new Dictionary<string, List<PropertyDef>>();
	}
}
