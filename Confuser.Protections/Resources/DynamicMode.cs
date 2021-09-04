using System;
using System.Collections.Generic;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Resources
{
	// Token: 0x0200004E RID: 78
	internal class DynamicMode : IEncodeMode
	{
		// Token: 0x060001AC RID: 428 RVA: 0x00008F5C File Offset: 0x0000715C
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, REContext ctx, Local block, Local key)
		{
			StatementBlock statement;
			StatementBlock statement2;
			ctx.DynCipher.GenerateCipherPair(ctx.Random, out statement, out statement2);
			List<Instruction> list = new List<Instruction>();
			DynamicMode.CodeGen codeGen = new DynamicMode.CodeGen(block, key, init, list);
			codeGen.GenerateCIL(statement2);
			codeGen.Commit(init.Body);
			DMCodeGen dmcodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
				Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
			});
			dmcodeGen.GenerateCIL(statement);
			this.encryptFunc = dmcodeGen.Compile<Action<uint[], uint[]>>();
			return list;
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00009008 File Offset: 0x00007208
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] array = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, array, 0, key.Length * 4);
			this.encryptFunc(array, key);
			return array;
		}

		// Token: 0x04000050 RID: 80
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x0200004F RID: 79
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060001AF RID: 431 RVA: 0x00002ABB File Offset: 0x00000CBB
			public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x060001B0 RID: 432 RVA: 0x00009044 File Offset: 0x00007244
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

			// Token: 0x04000051 RID: 81
			private readonly Local block;

			// Token: 0x04000052 RID: 82
			private readonly Local key;
		}
	}
}
