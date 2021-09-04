using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F3 RID: 243
	internal class DynamicDeriver : IKeyDeriver
	{
		// Token: 0x060003F6 RID: 1014 RVA: 0x0001A780 File Offset: 0x00018980
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
			StatementBlock statementBlock;
			ctx.Registry.GetService<IDynCipherService>().GenerateCipherPair(random, out this.derivation, out statementBlock);
			DMCodeGen dmcodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
				Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
			});
			dmcodeGen.GenerateCIL(this.derivation);
			this.encryptFunc = dmcodeGen.Compile<Action<uint[], uint[]>>();
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0001A804 File Offset: 0x00018A04
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] array = new uint[16];
			Buffer.BlockCopy(a, 0, array, 0, a.Length * 4);
			this.encryptFunc(array, b);
			return array;
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0001A838 File Offset: 0x00018A38
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			List<Instruction> list = new List<Instruction>();
			DynamicDeriver.CodeGen codeGen = new DynamicDeriver.CodeGen(dst, src, method, list);
			codeGen.GenerateCIL(this.derivation);
			codeGen.Commit(method.Body);
			return list;
		}

		// Token: 0x040001ED RID: 493
		private StatementBlock derivation;

		// Token: 0x040001EE RID: 494
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000F4 RID: 244
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060003FA RID: 1018 RVA: 0x00003552 File Offset: 0x00001752
			public CodeGen(Local block, Local key, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x060003FB RID: 1019 RVA: 0x0001A870 File Offset: 0x00018A70
			protected override Local Var(Variable var)
			{
				Local result;
				if (var.Name == "{BUFFER}")
				{
					result = this.block;
				}
				else if (var.Name == "{KEY}")
				{
					result = this.key;
				}
				else
				{
					result = base.Var(var);
				}
				return result;
			}

			// Token: 0x040001EF RID: 495
			private readonly Local block;

			// Token: 0x040001F0 RID: 496
			private readonly Local key;
		}
	}
}
