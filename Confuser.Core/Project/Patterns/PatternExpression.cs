using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x020000A0 RID: 160
	public abstract class PatternExpression
	{
		// Token: 0x060003C6 RID: 966
		public abstract object Evaluate(IDnlibDef definition);

		// Token: 0x060003C7 RID: 967
		public abstract void Serialize(IList<PatternToken> tokens);
	}
}
