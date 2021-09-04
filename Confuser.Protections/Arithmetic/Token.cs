using System;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000DC RID: 220
	public class Token
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x0000343C File Offset: 0x0000163C
		public Token(OpCode opCode, object Operand)
		{
			this.opCode = opCode;
			this.Operand = Operand;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00003454 File Offset: 0x00001654
		public Token(OpCode opCode)
		{
			this.opCode = opCode;
			this.Operand = null;
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0000346C File Offset: 0x0000166C
		public OpCode GetOpCode()
		{
			return this.opCode;
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00003474 File Offset: 0x00001674
		public object GetOperand()
		{
			return this.Operand;
		}

		// Token: 0x040001D9 RID: 473
		private OpCode opCode;

		// Token: 0x040001DA RID: 474
		private object Operand;
	}
}
