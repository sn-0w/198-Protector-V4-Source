using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000095 RID: 149
	public class NotOperator : PatternOperator
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000399 RID: 921 RVA: 0x00015A90 File Offset: 0x00013C90
		public override string Name
		{
			get
			{
				return "not";
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00015AA8 File Offset: 0x00013CA8
		public override bool IsUnary
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00015ABC File Offset: 0x00013CBC
		public override object Evaluate(IDnlibDef definition)
		{
			return !(bool)base.OperandA.Evaluate(definition);
		}

		// Token: 0x0400026D RID: 621
		internal const string OpName = "not";
	}
}
