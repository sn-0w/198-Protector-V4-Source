using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Compress
{
	// Token: 0x020000CA RID: 202
	internal class NormalDeriver : IKeyDeriver
	{
		// Token: 0x06000353 RID: 851 RVA: 0x0000322A File Offset: 0x0000142A
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
			this.k1 = (random.NextUInt32() | 1U);
			this.k2 = (random.NextUInt32() | 1U);
			this.k3 = (random.NextUInt32() | 1U);
			this.seed = random.NextUInt32();
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00017F98 File Offset: 0x00016198
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] array = new uint[16];
			uint num = this.seed;
			for (int i = 0; i < 16; i++)
			{
				switch (num % 3U)
				{
				case 0U:
					array[i] = (a[i] ^ b[i]);
					break;
				case 1U:
					array[i] = a[i] * b[i];
					break;
				case 2U:
					array[i] = a[i] + b[i];
					break;
				}
				num = num * num % 772287797U;
				switch (num % 3U)
				{
				case 0U:
					array[i] += this.k1;
					break;
				case 1U:
					array[i] ^= this.k2;
					break;
				case 2U:
					array[i] *= this.k3;
					break;
				}
				num = num * num % 772287797U;
			}
			return array;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00003263 File Offset: 0x00001463
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
			uint state = this.seed;
			int num;
			for (int i = 0; i < 16; i = num + 1)
			{
				yield return Instruction.Create(OpCodes.Ldloc, dst);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldloc, dst);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				yield return Instruction.Create(OpCodes.Ldloc, src);
				yield return Instruction.Create(OpCodes.Ldc_I4, i);
				yield return Instruction.Create(OpCodes.Ldelem_U4);
				switch (state % 3U)
				{
				case 0U:
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 1U:
					yield return Instruction.Create(OpCodes.Mul);
					break;
				case 2U:
					yield return Instruction.Create(OpCodes.Add);
					break;
				}
				state = state * state % 772287797U;
				switch (state % 3U)
				{
				case 0U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k1);
					yield return Instruction.Create(OpCodes.Add);
					break;
				case 1U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k2);
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 2U:
					yield return Instruction.Create(OpCodes.Ldc_I4, (int)this.k3);
					yield return Instruction.Create(OpCodes.Mul);
					break;
				}
				state = state * state % 772287797U;
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}

		// Token: 0x0400019E RID: 414
		private uint k1;

		// Token: 0x0400019F RID: 415
		private uint k2;

		// Token: 0x040001A0 RID: 416
		private uint k3;

		// Token: 0x040001A1 RID: 417
		private uint seed;
	}
}
