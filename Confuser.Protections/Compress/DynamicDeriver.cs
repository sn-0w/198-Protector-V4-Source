using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000C4 RID: 196
	internal class DynamicDeriver : IKeyDeriver
	{
		// Token: 0x06000344 RID: 836 RVA: 0x00017BA8 File Offset: 0x00015DA8
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

		// Token: 0x06000345 RID: 837 RVA: 0x00017C30 File Offset: 0x00015E30
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] array = new uint[16];
			Buffer.BlockCopy(a, 0, array, 0, a.Length * 4);
			this.encryptFunc(array, b);
			return array;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00017C68 File Offset: 0x00015E68
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			List<Instruction> list = new List<Instruction>();
			DynamicDeriver.CodeGen codeGen = new DynamicDeriver.CodeGen(dst, src, method, list);
			codeGen.GenerateCIL(this.derivation);
			codeGen.Commit(method.Body);
			return list;
		}

		// Token: 0x04000195 RID: 405
		private StatementBlock derivation;

		// Token: 0x04000196 RID: 406
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000C5 RID: 197
		private class CodeGen : CILCodeGen
		{
			// Token: 0x06000348 RID: 840 RVA: 0x000031F7 File Offset: 0x000013F7
			public CodeGen(Local block, Local key, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x06000349 RID: 841 RVA: 0x00017CA8 File Offset: 0x00015EA8
			protected override Local Var(Variable var)
			{
				bool flag = var.Name == "{BUFFER}";
				Local result;
				if (flag)
				{
					result = this.block;
				}
				else
				{
					bool flag2 = var.Name == "{KEY}";
					if (flag2)
					{
						result = this.key;
					}
					else
					{
						result = base.Var(var);
					}
				}
				return result;
			}

			// Token: 0x04000197 RID: 407
			private readonly Local block;

			// Token: 0x04000198 RID: 408
			private readonly Local key;
		}
	}
}
