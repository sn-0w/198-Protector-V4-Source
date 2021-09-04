using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000099 RID: 153
	public class MatchNameFunction : PatternFunction
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060003A9 RID: 937 RVA: 0x00015D6C File Offset: 0x00013F6C
		public override string Name
		{
			get
			{
				return "match-name";
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060003AA RID: 938 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00015D84 File Offset: 0x00013F84
		public override object Evaluate(IDnlibDef definition)
		{
			string pattern = base.Arguments[0].Evaluate(definition).ToString();
			return Regex.IsMatch(definition.Name, pattern);
		}

		// Token: 0x04000271 RID: 625
		internal const string FnName = "match-name";
	}
}
