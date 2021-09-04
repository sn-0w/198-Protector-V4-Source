using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000073 RID: 115
	internal class NancyFxAnalyzer : IRenamer
	{
		// Token: 0x060002D1 RID: 721 RVA: 0x00026CD0 File Offset: 0x00024ED0
		private static bool ShouldExclude(TypeDef type, IDnlibDef def)
		{
			ITypeDefOrRef baseType = type.BaseType;
			bool flag = ((baseType != null) ? baseType.FullName : null) == "NancyModule";
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				ITypeDefOrRef baseType2 = type.BaseType;
				bool flag2 = ((baseType2 != null) ? baseType2.FullName : null) == "DefaultNancyBootstrapper";
				result = flag2;
			}
			return result;
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x00026D30 File Offset: 0x00024F30
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

		// Token: 0x060002D3 RID: 723 RVA: 0x00026DC0 File Offset: 0x00024FC0
		private void Analyze(ConfuserContext context, INameService service, TypeDef type, ProtectionParameters parameters)
		{
			bool flag = NancyFxAnalyzer.ShouldExclude(type, type);
			if (flag)
			{
				service.SetCanRename(type, false);
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x00026DE4 File Offset: 0x00024FE4
		private void Analyze(ConfuserContext context, INameService service, MethodDef method, ProtectionParameters parameters)
		{
			bool flag = NancyFxAnalyzer.ShouldExclude(method.DeclaringType, method);
			if (flag)
			{
				service.SetCanRename(method, false);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x00026E0C File Offset: 0x0002500C
		private void Analyze(ConfuserContext context, INameService service, PropertyDef property, ProtectionParameters parameters)
		{
			bool flag = NancyFxAnalyzer.ShouldExclude(property.DeclaringType, property);
			if (flag)
			{
				service.SetCanRename(property, false);
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x00026E34 File Offset: 0x00025034
		private void Analyze(ConfuserContext context, INameService service, FieldDef field, ProtectionParameters parameters)
		{
			bool flag = NancyFxAnalyzer.ShouldExclude(field.DeclaringType, field);
			if (flag)
			{
				service.SetCanRename(field, false);
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
