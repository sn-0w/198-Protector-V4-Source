using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200002B RID: 43
	internal class RotateBit : CryptoElement
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x000025E2 File Offset: 0x000007E2
		public RotateBit() : base(1)
		{
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00002620 File Offset: 0x00000820
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x00002628 File Offset: 0x00000828
		public int Bits { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00002631 File Offset: 0x00000831
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00002639 File Offset: 0x00000839
		public bool IsAlternate { get; private set; }

		// Token: 0x060000DA RID: 218 RVA: 0x00002642 File Offset: 0x00000842
		public override void Initialize(RandomGenerator random)
		{
			this.Bits = random.NextInt32(1, 32);
			this.IsAlternate = (random.NextInt32() % 2 == 0);
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006E04 File Offset: 0x00005004
		public override void Emit(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			VariableExpression variableExpression;
			using (context.AcquireTempVar(out variableExpression))
			{
				bool isAlternate = this.IsAlternate;
				if (isAlternate)
				{
					context.Emit(new AssignmentStatement
					{
						Value = dataExpression >> 32 - this.Bits,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = (dataExpression << this.Bits | variableExpression),
						Target = dataExpression
					});
				}
				else
				{
					context.Emit(new AssignmentStatement
					{
						Value = dataExpression << 32 - this.Bits,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = (dataExpression >> this.Bits | variableExpression),
						Target = dataExpression
					});
				}
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006F00 File Offset: 0x00005100
		public override void EmitInverse(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			VariableExpression variableExpression;
			using (context.AcquireTempVar(out variableExpression))
			{
				bool isAlternate = this.IsAlternate;
				if (isAlternate)
				{
					context.Emit(new AssignmentStatement
					{
						Value = dataExpression << 32 - this.Bits,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = (dataExpression >> this.Bits | variableExpression),
						Target = dataExpression
					});
				}
				else
				{
					context.Emit(new AssignmentStatement
					{
						Value = dataExpression >> 32 - this.Bits,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = (dataExpression << this.Bits | variableExpression),
						Target = dataExpression
					});
				}
			}
		}
	}
}
