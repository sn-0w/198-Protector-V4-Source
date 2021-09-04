using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009D RID: 157
	public class OrOperator : PatternOperator
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x00016024 File Offset: 0x00014224
		public override string Name
		{
			get
			{
				return "or";
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00011680 File Offset: 0x0000F880
		public override bool IsUnary
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0001603C File Offset: 0x0001423C
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = (bool)base.OperandA.Evaluate(definition);
			bool flag2 = flag;
			object result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				result = (bool)base.OperandB.Evaluate(definition);
			}
			return result;
		}

		// Token: 0x04000275 RID: 629
		internal const string OpName = "or";
	}
}
