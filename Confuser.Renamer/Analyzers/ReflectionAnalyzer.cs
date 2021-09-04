using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000074 RID: 116
	public sealed class ReflectionAnalyzer : IRenamer
	{
		// Token: 0x060002D9 RID: 729 RVA: 0x00026E5C File Offset: 0x0002505C
		void IRenamer.Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			MethodDef methodDef = def as MethodDef;
			bool flag = methodDef == null || !methodDef.HasBody;
			if (!flag)
			{
				this.Analyze(service, context.Registry.GetService<ITraceService>(), context.Modules.Cast<ModuleDef>().ToArray<ModuleDef>(), methodDef);
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00026EAC File Offset: 0x000250AC
		public void Analyze(INameService nameService, ITraceService traceService, IReadOnlyList<ModuleDef> moduleDefs, MethodDef method)
		{
			ReflectionAnalyzer.<>c__DisplayClass1_0 CS$<>8__locals1;
			CS$<>8__locals1.traceService = traceService;
			CS$<>8__locals1.method = method;
			CS$<>8__locals1.methodTrace = null;
			foreach (Instruction instruction in CS$<>8__locals1.method.Body.Instructions)
			{
				IMethodDefOrRef methodDefOrRef;
				bool flag;
				if (instruction.OpCode.Code == Code.Call)
				{
					object operand = instruction.Operand;
					methodDefOrRef = (operand as IMethodDefOrRef);
					flag = (methodDefOrRef != null);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					bool flag3 = methodDefOrRef.DeclaringType.FullName == "System.Type";
					if (flag3)
					{
						Func<TypeDef, IEnumerable<IMemberDef>> func = null;
						bool flag4 = methodDefOrRef.Name == "GetMethod";
						if (flag4)
						{
							func = ((TypeDef t) => t.Methods);
						}
						else
						{
							bool flag5 = methodDefOrRef.Name == "GetField";
							if (flag5)
							{
								func = ((TypeDef t) => t.Fields);
							}
							else
							{
								bool flag6 = methodDefOrRef.Name == "GetProperty";
								if (flag6)
								{
									func = ((TypeDef t) => t.Properties);
								}
								else
								{
									bool flag7 = methodDefOrRef.Name == "GetEvent";
									if (flag7)
									{
										func = ((TypeDef t) => t.Events);
									}
									else
									{
										bool flag8 = methodDefOrRef.Name == "GetMember";
										if (flag8)
										{
											func = ((TypeDef t) => Enumerable.Empty<IMemberDef>().Concat(t.Methods).Concat(t.Fields).Concat(t.Properties).Concat(t.Events));
										}
									}
								}
							}
						}
						bool flag9 = func != null;
						if (flag9)
						{
							MethodTrace methodTrace = ReflectionAnalyzer.<Analyze>g__GetMethodTrace|1_0(ref CS$<>8__locals1);
							int[] array = methodTrace.TraceArguments(instruction);
							bool flag10 = array.Length >= 2;
							if (flag10)
							{
								IReadOnlyList<TypeDef> source = ReflectionAnalyzer.GetReferencedTypes(CS$<>8__locals1.method.Body.Instructions[array[0]], CS$<>8__locals1.method, methodTrace);
								IReadOnlyList<UTF8String> names = ReflectionAnalyzer.GetReferencedNames(CS$<>8__locals1.method.Body.Instructions[array[1]]);
								bool flag11 = !source.Any<TypeDef>();
								if (flag11)
								{
									source = moduleDefs.SelectMany((ModuleDef m) => m.GetTypes()).ToArray<TypeDef>();
								}
								IEnumerable<IMemberDef> source2 = source.SelectMany(func);
								Func<IMemberDef, bool> predicate;
								Func<IMemberDef, bool> <>9__7;
								if ((predicate = <>9__7) == null)
								{
									predicate = (<>9__7 = ((IMemberDef m) => names.Contains(m.Name)));
								}
								foreach (IMemberDef obj in source2.Where(predicate))
								{
									nameService.SetCanRename(obj, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x000271EC File Offset: 0x000253EC
		private static IReadOnlyList<TypeDef> GetReferencedTypes(Instruction instruction, MethodDef method, MethodTrace trace)
		{
			IMethodDefOrRef methodDefOrRef;
			bool flag;
			if (instruction.OpCode.Code == Code.Call)
			{
				object operand = instruction.Operand;
				methodDefOrRef = (operand as IMethodDefOrRef);
				flag = (methodDefOrRef != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				bool flag3 = methodDefOrRef.DeclaringType.FullName == "System.Type" && methodDefOrRef.Name == "GetTypeFromHandle";
				if (flag3)
				{
					int[] array = trace.TraceArguments(instruction);
					bool flag4 = array.Length == 1;
					if (flag4)
					{
						Instruction instruction2 = method.Body.Instructions[array[0]];
						TypeDef typeDef;
						bool flag5;
						if (instruction2.OpCode.Code == Code.Ldtoken)
						{
							object operand2 = instruction2.Operand;
							typeDef = (operand2 as TypeDef);
							flag5 = (typeDef != null);
						}
						else
						{
							flag5 = false;
						}
						bool flag6 = flag5;
						if (flag6)
						{
							return new List<TypeDef>
							{
								typeDef
							};
						}
					}
				}
			}
			return new List<TypeDef>();
		}

		// Token: 0x060002DC RID: 732 RVA: 0x000272D8 File Offset: 0x000254D8
		private static IReadOnlyList<UTF8String> GetReferencedNames(Instruction instruction)
		{
			string text;
			bool flag;
			if (instruction.OpCode.Code == Code.Ldstr)
			{
				object operand = instruction.Operand;
				text = (operand as string);
				flag = (text != null);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			IReadOnlyList<UTF8String> result;
			if (flag2)
			{
				result = new List<UTF8String>
				{
					text
				};
			}
			else
			{
				result = new List<UTF8String>();
			}
			return result;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		void IRenamer.PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		void IRenamer.PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00027330 File Offset: 0x00025530
		[CompilerGenerated]
		internal static MethodTrace <Analyze>g__GetMethodTrace|1_0(ref ReflectionAnalyzer.<>c__DisplayClass1_0 A_0)
		{
			bool flag = A_0.methodTrace == null;
			if (flag)
			{
				A_0.methodTrace = A_0.traceService.Trace(A_0.method);
			}
			return A_0.methodTrace;
		}
	}
}
