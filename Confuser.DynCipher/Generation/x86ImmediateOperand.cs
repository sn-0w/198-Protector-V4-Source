using System;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x02000020 RID: 32
	public class x86ImmediateOperand : Ix86Operand
	{
		// Token: 0x06000097 RID: 151 RVA: 0x0000248F File Offset: 0x0000068F
		public x86ImmediateOperand(int imm)
		{
			this.Immediate = imm;
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000024A1 File Offset: 0x000006A1
		// (set) Token: 0x06000099 RID: 153 RVA: 0x000024A9 File Offset: 0x000006A9
		public int Immediate { get; set; }

		// Token: 0x0600009A RID: 154 RVA: 0x0000596C File Offset: 0x00003B6C
		public override string ToString()
		{
			return this.Immediate.ToString("X") + "h";
		}
	}
}
