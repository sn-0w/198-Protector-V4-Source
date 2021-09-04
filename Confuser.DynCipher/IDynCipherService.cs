using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;

namespace Confuser.DynCipher
{
	// Token: 0x02000003 RID: 3
	public interface IDynCipherService
	{
		// Token: 0x06000009 RID: 9
		void GenerateCipherPair(RandomGenerator random, out StatementBlock encrypt, out StatementBlock decrypt);

		// Token: 0x0600000A RID: 10
		void GenerateExpressionPair(RandomGenerator random, Expression var, Expression result, int depth, out Expression expression, out Expression inverse);
	}
}
