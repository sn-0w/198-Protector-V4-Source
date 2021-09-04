using System;
using System.Collections.Generic;
using Confuser.Core.Helpers;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.Constants
{
	// Token: 0x020000A5 RID: 165
	internal class x86Mode : IEncodeMode
	{
		// Token: 0x060002D7 RID: 727 RVA: 0x00013D9C File Offset: 0x00011F9C
		public IEnumerable<Instruction> EmitDecrypt(MethodDef init, CEContext ctx, Local block, Local key)
		{
			StatementBlock statement;
			StatementBlock statement2;
			ctx.DynCipher.GenerateCipherPair(ctx.Random, out statement, out statement2);
			List<Instruction> list = new List<Instruction>();
			x86Mode.CipherCodeGen cipherCodeGen = new x86Mode.CipherCodeGen(block, key, init, list);
			cipherCodeGen.GenerateCIL(statement2);
			cipherCodeGen.Commit(init.Body);
			DMCodeGen dmcodeGen = new DMCodeGen(typeof(void), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{BUFFER}", typeof(uint[])),
				Tuple.Create<string, Type>("{KEY}", typeof(uint[]))
			});
			dmcodeGen.GenerateCIL(statement);
			this.encryptFunc = dmcodeGen.Compile<Action<uint[], uint[]>>();
			return list;
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x00013E48 File Offset: 0x00012048
		public uint[] Encrypt(uint[] data, int offset, uint[] key)
		{
			uint[] array = new uint[key.Length];
			Buffer.BlockCopy(data, offset * 4, array, 0, key.Length * 4);
			this.encryptFunc(array, key);
			return array;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00013E84 File Offset: 0x00012084
		public object CreateDecoder(MethodDef decoder, CEContext ctx)
		{
			x86Mode.x86Encoding encoding = new x86Mode.x86Encoding();
			encoding.Compile(ctx);
			MutationHelper.ReplacePlaceholder(decoder, delegate(Instruction[] arg)
			{
				List<Instruction> list = new List<Instruction>();
				list.AddRange(arg);
				list.Add(Instruction.Create(OpCodes.Call, encoding.native));
				return list.ToArray();
			});
			return encoding;
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00013ED0 File Offset: 0x000120D0
		public uint Encode(object data, CEContext ctx, uint id)
		{
			x86Mode.x86Encoding x86Encoding = (x86Mode.x86Encoding)data;
			return (uint)x86Encoding.expCompiled((int)id);
		}

		// Token: 0x0400014A RID: 330
		private Action<uint[], uint[]> encryptFunc;

		// Token: 0x020000A6 RID: 166
		private class CipherCodeGen : CILCodeGen
		{
			// Token: 0x060002DC RID: 732 RVA: 0x000030BD File Offset: 0x000012BD
			public CipherCodeGen(Local block, Local key, MethodDef init, IList<Instruction> instrs) : base(init, instrs)
			{
				this.block = block;
				this.key = key;
			}

			// Token: 0x060002DD RID: 733 RVA: 0x00013EF8 File Offset: 0x000120F8
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

			// Token: 0x0400014B RID: 331
			private readonly Local block;

			// Token: 0x0400014C RID: 332
			private readonly Local key;
		}

		// Token: 0x020000A7 RID: 167
		private class x86Encoding
		{
			// Token: 0x060002DE RID: 734 RVA: 0x00013F4C File Offset: 0x0001214C
			public void Compile(CEContext ctx)
			{
				Variable variable = new Variable("{VAR}");
				Variable variable2 = new Variable("{RESULT}");
				CorLibTypeSig @int = ctx.Module.CorLibTypes.Int32;
				this.native = new MethodDefUser("", MethodSig.CreateStatic(@int, @int), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
				this.native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
				ctx.Module.GlobalType.Methods.Add(this.native);
				ctx.Name.MarkHelper(this.native, ctx.Marker, ctx.Protection);
				x86CodeGen x86CodeGen = new x86CodeGen();
				x86Register? x86Register;
				do
				{
					ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
					{
						Variable = variable
					}, new VariableExpression
					{
						Variable = variable2
					}, 4, out this.expression, out this.inverse);
					x86Register = x86CodeGen.GenerateX86(this.inverse, (Variable v, x86Register r) => new x86Instruction[]
					{
						x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
						{
							new x86RegisterOperand(r)
						})
					});
				}
				while (x86Register == null);
				this.code = CodeGenUtils.AssembleCode(x86CodeGen, x86Register.Value);
				this.expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
				{
					Tuple.Create<string, Type>("{VAR}", typeof(int))
				}).GenerateCIL(this.expression).Compile<Func<int, int>>();
				ctx.Context.CurrentModuleWriterOptions.WriterEvent += this.InjectNativeCode;
			}

			// Token: 0x060002DF RID: 735 RVA: 0x000140E0 File Offset: 0x000122E0
			private void InjectNativeCode(object sender, ModuleWriterEventArgs e)
			{
				ModuleWriterBase writer = e.Writer;
				ModuleWriterEvent @event = e.Event;
				ModuleWriterEvent moduleWriterEvent = @event;
				if (moduleWriterEvent != ModuleWriterEvent.MDEndWriteMethodBodies)
				{
					if (moduleWriterEvent == ModuleWriterEvent.EndCalculateRvasAndFileOffsets)
					{
						uint rid = writer.Metadata.GetRid(this.native);
						RawMethodRow rawMethodRow = writer.Metadata.TablesHeap.MethodTable[rid];
						writer.Metadata.TablesHeap.MethodTable[rid] = new RawMethodRow((uint)this.codeChunk.RVA, rawMethodRow.ImplFlags, rawMethodRow.Flags, rawMethodRow.Name, rawMethodRow.Signature, rawMethodRow.ParamList);
					}
				}
				else
				{
					this.codeChunk = writer.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.code));
				}
			}

			// Token: 0x0400014D RID: 333
			private byte[] code;

			// Token: 0x0400014E RID: 334
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x0400014F RID: 335
			public Func<int, int> expCompiled;

			// Token: 0x04000150 RID: 336
			private Expression expression;

			// Token: 0x04000151 RID: 337
			private Expression inverse;

			// Token: 0x04000152 RID: 338
			public MethodDef native;
		}
	}
}
