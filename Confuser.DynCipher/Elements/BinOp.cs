using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000024 RID: 36
	internal class BinOp : CryptoElement
	{
		// Token: 0x060000AA RID: 170 RVA: 0x00002503 File Offset: 0x00000703
		public BinOp() : base(2)
		{
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000AB RID: 171 RVA: 0x0000250E File Offset: 0x0000070E
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00002516 File Offset: 0x00000716
		public CryptoBinOps Operation { get; private set; }

		// Token: 0x060000AD RID: 173 RVA: 0x0000251F File Offset: 0x0000071F
		public override void Initialize(RandomGenerator random)
		{
			this.Operation = (CryptoBinOps)random.NextInt32(3);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006270 File Offset: 0x00004470
		public override void Emit(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			Expression dataExpression2 = context.GetDataExpression(base.DataIndexes[1]);
			switch (this.Operation)
			{
			case CryptoBinOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression + dataExpression2,
					Target = dataExpression
				});
				break;
			case CryptoBinOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ dataExpression2),
					Target = dataExpression
				});
				break;
			case CryptoBinOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = ~(dataExpression ^ dataExpression2),
					Target = dataExpression
				});
				break;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000632C File Offset: 0x0000452C
		public override void EmitInverse(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			Expression dataExpression2 = context.GetDataExpression(base.DataIndexes[1]);
			switch (this.Operation)
			{
			case CryptoBinOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression - dataExpression2,
					Target = dataExpression
				});
				break;
			case CryptoBinOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ dataExpression2),
					Target = dataExpression
				});
				break;
			case CryptoBinOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ ~dataExpression2),
					Target = dataExpression
				});
				break;
			}
		}
	}
}
