using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009A RID: 154
	public class MatchTypeNameFunction : PatternFunction
	{
		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060003AD RID: 941 RVA: 0x00015DC4 File Offset: 0x00013FC4
		public override string Name
		{
			get
			{
				return "match-type-name";
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060003AE RID: 942 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00015DDC File Offset: 0x00013FDC
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = definition is TypeDef;
			object result;
			if (flag)
			{
				string pattern = base.Arguments[0].Evaluate(definition).ToString();
				result = Regex.IsMatch(definition.Name, pattern);
			}
			else
			{
				bool flag2 = definition is IMemberDef && ((IMemberDef)definition).DeclaringType != null;
				if (flag2)
				{
					string pattern2 = base.Arguments[0].Evaluate(definition).ToString();
					result = Regex.IsMatch(((IMemberDef)definition).DeclaringType.Name, pattern2);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04000272 RID: 626
		internal const string FnName = "match-type-name";
	}
}
