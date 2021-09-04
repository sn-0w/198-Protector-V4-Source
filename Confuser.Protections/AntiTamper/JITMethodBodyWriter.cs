using System;
using System.Diagnostics;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000FA RID: 250
	internal sealed class JITMethodBodyWriter : MethodBodyWriterBase
	{
		// Token: 0x06000419 RID: 1049 RVA: 0x0001AB40 File Offset: 0x00018D40
		public JITMethodBodyWriter(Metadata md, CilBody body, JITMethodBody jitBody, uint mulSeed, bool keepMaxStack) : base(body.Instructions, body.ExceptionHandlers)
		{
			this._metadata = md;
			this._body = body;
			this._jitBody = jitBody;
			this._keepMaxStack = keepMaxStack;
			this._jitBody.MulSeed = mulSeed;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0001AB8C File Offset: 0x00018D8C
		public void Write()
		{
			uint num = base.InitializeInstructionOffsets();
			this._jitBody.MaxStack = (this._keepMaxStack ? ((uint)this._body.MaxStack) : base.GetMaxStack());
			this._jitBody.Options = 0U;
			bool initLocals = this._body.InitLocals;
			if (initLocals)
			{
				this._jitBody.Options |= 16U;
			}
			bool flag = this._body.Variables.Count > 0;
			if (flag)
			{
				LocalSig sig = new LocalSig((from var in this._body.Variables
				select var.Type).ToList<TypeSig>());
				this._jitBody.LocalVars = SignatureWriter.Write(this._metadata, sig);
			}
			else
			{
				this._jitBody.LocalVars = new byte[0];
			}
			byte[] array = new byte[num];
			ArrayWriter arrayWriter = new ArrayWriter(array);
			uint num2 = base.WriteInstructions(ref arrayWriter);
			Debug.Assert(num == num2);
			this._jitBody.ILCode = array;
			this._jitBody.ExceptionHandlers = new JITExceptionHandlerClause[this.exceptionHandlers.Count];
			bool flag2 = this.exceptionHandlers.Count <= 0;
			if (!flag2)
			{
				this._jitBody.Options |= 8U;
				for (int i = 0; i < this.exceptionHandlers.Count; i++)
				{
					ExceptionHandler exceptionHandler = this.exceptionHandlers[i];
					this._jitBody.ExceptionHandlers[i].Flags = (uint)exceptionHandler.HandlerType;
					uint offset = base.GetOffset(exceptionHandler.TryStart);
					uint offset2 = base.GetOffset(exceptionHandler.TryEnd);
					this._jitBody.ExceptionHandlers[i].TryOffset = offset;
					this._jitBody.ExceptionHandlers[i].TryLength = offset2 - offset;
					uint offset3 = base.GetOffset(exceptionHandler.HandlerStart);
					uint offset4 = base.GetOffset(exceptionHandler.HandlerEnd);
					this._jitBody.ExceptionHandlers[i].HandlerOffset = offset3;
					this._jitBody.ExceptionHandlers[i].HandlerLength = offset4 - offset3;
					ExceptionHandlerType handlerType = exceptionHandler.HandlerType;
					ExceptionHandlerType exceptionHandlerType = handlerType;
					if (exceptionHandlerType != ExceptionHandlerType.Catch)
					{
						if (exceptionHandlerType == ExceptionHandlerType.Filter)
						{
							this._jitBody.ExceptionHandlers[i].ClassTokenOrFilterOffset = base.GetOffset(exceptionHandler.FilterStart);
						}
					}
					else
					{
						uint raw = this._metadata.GetToken(exceptionHandler.CatchType).Raw;
						bool flag3 = (raw & 4278190080U) == 452984832U;
						if (flag3)
						{
							this._jitBody.Options |= 128U;
						}
						this._jitBody.ExceptionHandlers[i].ClassTokenOrFilterOffset = raw;
					}
				}
			}
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineField(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineMethod(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineSig(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineString(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineTok(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0001AEA0 File Offset: 0x000190A0
		protected override void WriteInlineType(ref ArrayWriter writer, Instruction instr)
		{
			writer.WriteUInt32(this._metadata.GetToken(instr.Operand).Raw);
		}

		// Token: 0x04000204 RID: 516
		private readonly CilBody _body;

		// Token: 0x04000205 RID: 517
		private readonly JITMethodBody _jitBody;

		// Token: 0x04000206 RID: 518
		private readonly bool _keepMaxStack;

		// Token: 0x04000207 RID: 519
		private readonly Metadata _metadata;
	}
}
