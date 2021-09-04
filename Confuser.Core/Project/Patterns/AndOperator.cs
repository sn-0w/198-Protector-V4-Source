using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200008F RID: 143
	public class AndOperator : PatternOperator
	{
		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000381 RID: 897 RVA: 0x00015630 File Offset: 0x00013830
		public override string Name
		{
			get
			{
				return "and";
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000382 RID: 898 RVA: 0x00011680 File Offset: 0x0000F880
		public override bool IsUnary
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00015648 File Offset: 0x00013848
		public override object Evaluate(IDnlibDef definition)
		{
			bool flag = (bool)base.OperandA.Evaluate(definition);
			bool flag2 = !flag;
			object result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				result = (bool)base.OperandB.Evaluate(definition);
			}
			return result;
		}

		// Token: 0x04000267 RID: 615
		internal const string OpName = "and";
	}
}
