using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200002A RID: 42
	internal class NumOp : CryptoElement
	{
		// Token: 0x060000CB RID: 203 RVA: 0x000025E2 File Offset: 0x000007E2
		public NumOp() : base(1)
		{
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000CC RID: 204 RVA: 0x000025ED File Offset: 0x000007ED
		// (set) Token: 0x060000CD RID: 205 RVA: 0x000025F5 File Offset: 0x000007F5
		public uint Key { get; private set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000CE RID: 206 RVA: 0x000025FE File Offset: 0x000007FE
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00002606 File Offset: 0x00000806
		public uint InverseKey { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000260F File Offset: 0x0000080F
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x00002617 File Offset: 0x00000817
		public CryptoNumOps Operation { get; private set; }

		// Token: 0x060000D2 RID: 210 RVA: 0x00006B6C File Offset: 0x00004D6C
		public override void Initialize(RandomGenerator random)
		{
			this.Operation = (CryptoNumOps)random.NextInt32(4);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
			case CryptoNumOps.Xor:
				this.Key = (this.InverseKey = random.NextUInt32());
				break;
			case CryptoNumOps.Mul:
				this.Key = (random.NextUInt32() | 1U);
				this.InverseKey = MathsUtils.modInv(this.Key);
				break;
			case CryptoNumOps.Xnor:
				this.Key = random.NextUInt32();
				this.InverseKey = ~this.Key;
				break;
			}
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006C04 File Offset: 0x00004E04
		public override void Emit(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression + this.Key,
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Mul:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression * this.Key,
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ this.Key),
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = ~(dataExpression ^ this.Key),
					Target = dataExpression
				});
				break;
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006D08 File Offset: 0x00004F08
		public override void EmitInverse(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			switch (this.Operation)
			{
			case CryptoNumOps.Add:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression - this.InverseKey,
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Mul:
				context.Emit(new AssignmentStatement
				{
					Value = dataExpression * this.InverseKey,
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Xor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ this.InverseKey),
					Target = dataExpression
				});
				break;
			case CryptoNumOps.Xnor:
				context.Emit(new AssignmentStatement
				{
					Value = (dataExpression ^ this.InverseKey),
					Target = dataExpression
				});
				break;
			}
		}
	}
}
