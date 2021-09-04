using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000097 RID: 151
	public class DeclTypeFunction : PatternFunction
	{
		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060003A1 RID: 929 RVA: 0x00015C94 File Offset: 0x00013E94
		public override string Name
		{
			get
			{
				return "decl-type";
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00015CAC File Offset: 0x00013EAC
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = !(definition is IMemberDef) || ((IMemberDef)definition).DeclaringType == null;
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				object obj = base.Arguments[0].Evaluate(definition);
				result = (((IMemberDef)definition).DeclaringType.FullName == obj.ToString());
			}
			return result;
		}

		// Token: 0x0400026F RID: 623
		internal const string FnName = "decl-type";
	}
}
