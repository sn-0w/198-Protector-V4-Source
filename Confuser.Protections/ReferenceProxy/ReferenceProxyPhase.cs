using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x0200006F RID: 111
	internal class ReferenceProxyPhase : ProtectionPhase
	{
		// Token: 0x06000216 RID: 534 RVA: 0x00002136 File Offset: 0x00000336
		public ReferenceProxyPhase(ReferenceProxyProtection parent) : base(parent)
		{
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000217 RID: 535 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000218 RID: 536 RVA: 0x00002CBE File Offset: 0x00000EBE
		public override string Name
		{
			get
			{
				return "Encoding reference proxies";
			}
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000C450 File Offset: 0x0000A650
		private RPContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, ReferenceProxyPhase.RPStore store)
		{
			RPContext rpcontext = new RPContext();
			rpcontext.Mode = parameters.GetParameter<Mode>(context, method, "mode", Mode.Strong);
			rpcontext.Encoding = parameters.GetParameter<EncodingType>(context, method, "encoding", EncodingType.x86);
			rpcontext.InternalAlso = parameters.GetParameter<bool>(context, method, "internal", true);
			rpcontext.TypeErasure = parameters.GetParameter<bool>(context, method, "typeErasure", true);
			rpcontext.Depth = parameters.GetParameter<int>(context, method, "depth", 5);
			rpcontext.Module = method.Module;
			rpcontext.Method = method;
			rpcontext.Body = method.Body;
			rpcontext.BranchTargets = new HashSet<Instruction>(from target in (from instr in method.Body.Instructions
			select instr.Operand as Instruction).Concat((from instr in method.Body.Instructions
			where instr.Operand is Instruction[]
			select instr).SelectMany((Instruction instr) => (Instruction[])instr.Operand))
			where target != null
			select target);
			rpcontext.Protection = (ReferenceProxyProtection)base.Parent;
			rpcontext.Random = store.random;
			rpcontext.Context = context;
			rpcontext.Marker = context.Registry.GetService<IMarkerService>();
			rpcontext.DynCipher = context.Registry.GetService<IDynCipherService>();
			rpcontext.Name = context.Registry.GetService<INameService>();
			rpcontext.Delegates = store.delegates;
			Mode mode = rpcontext.Mode;
			Mode mode2 = mode;
			if (mode2 != Mode.Mild)
			{
				if (mode2 != Mode.Strong)
				{
					throw new UnreachableException();
				}
				RPContext rpcontext2 = rpcontext;
				StrongMode modeHandler;
				if ((modeHandler = store.strong) == null)
				{
					modeHandler = (store.strong = new StrongMode());
				}
				rpcontext2.ModeHandler = modeHandler;
			}
			else
			{
				RPContext rpcontext3 = rpcontext;
				MildMode modeHandler2;
				if ((modeHandler2 = store.mild) == null)
				{
					modeHandler2 = (store.mild = new MildMode());
				}
				rpcontext3.ModeHandler = modeHandler2;
			}
			switch (rpcontext.Encoding)
			{
			case EncodingType.Normal:
			{
				RPContext rpcontext4 = rpcontext;
				NormalEncoding encodingHandler;
				if ((encodingHandler = store.normal) == null)
				{
					encodingHandler = (store.normal = new NormalEncoding());
				}
				rpcontext4.EncodingHandler = encodingHandler;
				break;
			}
			case EncodingType.Expression:
			{
				RPContext rpcontext5 = rpcontext;
				ExpressionEncoding encodingHandler2;
				if ((encodingHandler2 = store.expression) == null)
				{
					encodingHandler2 = (store.expression = new ExpressionEncoding());
				}
				rpcontext5.EncodingHandler = encodingHandler2;
				break;
			}
			case EncodingType.x86:
			{
				RPContext rpcontext6 = rpcontext;
				x86Encoding encodingHandler3;
				if ((encodingHandler3 = store.x86) == null)
				{
					encodingHandler3 = (store.x86 = new x86Encoding());
				}
				rpcontext6.EncodingHandler = encodingHandler3;
				bool flag = (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U;
				if (flag)
				{
					context.CurrentModuleWriterOptions.Cor20HeaderOptions.Flags &= ~ComImageFlags.ILOnly;
				}
				break;
			}
			default:
				throw new UnreachableException();
			}
			return rpcontext;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000C754 File Offset: 0x0000A954
		private static RPContext ParseParameters(ModuleDef module, ConfuserContext context, ProtectionParameters parameters, ReferenceProxyPhase.RPStore store)
		{
			return new RPContext
			{
				Depth = parameters.GetParameter<int>(context, module, "depth", 3),
				InitCount = parameters.GetParameter<int>(context, module, "initCount", 16),
				Random = store.random,
				Module = module,
				Context = context,
				Marker = context.Registry.GetService<IMarkerService>(),
				DynCipher = context.Registry.GetService<IDynCipherService>(),
				Name = context.Registry.GetService<INameService>(),
				Delegates = store.delegates
			};
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000C7F0 File Offset: 0x0000A9F0
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.RefProxy");
			ReferenceProxyPhase.RPStore rpstore = new ReferenceProxyPhase.RPStore
			{
				random = randomGenerator
			};
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				bool flag = methodDef.HasBody && methodDef.Body.Instructions.Count > 0;
				if (flag)
				{
					this.ProcessMethod(this.ParseParameters(methodDef, context, parameters, rpstore));
					context.CheckCancellation();
				}
			}
			RPContext ctx = ReferenceProxyPhase.ParseParameters(context.CurrentModule, context, parameters, rpstore);
			bool flag2 = rpstore.mild != null;
			if (flag2)
			{
				rpstore.mild.Finalize(ctx);
			}
			bool flag3 = rpstore.strong != null;
			if (flag3)
			{
				rpstore.strong.Finalize(ctx);
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000C8F8 File Offset: 0x0000AAF8
		private void ProcessMethod(RPContext ctx)
		{
			int i = 0;
			while (i < ctx.Body.Instructions.Count)
			{
				Instruction instruction = ctx.Body.Instructions[i];
				bool flag = instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt || instruction.OpCode.Code == Code.Newobj;
				if (flag)
				{
					IMethod method = (IMethod)instruction.Operand;
					MethodDef methodDef = method.ResolveMethodDef();
					bool flag2 = methodDef != null && ctx.Context.Annotations.Get<object>(methodDef, ReferenceProxyProtection.TargetExcluded, null) != null;
					if (flag2)
					{
						break;
					}
					bool flag3 = instruction.OpCode.Code != Code.Newobj && method.Name == ".ctor";
					if (!flag3)
					{
						bool flag4 = method is MethodDef && !ctx.InternalAlso;
						if (!flag4)
						{
							bool flag5 = method is MethodSpec;
							if (!flag5)
							{
								bool flag6 = method.DeclaringType is TypeSpec;
								if (!flag6)
								{
									bool flag7 = method.MethodSig.ParamsAfterSentinel != null && method.MethodSig.ParamsAfterSentinel.Count > 0;
									if (!flag7)
									{
										TypeDef typeDef = method.DeclaringType.ResolveTypeDefThrow();
										bool flag8 = typeDef.IsDelegate();
										if (!flag8)
										{
											bool flag9 = typeDef.IsValueType && method.MethodSig.HasThis;
											if (!flag9)
											{
												bool flag10 = i - 1 >= 0 && ctx.Body.Instructions[i - 1].OpCode.OpCodeType == OpCodeType.Prefix;
												if (!flag10)
												{
													ctx.ModeHandler.ProcessCall(ctx, i);
												}
											}
										}
									}
								}
							}
						}
					}
				}
				IL_1B4:
				i++;
				continue;
				goto IL_1B4;
			}
		}

		// Token: 0x02000070 RID: 112
		private class RPStore
		{
			// Token: 0x040000A3 RID: 163
			public readonly Dictionary<MethodSig, TypeDef> delegates = new Dictionary<MethodSig, TypeDef>(new ReferenceProxyPhase.RPStore.MethodSigComparer());

			// Token: 0x040000A4 RID: 164
			public ExpressionEncoding expression;

			// Token: 0x040000A5 RID: 165
			public MildMode mild;

			// Token: 0x040000A6 RID: 166
			public RandomGenerator random;

			// Token: 0x040000A7 RID: 167
			public NormalEncoding normal;

			// Token: 0x040000A8 RID: 168
			public StrongMode strong;

			// Token: 0x040000A9 RID: 169
			public x86Encoding x86;

			// Token: 0x02000071 RID: 113
			private class MethodSigComparer : IEqualityComparer<MethodSig>
			{
				// Token: 0x0600021E RID: 542 RVA: 0x0000CADC File Offset: 0x0000ACDC
				public bool Equals(MethodSig x, MethodSig y)
				{
					return default(SigComparer).Equals(x, y);
				}

				// Token: 0x0600021F RID: 543 RVA: 0x0000CAFC File Offset: 0x0000ACFC
				public int GetHashCode(MethodSig obj)
				{
					return default(SigComparer).GetHashCode(obj);
				}
			}
		}
	}
}
