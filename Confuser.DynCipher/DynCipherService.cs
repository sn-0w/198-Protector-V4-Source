using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher
{
	// Token: 0x02000004 RID: 4
	internal class DynCipherService : IDynCipherService
	{
		// Token: 0x0600000B RID: 11 RVA: 0x0000214B File Offset: 0x0000034B
		public void GenerateCipherPair(RandomGenerator random, out StatementBlock encrypt, out StatementBlock decrypt)
		{
			CipherGenerator.GeneratePair(random, out encrypt, out decrypt);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002157 File Offset: 0x00000357
		public void GenerateExpressionPair(RandomGenerator random, Expression var, Expression result, int depth, out Expression expression, out Expression inverse)
		{
			ExpressionGenerator.GeneratePair(random, var, result, depth, out expression, out inverse);
		}
	}
}
