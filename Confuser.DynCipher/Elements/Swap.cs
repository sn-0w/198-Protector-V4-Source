using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x0200002C RID: 44
	internal class Swap : CryptoElement
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00002503 File Offset: 0x00000703
		public Swap() : base(2)
		{
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000DE RID: 222 RVA: 0x00002667 File Offset: 0x00000867
		// (set) Token: 0x060000DF RID: 223 RVA: 0x0000266F File Offset: 0x0000086F
		public uint Mask { get; private set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x00002678 File Offset: 0x00000878
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x00002680 File Offset: 0x00000880
		public uint Key { get; private set; }

		// Token: 0x060000E2 RID: 226 RVA: 0x00006FFC File Offset: 0x000051FC
		public override void Initialize(RandomGenerator random)
		{
			bool flag = random.NextInt32(3) == 0;
			if (flag)
			{
				this.Mask = uint.MaxValue;
			}
			else
			{
				this.Mask = random.NextUInt32();
			}
			this.Key = (random.NextUInt32() | 1U);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00007040 File Offset: 0x00005240
		private void EmitCore(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(base.DataIndexes[0]);
			Expression dataExpression2 = context.GetDataExpression(base.DataIndexes[1]);
			bool flag = this.Mask == uint.MaxValue;
			if (flag)
			{
				VariableExpression variableExpression;
				using (context.AcquireTempVar(out variableExpression))
				{
					context.Emit(new AssignmentStatement
					{
						Value = dataExpression * this.Key,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = dataExpression2,
						Target = dataExpression
					}).Emit(new AssignmentStatement
					{
						Value = variableExpression * MathsUtils.modInv(this.Key),
						Target = dataExpression2
					});
				}
			}
			else
			{
				LiteralExpression b = this.Mask;
				LiteralExpression b2 = ~this.Mask;
				VariableExpression variableExpression;
				using (context.AcquireTempVar(out variableExpression))
				{
					context.Emit(new AssignmentStatement
					{
						Value = (dataExpression & b) * this.Key,
						Target = variableExpression
					}).Emit(new AssignmentStatement
					{
						Value = ((dataExpression & b2) | (dataExpression2 & b)),
						Target = dataExpression
					}).Emit(new AssignmentStatement
					{
						Value = ((dataExpression2 & b2) | variableExpression * MathsUtils.modInv(this.Key)),
						Target = dataExpression2
					});
				}
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00002689 File Offset: 0x00000889
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00002689 File Offset: 0x00000889
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context);
		}
	}
}
