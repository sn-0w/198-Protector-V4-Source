using System;

namespace Confuser.DynCipher.AST
{
	// Token: 0x0200002E RID: 46
	public class AssignmentStatement : Statement
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000EC RID: 236 RVA: 0x000026BF File Offset: 0x000008BF
		// (set) Token: 0x060000ED RID: 237 RVA: 0x000026C7 File Offset: 0x000008C7
		public Expression Target { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000EE RID: 238 RVA: 0x000026D0 File Offset: 0x000008D0
		// (set) Token: 0x060000EF RID: 239 RVA: 0x000026D8 File Offset: 0x000008D8
		public Expression Value { get; set; }

		// Token: 0x060000F0 RID: 240 RVA: 0x00007234 File Offset: 0x00005434
		public override string ToString()
		{
			return string.Format("{0} = {1};", this.Target, this.Value);
		}
	}
}
