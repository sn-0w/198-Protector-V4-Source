using System;

namespace Confuser.Protections.Arithmetic
{
	// Token: 0x020000D9 RID: 217
	public class ArithmeticVT
	{
		// Token: 0x0600039A RID: 922 RVA: 0x00003405 File Offset: 0x00001605
		public ArithmeticVT(Value value, Token token, ArithmeticTypes arithmeticTypes)
		{
			this.value = value;
			this.token = token;
			this.arithmeticTypes = arithmeticTypes;
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00003424 File Offset: 0x00001624
		public Value GetValue()
		{
			return this.value;
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0000342C File Offset: 0x0000162C
		public Token GetToken()
		{
			return this.token;
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00003434 File Offset: 0x00001634
		public ArithmeticTypes GetArithmetic()
		{
			return this.arithmeticTypes;
		}

		// Token: 0x040001D6 RID: 470
		private Value value;

		// Token: 0x040001D7 RID: 471
		private Token token;

		// Token: 0x040001D8 RID: 472
		private ArithmeticTypes arithmeticTypes;
	}
}
