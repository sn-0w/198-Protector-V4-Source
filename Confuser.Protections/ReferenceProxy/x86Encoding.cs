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

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200005B RID: 91
	internal class x86Encoding : IRPEncoding
	{
		// Token: 0x060001D1 RID: 465 RVA: 0x0000A20C File Offset: 0x0000840C
		public Instruction[] EmitDecode(MethodDef init, RPContext ctx, Instruction[] arg)
		{
			Tuple<MethodDef, Func<int, int>> key = this.GetKey(ctx, init);
			List<Instruction> list = new List<Instruction>();
			list.AddRange(arg);
			list.Add(Instruction.Create(OpCodes.Call, key.Item1));
			return list.ToArray();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000A254 File Offset: 0x00008454
		public int Encode(MethodDef init, RPContext ctx, int value)
		{
			Tuple<MethodDef, Func<int, int>> key = this.GetKey(ctx, init);
			return key.Item2(value);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0000A27C File Offset: 0x0000847C
		private void Compile(RPContext ctx, out Func<int, int> expCompiled, out MethodDef native)
		{
			Variable variable = new Variable("{VAR}");
			Variable variable2 = new Variable("{RESULT}");
			CorLibTypeSig @int = ctx.Module.CorLibTypes.Int32;
			native = new MethodDefUser(ctx.Context.Registry.GetService<INameService>().RandomName(), MethodSig.CreateStatic(@int, @int), MethodAttributes.Static | MethodAttributes.PinvokeImpl);
			native.ImplAttributes = (MethodImplAttributes.Native | MethodImplAttributes.ManagedMask | MethodImplAttributes.PreserveSig);
			ctx.Module.GlobalType.Methods.Add(native);
			ctx.Context.Registry.GetService<IMarkerService>().Mark(native, ctx.Protection);
			ctx.Context.Registry.GetService<INameService>().SetCanRename(native, false);
			x86CodeGen x86CodeGen = new x86CodeGen();
			Expression expression;
			x86Register? x86Register;
			do
			{
				Expression expression2;
				ctx.DynCipher.GenerateExpressionPair(ctx.Random, new VariableExpression
				{
					Variable = variable
				}, new VariableExpression
				{
					Variable = variable2
				}, ctx.Depth, out expression, out expression2);
				x86Register = x86CodeGen.GenerateX86(expression2, (Variable v, x86Register r) => new x86Instruction[]
				{
					x86Instruction.Create(x86OpCode.POP, new Ix86Operand[]
					{
						new x86RegisterOperand(r)
					})
				});
			}
			while (x86Register == null);
			byte[] item = CodeGenUtils.AssembleCode(x86CodeGen, x86Register.Value);
			expCompiled = new DMCodeGen(typeof(int), new Tuple<string, Type>[]
			{
				Tuple.Create<string, Type>("{VAR}", typeof(int))
			}).GenerateCIL(expression).Compile<Func<int, int>>();
			this.nativeCodes.Add(Tuple.Create<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(native, item, null));
			bool flag = !this.addedHandler;
			if (flag)
			{
				ctx.Context.CurrentModuleWriterOptions.WriterEvent += this.InjectNativeCode;
				this.addedHandler = true;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x0000A448 File Offset: 0x00008648
		private void InjectNativeCode(object sender, ModuleWriterEventArgs e)
		{
			ModuleWriterBase moduleWriterBase = (ModuleWriterBase)sender;
			bool flag = e.Event == ModuleWriterEvent.MDEndWriteMethodBodies;
			if (flag)
			{
				for (int i = 0; i < this.nativeCodes.Count; i++)
				{
					this.nativeCodes[i] = new Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>(this.nativeCodes[i].Item1, this.nativeCodes[i].Item2, moduleWriterBase.MethodBodies.Add(new dnlib.DotNet.Writer.MethodBody(this.nativeCodes[i].Item2)));
				}
			}
			else
			{
				bool flag2 = e.Event == ModuleWriterEvent.EndCalculateRvasAndFileOffsets;
				if (flag2)
				{
					foreach (Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody> tuple in this.nativeCodes)
					{
						uint rid = moduleWriterBase.Metadata.GetRid(tuple.Item1);
						RawMethodRow rawMethodRow = moduleWriterBase.Metadata.TablesHeap.MethodTable[rid];
						moduleWriterBase.Metadata.TablesHeap.MethodTable[rid] = new RawMethodRow((uint)tuple.Item3.RVA, rawMethodRow.ImplFlags, rawMethodRow.Flags, rawMethodRow.Name, rawMethodRow.Signature, rawMethodRow.ParamList);
					}
				}
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000A5BC File Offset: 0x000087BC
		private Tuple<MethodDef, Func<int, int>> GetKey(RPContext ctx, MethodDef init)
		{
			Tuple<MethodDef, Func<int, int>> result;
			bool flag = !this.keys.TryGetValue(init, out result);
			if (flag)
			{
				Func<int, int> item;
				MethodDef item2;
				this.Compile(ctx, out item, out item2);
				result = (this.keys[init] = Tuple.Create<MethodDef, Func<int, int>>(item2, item));
			}
			return result;
		}

		// Token: 0x04000078 RID: 120
		private readonly Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>> keys = new Dictionary<MethodDef, Tuple<MethodDef, Func<int, int>>>();

		// Token: 0x04000079 RID: 121
		private readonly List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>> nativeCodes = new List<Tuple<MethodDef, byte[], dnlib.DotNet.Writer.MethodBody>>();

		// Token: 0x0400007A RID: 122
		private bool addedHandler;

		// Token: 0x0200005C RID: 92
		private class CodeGen : CILCodeGen
		{
			// Token: 0x060001D7 RID: 471 RVA: 0x00002BCC File Offset: 0x00000DCC
			public CodeGen(Instruction[] arg, MethodDef method, IList<Instruction> instrs) : base(method, instrs)
			{
				this.arg = arg;
			}

			// Token: 0x060001D8 RID: 472 RVA: 0x0000A60C File Offset: 0x0000880C
			protected override void LoadVar(Variable var)
			{
				bool flag = var.Name == "{RESULT}";
				if (flag)
				{
					foreach (Instruction instr in this.arg)
					{
						base.Emit(instr);
					}
				}
				else
				{
					base.LoadVar(var);
				}
			}

			// Token: 0x0400007B RID: 123
			private readonly Instruction[] arg;
		}
	}
}
