using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005E RID: 94
	internal class ExpressionEncoding : IRPEncoding
	{
		// Token: 0x060001DC RID: 476 RVA: 0x0000A68C File Offset: 0x0000888C
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			ValueTuple<Expression, ExpressionEncoding.EncodeKey> key = this.GetKey(ctx, init);
			List<Instruction> list = new List<Instruction>();
			new ExpressionEncoding.CodeGen(arg, ctx.Method, list).GenerateCIL(key.Item1);
			CilBody body = init.Body;
			body.MaxStack += (ushort)ctx.Depth;
			return list.ToArray();
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A6E8 File Offset: 0x000088E8
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			ValueTuple<Expression, ExpressionEncoding.EncodeKey> key = this.GetKey(ctx, init);
			return key.Item2(value);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000A710 File Offset: 0x00008910
		private static void Compile(RPContext ctx, CilBody body, out ExpressionEncoding.EncodeKey expCompiled, out Expression inverse)
		{
			Variable variable = new Variable("{VAR}");
			Variable variable2 = new Variable("{RESULT}");
			Expression expression;
			ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
			{
				Variable = variable
			}, new VariableExpression
			{
				Variable = variable2
			}, ctx.Depth, out expression, out inverse);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<ExpressionEncoding.EncodeKey>();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000A7A4 File Offset: 0x000089A4
		[return: TupleElementNames(new string[]
		{
			"DecodeExpression",
			"EncodeFunction"
		})]
		private ValueTuple<Expression, ExpressionEncoding.EncodeKey> GetKey(RPContext ctx, MethodDef init)
		{
			ValueTuple<Expression, ExpressionEncoding.EncodeKey> valueTuple;
			bool flag = this._keys.TryGetValue(init, out valueTuple);
			ValueTuple<Expression, ExpressionEncoding.EncodeKey> result;
			if (flag)
			{
				result = valueTuple;
			}
			else
			{
				ExpressionEncoding.EncodeKey encodeKey;
				Expression expression;
				ExpressionEncoding.Compile(ctx, init.Body, out encodeKey, out expression);
				valueTuple = (this._keys[init] = new ValueTuple<Expression, ExpressionEncoding.EncodeKey>(expression, encodeKey));
				result = valueTuple;
			}
			return result;
		}

		// Token: 0x0400007E RID: 126
		[TupleElementNames(new string[]
		{
			"DecodeExpression",
			"EncodeFunction"
		})]
		private readonly Dictionary<MethodDef, ValueTuple<Expression, ExpressionEncoding.EncodeKey>> _keys = new Dictionary<MethodDef, ValueTuple<Expression, ExpressionEncoding.EncodeKey>>();

		// Token: 0x0200005F RID: 95
		// (Invoke) Token: 0x060001E2 RID: 482
		private delegate int EncodeKey(int key);

		// Token: 0x02000060 RID: 96
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060001E5 RID: 485 RVA: 0x00002BFF File Offset: 0x00000DFF
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this._arg = arg;
			}

			// Token: 0x060001E6 RID: 486 RVA: 0x0000A7F8 File Offset: 0x000089F8
			protected override void LoadVar(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				if (flag)
				{
					foreach (Instruction instr in this._arg)
					{
						base.Emit(instr);
					}
				}
				else
				{
					base.LoadVar(var);
				}
			}

			// Token: 0x0400007F RID: 127
			private readonly Instruction[] _arg;
		}
	}
}
