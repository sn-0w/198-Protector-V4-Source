using System;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000098 RID: 152
	public class MatchFunction : PatternFunction
	{
		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060003A5 RID: 933 RVA: 0x00015D18 File Offset: 0x00013F18
		public override string Name
		{
			get
			{
				return "match";
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00015D30 File Offset: 0x00013F30
		public override object Evaluate(IDnlibDef definition)
		{
			string pattern = base.Arguments[0].Evaluate(definition).ToString();
			return Regex.IsMatch(definition.FullName, pattern);
		}

		// Token: 0x04000270 RID: 624
		internal const string FnName = "match";
	}
}
