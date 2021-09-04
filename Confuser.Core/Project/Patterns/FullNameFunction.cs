using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000094 RID: 148
	public class FullNameFunction : PatternFunction
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x06000395 RID: 917 RVA: 0x00015A3C File Offset: 0x00013C3C
		public override string Name
		{
			get
			{
				return "full-name";
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000396 RID: 918 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00015A54 File Offset: 0x00013C54
		public override object Evaluate(IDnlibDef definition)
		{
			object obj = base.Arguments[0].Evaluate(definition);
			return definition.FullName == obj.ToString();
		}

		// Token: 0x0400026C RID: 620
		internal const string FnName = "full-name";
	}
}
