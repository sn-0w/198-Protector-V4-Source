using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Core;
using Confuser.Renamer.References;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000076 RID: 118
	internal class TypeBlobAnalyzer : IRenamer
	{
		// Token: 0x060002E8 RID: 744 RVA: 0x000277F8 File Offset: 0x000259F8
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			ModuleDefMD module = def as ModuleDefMD;
			bool flag = module == null;
			if (!flag)
			{
				MDTable mdtable = module.TablesStream.Get(Table.Method);
				uint rows = mdtable.Rows;
				IEnumerable<MethodDef> enumerable = module.GetTypes().SelectMany((TypeDef type) => type.Methods);
				foreach (MethodDef methodDef in enumerable)
				{
					foreach (MethodOverride methodOverride in methodDef.Overrides)
					{
						bool flag2 = methodOverride.MethodBody is MemberRef;
						if (flag2)
						{
							this.AnalyzeMemberRef(context, service, (MemberRef)methodOverride.MethodBody);
						}
						bool flag3 = methodOverride.MethodDeclaration is MemberRef;
						if (flag3)
						{
							this.AnalyzeMemberRef(context, service, (MemberRef)methodOverride.MethodDeclaration);
						}
					}
					bool flag4 = !methodDef.HasBody;
					if (!flag4)
					{
						foreach (Instruction instruction in methodDef.Body.Instructions)
						{
							bool flag5 = instruction.Operand is MemberRef;
							if (flag5)
							{
								this.AnalyzeMemberRef(context, service, (MemberRef)instruction.Operand);
							}
							else
							{
								bool flag6 = instruction.Operand is MethodSpec;
								if (flag6)
								{
									MethodSpec methodSpec = (MethodSpec)instruction.Operand;
									bool flag7 = methodSpec.Method is MemberRef;
									if (flag7)
									{
										this.AnalyzeMemberRef(context, service, (MemberRef)methodSpec.Method);
									}
								}
							}
						}
					}
				}
				mdtable = module.TablesStream.Get(Table.CustomAttribute);
				rows = mdtable.Rows;
				IEnumerable<CustomAttribute> enumerable2 = (from a in Enumerable.Range(1, (int)rows).Select(delegate(int rid)
				{
					RawCustomAttributeRow rawCustomAttributeRow;
					bool flag12 = module.TablesStream.TryReadCustomAttributeRow((uint)rid, out rawCustomAttributeRow);
					IHasCustomAttribute result;
					if (flag12)
					{
						result = module.ResolveHasCustomAttribute(rawCustomAttributeRow.Parent);
					}
					else
					{
						result = null;
					}
					return result;
				})
				where a != null
				select a).Distinct<IHasCustomAttribute>().SelectMany((IHasCustomAttribute owner) => owner.CustomAttributes);
				foreach (CustomAttribute customAttribute in enumerable2)
				{
					bool flag8 = customAttribute.Constructor is MemberRef;
					if (flag8)
					{
						this.AnalyzeMemberRef(context, service, (MemberRef)customAttribute.Constructor);
					}
					foreach (CAArgument arg in customAttribute.ConstructorArguments)
					{
						this.AnalyzeCAArgument(context, service, arg);
					}
					foreach (CANamedArgument canamedArgument in customAttribute.Fields)
					{
						this.AnalyzeCAArgument(context, service, canamedArgument.Argument);
					}
					foreach (CANamedArgument canamedArgument2 in customAttribute.Properties)
					{
						this.AnalyzeCAArgument(context, service, canamedArgument2.Argument);
					}
					TypeDef typeDef = customAttribute.AttributeType.ResolveTypeDefThrow();
					bool flag9 = !context.Modules.Contains((ModuleDefMD)typeDef.Module);
					if (!flag9)
					{
						foreach (CANamedArgument canamedArgument3 in customAttribute.Fields)
						{
							FieldDef fieldDef = typeDef.FindField(canamedArgument3.Name, new FieldSig(canamedArgument3.Type));
							bool flag10 = fieldDef == null;
							if (flag10)
							{
								context.Logger.WarnFormat("Failed to resolve CA field '{0}::{1} : {2}'.", new object[]
								{
									typeDef,
									canamedArgument3.Name,
									canamedArgument3.Type
								});
							}
							else
							{
								service.AddReference<IDnlibDef>(fieldDef, new CAMemberReference(canamedArgument3, fieldDef));
							}
						}
						foreach (CANamedArgument canamedArgument4 in customAttribute.Properties)
						{
							PropertyDef propertyDef = typeDef.FindProperty(canamedArgument4.Name, new PropertySig(true, canamedArgument4.Type));
							bool flag11 = propertyDef == null;
							if (flag11)
							{
								context.Logger.WarnFormat("Failed to resolve CA property '{0}::{1} : {2}'.", new object[]
								{
									typeDef,
									canamedArgument4.Name,
									canamedArgument4.Type
								});
							}
							else
							{
								service.AddReference<IDnlibDef>(propertyDef, new CAMemberReference(canamedArgument4, propertyDef));
							}
						}
					}
				}
			}
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00027DF0 File Offset: 0x00025FF0
		private void AnalyzeCAArgument(ConfuserContext context, INameService service, CAArgument arg)
		{
			bool flag = arg.Value == null;
			if (!flag)
			{
				bool flag2 = arg.Type.DefinitionAssembly.IsCorLib() && arg.Type.FullName == "System.Type";
				if (flag2)
				{
					TypeSig typeSig = (TypeSig)arg.Value;
					foreach (ITypeDefOrRef typeDefOrRef in typeSig.FindTypeRefs())
					{
						TypeDef typeDef = typeDefOrRef.ResolveTypeDefThrow();
						bool flag3 = context.Modules.Contains((ModuleDefMD)typeDef.Module);
						if (flag3)
						{
							bool flag4 = typeDefOrRef is TypeRef;
							if (flag4)
							{
								service.AddReference<TypeDef>(typeDef, new TypeRefReference((TypeRef)typeDefOrRef, typeDef));
							}
							service.ReduceRenameMode(typeDef, RenameMode.ASCII);
						}
					}
				}
				else
				{
					bool flag5 = arg.Value is CAArgument[];
					if (flag5)
					{
						foreach (CAArgument arg2 in (CAArgument[])arg.Value)
						{
							this.AnalyzeCAArgument(context, service, arg2);
						}
					}
				}
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00027F3C File Offset: 0x0002613C
		private void AnalyzeMemberRef(ConfuserContext context, INameService service, MemberRef memberRef)
		{
			ITypeDefOrRef declaringType = memberRef.DeclaringType;
			TypeSpec typeSpec = declaringType as TypeSpec;
			bool flag = typeSpec == null || typeSpec.TypeSig.IsArray || typeSpec.TypeSig.IsSZArray;
			if (!flag)
			{
				TypeSig typeSig = typeSpec.TypeSig;
				while (typeSig.Next != null)
				{
					typeSig = typeSig.Next;
				}
				Debug.Assert(typeSig is TypeDefOrRefSig || typeSig is GenericInstSig || typeSig is GenericSig);
				bool flag2 = typeSig is GenericInstSig;
				if (flag2)
				{
					GenericInstSig genericInstSig = (GenericInstSig)typeSig;
					Debug.Assert(!(genericInstSig.GenericType.TypeDefOrRef is TypeSpec));
					TypeDef typeDef = genericInstSig.GenericType.TypeDefOrRef.ResolveTypeDefThrow();
					bool flag3 = !context.Modules.Contains((ModuleDefMD)typeDef.Module) || memberRef.IsArrayAccessors();
					if (!flag3)
					{
						bool isFieldRef = memberRef.IsFieldRef;
						IDnlibDef dnlibDef;
						if (isFieldRef)
						{
							dnlibDef = memberRef.ResolveFieldThrow();
						}
						else
						{
							bool isMethodRef = memberRef.IsMethodRef;
							if (!isMethodRef)
							{
								throw new UnreachableException();
							}
							dnlibDef = memberRef.ResolveMethodThrow();
						}
						service.AddReference<IDnlibDef>(dnlibDef, new MemberRefReference(memberRef, dnlibDef));
					}
				}
			}
		}
	}
}
