using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x0200006E RID: 110
	internal class AspNetCoreAnalyzer : IRenamer
	{
		// Token: 0x060002A9 RID: 681 RVA: 0x000254B8 File Offset: 0x000236B8
		private static bool ShouldExclude(TypeDef type, IDnlibDef def)
		{
			foreach (MethodDef methodDef in type.FindInstanceConstructors())
			{
				bool flag = methodDef.Parameters.Count > 1 && methodDef.Parameters.Skip(1).First<Parameter>().Type.TypeName == "IHostingEnvironment";
				if (flag)
				{
					return true;
				}
			}
			ITypeDefOrRef baseType = type.BaseType;
			return ((baseType != null) ? baseType.FullName : null) == "Microsoft.AspNetCore.Mvc.Controller";
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00025570 File Offset: 0x00023770
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

		// Token: 0x060002AB RID: 683 RVA: 0x00025600 File Offset: 0x00023800
		private void Analyze(ConfuserContext context, INameService service, TypeDef type, ProtectionParameters parameters)
		{
			bool flag = AspNetCoreAnalyzer.ShouldExclude(type, type);
			if (flag)
			{
				service.SetCanRename(type, false);
			}
		}

		// Token: 0x060002AC RID: 684 RVA: 0x00025624 File Offset: 0x00023824
		private void Analyze(ConfuserContext context, INameService service, MethodDef method, ProtectionParameters parameters)
		{
			bool flag = AspNetCoreAnalyzer.ShouldExclude(method.DeclaringType, method);
			if (flag)
			{
				service.SetCanRename(method, false);
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0002564C File Offset: 0x0002384C
		private void Analyze(ConfuserContext context, INameService service, PropertyDef property, ProtectionParameters parameters)
		{
			bool flag = AspNetCoreAnalyzer.ShouldExclude(property.DeclaringType, property);
			if (flag)
			{
				service.SetCanRename(property, false);
			}
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00025674 File Offset: 0x00023874
		private void Analyze(ConfuserContext context, INameService service, FieldDef field, ProtectionParameters parameters)
		{
			bool flag = AspNetCoreAnalyzer.ShouldExclude(field.DeclaringType, field);
			if (flag)
			{
				service.SetCanRename(field, false);
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
