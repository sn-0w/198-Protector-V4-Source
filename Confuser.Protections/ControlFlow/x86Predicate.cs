using System;
using System.Collections.Generic;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.DynCipher.AST;
using Confuser.DynCipher.Generation;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x0200008E RID: 142
	internal class x86Predicate : IPredicate
	{
		// Token: 0x06000284 RID: 644 RVA: 0x00002F33 File Offset: 0x00001133
		public x86Predicate(CFContext ctx)
		{
			this.ctx = ctx;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x000102F4 File Offset: 0x0000E4F4
		public void Init(CilBody body)
		{
			bool flag = this.inited;
			if (!flag)
			{
				this.encoding = this.ctx.Context.Annotations.Get<x86Predicate.x86Encoding>(this.ctx.Method.DeclaringType, x86Predicate.Encoding, null);
				bool flag2 = this.encoding == null;
				if (flag2)
				{
					this.encoding = new x86Predicate.x86Encoding();
					this.encoding.Compile(this.ctx);
					this.ctx.Context.Annotations.Set<x86Predicate.x86Encoding>(this.ctx.Method.DeclaringType, x86Predicate.Encoding, this.encoding);
				}
				this.inited = true;
			}
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00002F44 File Offset: 0x00001144
		public void EmitSwitchLoad(IList<Instruction> instrs)
		{
			instrs.Add(Instruction.Create(OpCodes.Call, this.encoding.native));
		}

		// Token: 0x06000287 RID: 647 RVA: 0x000103A8 File Offset: 0x0000E5A8
		public int GetSwitchKey(int key)
		{
			return this.encoding.expCompiled(key);
		}

		// Token: 0x040000F5 RID: 245
		private static readonly object Encoding = new object();

		// Token: 0x040000F6 RID: 246
		private readonly CFContext ctx;

		// Token: 0x040000F7 RID: 247
		private x86Predicate.x86Encoding encoding;

		// Token: 0x040000F8 RID: 248
		private bool inited;

		// Token: 0x0200008F RID: 143
		private class x86Encoding
		{
			// Token: 0x06000289 RID: 649 RVA: 0x000103CC File Offset: 0x0000E5CC
			public void Compile(CFContext ctx)
			{
				Variable variable = new Variable("{VAR}");
				Variable variable2 = new Variable("{RESULT}");
				CorLibTypeSig @int = ctx.Method.Module.CorLibTypes.Int32;
				this.native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(@int, @int), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
				this.native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
				ctx.Method.Module.GlobalType.Methods.Add(this.native);
				ctx.Context.Registry.GetService<IMarkerService>().Mark(this.native, ctx.Protection);
				ctx.Context.Registry.GetService<INameService>().SetCanRename(this.native, false);
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
					}, ctx.Depth, out this.expression, out this.inverse);
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

			// Token: 0x0600028A RID: 650 RVA: 0x000105A0 File Offset: 0x0000E7A0
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

			// Token: 0x040000F9 RID: 249
			private byte[] code;

			// Token: 0x040000FA RID: 250
			private dnlib.DotNet.Writer.MethodBody codeChunk;

			// Token: 0x040000FB RID: 251
			public Func<int, int> expCompiled;

			// Token: 0x040000FC RID: 252
			private Expression expression;

			// Token: 0x040000FD RID: 253
			private Expression inverse;

			// Token: 0x040000FE RID: 254
			public MethodDef native;
		}
	}
}
