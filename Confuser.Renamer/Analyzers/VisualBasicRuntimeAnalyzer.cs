using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.Analyzers
{
	// Token: 0x02000077 RID: 119
	internal sealed class VisualBasicRuntimeAnalyzer : IRenamer
	{
		// Token: 0x060002EE RID: 750 RVA: 0x0002807C File Offset: 0x0002627C
		public void Analyze(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
			TypeDef typeDef = def as TypeDef;
			bool flag = typeDef != null;
			if (flag)
			{
				VisualBasicRuntimeAnalyzer.AnalyzeType(context, service, parameters, typeDef);
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x000280A8 File Offset: 0x000262A8
		private static void AnalyzeType(ConfuserContext context, INameService service, ProtectionParameters parameters, TypeDef def)
		{
			bool flag = VisualBasicRuntimeAnalyzer.IsEmbeddedAttribute(def) && def.BaseType != null && def.BaseType.FullName.Equals("System.Attribute", StringComparison.Ordinal);
			if (flag)
			{
				service.SetCanRename(def, false);
			}
			else
			{
				bool flag2;
				if (def.HasCustomAttributes)
				{
					flag2 = def.CustomAttributes.Any((CustomAttribute a) => VisualBasicRuntimeAnalyzer.IsEmbeddedAttribute(a.AttributeType));
				}
				else
				{
					flag2 = false;
				}
				bool flag3 = flag2;
				if (flag3)
				{
					service.SetCanRename(def, false);
				}
			}
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00028134 File Offset: 0x00026334
		private static bool IsEmbeddedAttribute(ITypeDefOrRef defOrRef)
		{
			bool flag = defOrRef.FullName.Equals("Microsoft.VisualBasic.Embedded", StringComparison.Ordinal);
			if (flag)
			{
				TypeDef typeDef = defOrRef as TypeDef;
				bool flag2 = typeDef != null;
				if (flag2)
				{
					return typeDef.IsNotPublic && typeDef.BaseType != null && typeDef.BaseType.FullName.Equals("System.Attribute", StringComparison.Ordinal);
				}
			}
			return false;
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PostRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0000D3D3 File Offset: 0x0000B5D3
		public void PreRename(ConfuserContext context, INameService service, ProtectionParameters parameters, IDnlibDef def)
		{
		}
	}
}
