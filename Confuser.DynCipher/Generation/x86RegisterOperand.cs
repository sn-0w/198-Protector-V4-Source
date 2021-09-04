using System;

namespace Confuser.DynCipher.Generation
{
	// Token: 0x0200001F RID: 31
	public class x86RegisterOperand : Ix86Operand
	{
		// Token: 0x06000093 RID: 147 RVA: 0x0000246C File Offset: 0x0000066C
		public x86RegisterOperand(x86Register reg)
		{
			this.Register = reg;
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000094 RID: 148 RVA: 0x0000247E File Offset: 0x0000067E
		// (set) Token: 0x06000095 RID: 149 RVA: 0x00002486 File Offset: 0x00000686
		public x86Register Register { get; set; }

		// Token: 0x06000096 RID: 150 RVA: 0x00005944 File Offset: 0x00003B44
		public override string ToString()
		{
			return this.Register.ToString();
		}
	}
}
