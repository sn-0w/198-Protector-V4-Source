using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Protections.TypeScramble.Scrambler.Rewriter.Instructions;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.TypeScramble.Scrambler
{
	// Token: 0x0200003C RID: 60
	internal sealed class TypeRewriter
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000161 RID: 353 RVA: 0x000028D1 File Offset: 0x00000AD1
		private TypeService Service { get; }

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000162 RID: 354 RVA: 0x000028D9 File Offset: 0x00000AD9
		private InstructionRewriterFactory RewriteFactory { get; }

		// Token: 0x06000163 RID: 355 RVA: 0x00007A0C File Offset: 0x00005C0C
		internal TypeRewriter(ConfuserContext context)
		{
			Debug.Assert(context != null, "context != null");
			this.Service = context.Registry.GetService<TypeService>();
			Debug.Assert(this.Service != null, "Service != null");
			this.RewriteFactory = new InstructionRewriterFactory
			{
				new MethodSpecInstructionRewriter(),
				new MethodDefInstructionRewriter(),
				new MemberRefInstructionRewriter(),
				new TypeRefInstructionRewriter(),
				new TypeDefInstructionRewriter()
			};
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000028E1 File Offset: 0x00000AE1
		internal void ApplyGenerics()
		{
			this.Service.PrepareItems();
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00007A9C File Offset: 0x00005C9C
		internal void Process(MethodDef method)
		{
			Debug.Assert(method != null, "method != null");
			IList<Instruction> instructions = method.Body.Instructions;
			for (int i = 0; i < instructions.Count; i++)
			{
				this.RewriteFactory.Process(this.Service, method, instructions, ref i);
			}
		}
	}
}
