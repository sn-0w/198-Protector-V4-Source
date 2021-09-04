using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000092 RID: 146
	public class InheritsFunction : PatternFunction
	{
		// Token: 0x17000067 RID: 103
		// (get) Token: 0x0600038D RID: 909 RVA: 0x0001583C File Offset: 0x00013A3C
		public override string Name
		{
			get
			{
				return "inherits";
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600038E RID: 910 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00015854 File Offset: 0x00013A54
		public override object Evaluate(IDnlibDef definition)
		{
			string text = base.Arguments[0].Evaluate(definition).ToString();
			TypeDef typeDef = definition as TypeDef;
			bool flag = typeDef == null && definition is IMemberDef;
			if (flag)
			{
				typeDef = ((IMemberDef)definition).DeclaringType;
			}
			bool flag2 = typeDef == null;
			object result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				bool flag3 = typeDef.InheritsFrom(text) || typeDef.Implements(text);
				if (flag3)
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0400026A RID: 618
		internal const string FnName = "inherits";
	}
}
