using System;
using Confuser.Core.Services;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;

namespace Confuser.DynCipher.Elements
{
	// Token: 0x02000022 RID: 34
	internal class AddKey : CryptoElement
	{
		// Token: 0x060000A3 RID: 163 RVA: 0x000024D4 File Offset: 0x000006D4
		public AddKey(int index) : base(0)
		{
			this.Index = index;
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x000024E7 File Offset: 0x000006E7
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x000024EF File Offset: 0x000006EF
		public int Index { get; private set; }

		// Token: 0x060000A6 RID: 166 RVA: 0x0000213F File Offset: 0x0000033F
		public override void Initialize(RandomGenerator random)
		{
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00006228 File Offset: 0x00004428
		private void EmitCore(CipherGenContext context)
		{
			Expression dataExpression = context.GetDataExpression(this.Index);
			context.Emit(new AssignmentStatement
			{
				Value = (dataExpression ^ context.GetKeyExpression(this.Index)),
				Target = dataExpression
			});
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000024F8 File Offset: 0x000006F8
		public override void Emit(CipherGenContext context)
		{
			this.EmitCore(context);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000024F8 File Offset: 0x000006F8
		public override void EmitInverse(CipherGenContext context)
		{
			this.EmitCore(context);
		}
	}
}
