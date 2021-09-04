using System;
using System.Linq;
using System.Text.RegularExpressions;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000075 RID: 117
	internal class ResourceAnalyzer : IRenamer
	{
		// Token: 0x060002E1 RID: 737 RVA: 0x0002736C File Offset: 0x0002556C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDef moduleDef = def as ModuleDef;
			bool flag = moduleDef == null;
			if (!flag)
			{
				string @string = moduleDef.Assembly.Name.String;
				bool flag2 = !string.IsNullOrEmpty(moduleDef.Assembly.Culture) && @string.EndsWith(".resources");
				if (flag2)
				{
					Regex regex = new Regex(string.Format("^(.*)\\.{0}\\.resources$", moduleDef.Assembly.Culture));
					string nameAsmName = @string.Substring(0, @string.Length - ".resources".Length);
					ModuleDef moduleDef2 = context.Modules.SingleOrDefault((ModuleDefMD mod) => mod.Assembly.Name == nameAsmName);
					bool flag3 = moduleDef2 == null;
					if (flag3)
					{
						context.Logger.ErrorFormat("Could not find main assembly of satellite assembly '{0}'.", new object[]
						{
							moduleDef.Assembly.FullName
						});
						throw new ConfuserException(null);
					}
					string format = "{0}." + moduleDef.Assembly.Culture + ".resources";
					foreach (Resource resource in moduleDef.Resources)
					{
						Match match = regex.Match(resource.Name);
						bool flag4 = !match.Success;
						if (!flag4)
						{
							string value = match.Groups[1].Value;
							TypeDef typeDef = moduleDef2.FindReflection(value);
							bool flag5 = typeDef == null;
							if (flag5)
							{
								context.Logger.WarnFormat("Could not find resource type '{0}'.", new object[]
								{
									value
								});
							}
							else
							{
								service.ReduceRenameMode(typeDef, RenameMode.ASCII);
								service.AddReference<TypeDef>(typeDef, new ResourceReference(resource, typeDef, format));
							}
						}
					}
				}
				else
				{
					string format2 = "{0}.resources";
					foreach (Resource resource2 in moduleDef.Resources)
					{
						Match match2 = ResourceAnalyzer.ResourceNamePattern.Match(resource2.Name);
						bool flag6 = !match2.Success || resource2.ResourceType > ResourceType.Embedded;
						if (!flag6)
						{
							string text = match2.Groups[1].Value;
							bool flag7 = text.EndsWith(".g");
							if (!flag7)
							{
								bool flag8 = false;
								TypeDef typeDef2 = moduleDef.FindReflection(text);
								bool flag9 = typeDef2 == null;
								if (flag9)
								{
									bool flag10 = text.EndsWith(".Resources");
									if (flag10)
									{
										text = text.Substring(0, text.Length - 10) + ".My.Resources.Resources";
										typeDef2 = moduleDef.FindReflection(text);
										flag8 = (typeDef2 != null);
									}
								}
								bool flag11 = typeDef2 == null;
								if (flag11)
								{
									context.Logger.WarnFormat("Could not find resource type '{0}'.", new object[]
									{
										text
									});
								}
								else
								{
									service.ReduceRenameMode(typeDef2, RenameMode.ASCII);
									service.AddReference<TypeDef>(typeDef2, new ResourceReference(resource2, typeDef2, format2));
									bool flag12 = flag8;
									if (flag12)
									{
										ResourceAnalyzer.FindLdTokenResourceReferences(typeDef2, match2.Groups[1].Value, service);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x000276F8 File Offset: 0x000258F8
		private static void FindLdTokenResourceReferences(TypeDef type, string name, INameService service)
		{
			foreach (MethodDef method in type.Methods)
			{
				ResourceAnalyzer.FindLdTokenResourceReferences(type, method, name, service);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0002774C File Offset: 0x0002594C
		private static void FindLdTokenResourceReferences(TypeDef type, MethodDef method, string name, INameService service)
		{
			bool flag = !method.HasBody;
			if (!flag)
			{
				foreach (Instruction instruction in method.Body.Instructions)
				{
					bool flag2 = instruction.OpCode.Code == Code.Ldstr && ((string)instruction.Operand).Equals(name);
					if (flag2)
					{
						service.AddReference<TypeDef>(type, new StringTypeReference(instruction, type));
					}
				}
			}
		}

		// Token: 0x04000535 RID: 1333
		private static readonly Regex ResourceNamePattern = new Regex("^(.*)\\.resources$");
	}
}
