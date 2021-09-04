using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000061 RID: 97
	internal class NormalEncoding : IRPEncoding
	{
		// Token: 0x060001E7 RID: 487 RVA: 0x0000A848 File Offset: 0x00008A48
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			int item = this.GetKey(ctx.Random, init).Item1;
			List<Instruction> list = new List<Instruction>();
			bool flag = ctx.Random.NextBoolean();
			if (flag)
			{
				list.Add(Instruction.Create(OpCodes.Ldc_I4, item));
				list.AddRange(arg);
			}
			else
			{
				list.AddRange(arg);
				list.Add(Instruction.Create(OpCodes.Ldc_I4, item));
			}
			list.Add(Instruction.Create(OpCodes.Mul));
			return list.ToArray();
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000A8D4 File Offset: 0x00008AD4
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			int item = this.GetKey(ctx.Random, init).Item2;
			return value * item;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000A8FC File Offset: 0x00008AFC
		[return: TupleElementNames(new string[]
		{
			"Key",
			"InvKey"
		})]
		private ValueTuple<int, int> GetKey(RandomGenerator random, MethodDef init)
		{
			ValueTuple<int, int> valueTuple;
			bool flag = this._keys.TryGetValue(init, out valueTuple);
			ValueTuple<int, int> result;
			if (flag)
			{
				result = valueTuple;
			}
			else
			{
				int num = random.NextInt32() | 1;
				valueTuple = (this._keys[init] = new ValueTuple<int, int>(num, (int)MathsUtils.modInv((uint)num)));
				result = valueTuple;
			}
			return result;
		}

		// Token: 0x04000080 RID: 128
		[TupleElementNames(new string[]
		{
			"Key",
			"InvKey"
		})]
		private readonly Dictionary<MethodDef, ValueTuple<int, int>> _keys = new Dictionary<MethodDef, ValueTuple<int, int>>();
	}
}
