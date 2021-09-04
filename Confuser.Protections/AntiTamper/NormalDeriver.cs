using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x02000101 RID: 257
	internal class NormalDeriver : IKeyDeriver
	{
		// Token: 0x06000445 RID: 1093 RVA: 0x00002B94 File Offset: 0x00000D94
		public void Init(ConfuserContext ctx, RandomGenerator random)
		{
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x0001C0C0 File Offset: 0x0001A2C0
		public uint[] DeriveKey(uint[] a, uint[] b)
		{
			uint[] array = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				switch (i % 3)
				{
				case 0:
					array[i] = (a[i] ^ b[i]);
					break;
				case 1:
					array[i] = a[i] * b[i];
					break;
				case 2:
					array[i] = a[i] + b[i];
					break;
				}
			}
			return array;
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00003729 File Offset: 0x00001929
		public IEnumerable<Instruction> EmitDerivation(MethodDef method, ConfuserContext ctx, Local dst, Local src)
		{
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
				switch (i % 3)
				{
				case 0:
					yield return Instruction.Create(OpCodes.Xor);
					break;
				case 1:
					yield return Instruction.Create(OpCodes.Mul);
					break;
				case 2:
					yield return Instruction.Create(OpCodes.Add);
					break;
				}
				yield return Instruction.Create(OpCodes.Stelem_I4);
				num = i;
			}
			yield break;
		}
	}
}
