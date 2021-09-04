using System;
using System.Collections.Generic;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000071 RID: 113
	internal class JsonAnalyzer : IRenamer
	{
		// Token: 0x060002BF RID: 703 RVA: 0x00025F20 File Offset: 0x00024120
		private static CustomAttribute GetJsonContainerAttribute(IHasCustomAttribute attrs)
		{
			foreach (CustomAttribute customAttribute in attrs.CustomAttributes)
			{
				bool flag = JsonAnalyzer.JsonContainers.Contains(customAttribute.TypeFullName);
				if (flag)
				{
					return customAttribute;
				}
			}
			return null;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00025F90 File Offset: 0x00024190
		private static bool ShouldExclude(TypeDef type, IDnlibDef def)
		{
			bool flag = def.CustomAttributes.IsDefined("Newtonsoft.Json.JsonPropertyAttribute");
			CustomAttribute customAttribute;
			if (flag)
			{
				customAttribute = def.CustomAttributes.Find("Newtonsoft.Json.JsonPropertyAttribute");
				bool flag2 = customAttribute.HasConstructorArguments || customAttribute.GetProperty("PropertyName") != null;
				if (flag2)
				{
					return false;
				}
			}
			customAttribute = JsonAnalyzer.GetJsonContainerAttribute(type);
			bool flag3 = customAttribute == null || customAttribute.TypeFullName != "Newtonsoft.Json.JsonObjectAttribute";
			bool result;
			if (flag3)
			{
				result = false;
			}
			else
			{
				bool flag4 = def.CustomAttributes.IsDefined("Newtonsoft.Json.JsonIgnoreAttribute");
				if (flag4)
				{
					result = false;
				}
				else
				{
					int num = 0;
					bool flag5 = customAttribute.HasConstructorArguments && customAttribute.ConstructorArguments[0].Type.FullName == "Newtonsoft.Json.MemberSerialization";
					if (flag5)
					{
						num = (int)customAttribute.ConstructorArguments[0].Value;
					}
					else
					{
						foreach (CANamedArgument canamedArgument in customAttribute.Properties)
						{
							bool flag6 = canamedArgument.Name == "MemberSerialization";
							if (flag6)
							{
								num = (int)canamedArgument.Value;
							}
						}
					}
					bool flag7 = num == 0;
					if (flag7)
					{
						result = ((def is PropertyDef && ((PropertyDef)def).IsPublic()) || (def is FieldDef && ((FieldDef)def).IsPublic));
					}
					else
					{
						bool flag8 = num == 1;
						if (flag8)
						{
							result = false;
						}
						else
						{
							bool flag9 = num == 2;
							result = (flag9 && def is FieldDef);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0002615C File Offset: 0x0002435C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			bool flag = def is TypeDef;
			if (flag)
			{
				this.Analyze(context, service, (TypeDef)def, parameters);
			}
			else
			{
				bool flag2 = def is MethodDef;
				if (flag2)
				{
					this.Analyze(context, service, (MethodDef)def, parameters);
				}
				else
				{
					bool flag3 = def is PropertyDef;
					if (flag3)
					{
						this.Analyze(context, service, (PropertyDef)def, parameters);
					}
					else
					{
						bool flag4 = def is FieldDef;
						if (flag4)
						{
							this.Analyze(context, service, (FieldDef)def, parameters);
						}
					}
				}
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000261EC File Offset: 0x000243EC
		private void Analyze(ConfuserContext context, INameService service, TypeDef type, ProtectionParameters parameters)
		{
			CustomAttribute jsonContainerAttribute = JsonAnalyzer.GetJsonContainerAttribute(type);
			bool flag = jsonContainerAttribute == null;
			if (!flag)
			{
				bool flag2 = false;
				bool flag3 = jsonContainerAttribute.HasConstructorArguments && jsonContainerAttribute.ConstructorArguments[0].Type.FullName == "System.String";
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					foreach (CANamedArgument canamedArgument in jsonContainerAttribute.Properties)
					{
						bool flag4 = canamedArgument.Name == "Id";
						if (flag4)
						{
							flag2 = true;
						}
					}
				}
				bool flag5 = !flag2;
				if (flag5)
				{
					service.SetCanRename(type, false);
				}
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x000262B8 File Offset: 0x000244B8
		private void Analyze(ConfuserContext context, INameService service, MethodDef method, ProtectionParameters parameters)
		{
			bool flag = JsonAnalyzer.GetJsonContainerAttribute(method.DeclaringType) != null && method.IsConstructor;
			if (flag)
			{
				service.SetParam(method, "renameArgs", "false");
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x000262F4 File Offset: 0x000244F4
		private void Analyze(ConfuserContext context, INameService service, PropertyDef property, ProtectionParameters parameters)
		{
			bool flag = JsonAnalyzer.ShouldExclude(property.DeclaringType, property);
			if (flag)
			{
				service.SetCanRename(property, false);
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00026320 File Offset: 0x00024520
		private void Analyze(ConfuserContext context, INameService service, FieldDef field, ProtectionParameters parameters)
		{
			bool flag = JsonAnalyzer.ShouldExclude(field.DeclaringType, field);
			if (flag)
			{
				service.SetCanRename(field, false);
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x04000531 RID: 1329
		private const string JsonProperty = "Newtonsoft.Json.JsonPropertyAttribute";

		// Token: 0x04000532 RID: 1330
		private const string JsonIgnore = "Newtonsoft.Json.JsonIgnoreAttribute";

		// Token: 0x04000533 RID: 1331
		private const string JsonObject = "Newtonsoft.Json.JsonObjectAttribute";

		// Token: 0x04000534 RID: 1332
		private static readonly HashSet<string> JsonContainers = new HashSet<string>
		{
			"Newtonsoft.Json.JsonArrayAttribute",
			"Newtonsoft.Json.JsonContainerAttribute",
			"Newtonsoft.Json.JsonDictionaryAttribute",
			"Newtonsoft.Json.JsonObjectAttribute"
		};
	}
}
