using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009C RID: 156
	public class NamespaceFunction : PatternFunction
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x00015F3C File Offset: 0x0001413C
		public override string Name
		{
			get
			{
				return "namespace";
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00015F54 File Offset: 0x00014154
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is TypeDef) && !(definition is IMemberDef);
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string pattern = "^" + base.Arguments[0].Evaluate(definition).ToString() + "$";
				TypeDef typeDef = definition as TypeDef;
				bool flag2 = typeDef == null;
				if (flag2)
				{
					typeDef = ((IMemberDef)definition).DeclaringType;
				}
				bool flag3 = typeDef == null;
				if (flag3)
				{
					result = false;
				}
				else
				{
					while (typeDef.IsNested)
					{
						typeDef = typeDef.DeclaringType;
					}
					result = (typeDef != null && Regex.IsMatch(typeDef.Namespace ?? "", pattern));
				}
			}
			return result;
		}

		// Token: 0x04000274 RID: 628
		internal const string FnName = "namespace";
	}
}
