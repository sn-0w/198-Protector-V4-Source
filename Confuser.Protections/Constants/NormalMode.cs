using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000B9 RID: 185
	internal class NormalMode : IEncodeMode
	{
		// Token: 0x06000315 RID: 789 RVA: 0x00003157 File Offset: 0x00001357
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
		{
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				yield return Instruction.Create(OpCodes.Ldloc, block);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldloc, block);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Ldloc, key);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Xor);
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00009F40 File Offset: 0x00008140
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] array = new uint[key.Length];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = (data[i + offset] ^ key[i]);
			}
			return array;
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000160A0 File Offset: 0x000142A0
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

		// Token: 0x06000318 RID: 792 RVA: 0x00014330 File Offset: 0x00012530
		public uint Encode(object data, CEContext ctx, uint id)
		{
			Tuple<uint, uint> tuple = (Tuple<uint, uint>)data;
			uint num = (id ^ tuple.Item2) * tuple.Item1;
			Debug.Assert((num * MathsUtils.modInv(tuple.Item1) ^ tuple.Item2) == id);
			return num;
		}
	}
}
