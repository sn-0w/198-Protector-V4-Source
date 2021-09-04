using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000AA RID: 170
	internal class DynamicMode : IEncodeMode
	{
		// Token: 0x060002E6 RID: 742 RVA: 0x000141E8 File Offset: 0x000123E8
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
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

		// Token: 0x060002E7 RID: 743 RVA: 0x00014294 File Offset: 0x00012494
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] array = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, array, 0, key.Length * 4);
			this.encryptFunc(array, key);
			return array;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x000142D0 File Offset: 0x000124D0
		public object CreateDecoder(MethodDef decoder, CEContext ctx)
		{
			uint k1 = ctx.Random.NextUInt32() | 1U;
			uint k2 = ctx.Random.NextUInt32();
			MutationHelper.ReplacePlaceholder(decoder, delegate(Instruction[] arg)
			{
				List<Instruction> list = new List<Instruction>();
				list.AddRange(arg);
				list.Add(Instruction.Create(OpCodes.Ldc_I4, (int)MathsUtils.modInv(k1)));
				list.Add(Instruction.Create(OpCodes.Mul));
				list.Add(Instruction.Create(OpCodes.Ldc_I4, (int)k2));
				list.Add(Instruction.Create(OpCodes.Xor));
				return list.ToArray();
			});
			return Tuple.Create<uint, uint>(k1, k2);
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x00014330 File Offset: 0x00012530
		public uint Encode(object data, CEContext ctx, uint id)
		{
			Tuple<uint, uint> tuple = (Tuple<uint, uint>)data;
			uint num = (id ^ tuple.Item2) * tuple.Item1;
			Debug.Assert((num * MathsUtils.modInv(tuple.Item1) ^ tuple.Item2) == id);
			return num;
		}

		// Token: 0x04000156 RID: 342
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000AB RID: 171
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060002EB RID: 747 RVA: 0x000030E4 File Offset: 0x000012E4
			public CodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x060002EC RID: 748 RVA: 0x00014378 File Offset: 0x00012578
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

			// Token: 0x04000157 RID: 343
			private readonly Local block;

			// Token: 0x04000158 RID: 344
			private readonly Local key;
		}
	}
}
