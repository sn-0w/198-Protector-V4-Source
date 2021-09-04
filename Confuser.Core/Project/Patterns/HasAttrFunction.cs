using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000090 RID: 144
	public class HasAttrFunction : PatternFunction
	{
		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000385 RID: 901 RVA: 0x00015694 File Offset: 0x00013894
		public override string Name
		{
			get
			{
				return "has-attr";
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000386 RID: 902 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000156C0 File Offset: 0x000138C0
		public override object Evaluate(IDnlibDef definition)
		{
			string fullName = base.Arguments[0].Evaluate(definition).ToString();
			return definition.CustomAttributes.IsDefined(fullName);
		}

		// Token: 0x04000268 RID: 616
		internal const string FnName = "has-attr";
	}
}
