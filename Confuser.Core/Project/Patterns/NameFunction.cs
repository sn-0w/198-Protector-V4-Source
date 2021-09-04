using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x0200009F RID: 159
	public class NameFunction : PatternFunction
	{
		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x00016100 File Offset: 0x00014300
		public override string Name
		{
			get
			{
				return "name";
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060003C3 RID: 963 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00016118 File Offset: 0x00014318
		public override object Evaluate(IDnlibDef definition)
		{
			object obj = base.Arguments[0].Evaluate(definition);
			return definition.Name == obj.ToString();
		}

		// Token: 0x04000277 RID: 631
		internal const string FnName = "name";
	}
}
