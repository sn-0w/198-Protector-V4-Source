using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.MD;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000085 RID: 133
	internal class ControlFlowPhase : ProtectionPhase
	{
		// Token: 0x0600025A RID: 602 RVA: 0x00002E01 File Offset: 0x00001001
		public ControlFlowPhase(ControlFlowProtection parent) : base(parent)
		{
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600025B RID: 603 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Methods;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00002E0A File Offset: 0x0000100A
		public override string Name
		{
			get
			{
				return "Control flow mangling";
			}
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000EF7C File Offset: 0x0000D17C
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool disableOpti = ControlFlowPhase.DisabledOptimization(context.CurrentModule);
			RandomGenerator randomGenerator = context.Registry.GetService<IRandomService>().GetRandomGenerator("Ki.ControlFlow");
			foreach (MethodDef methodDef in parameters.Targets.OfType<MethodDef>().WithProgress(context.Logger))
			{
				if (methodDef.HasBody && methodDef.Body.Instructions.Count > 0)
				{
					this.ProcessMethod(methodDef.Body, this.ParseParameters(methodDef, context, parameters, randomGenerator, disableOpti));
					context.CheckCancellation();
				}
			}
		}

		// Token: 0x0600025F RID: 607 RVA: 0x0000F034 File Offset: 0x0000D234
		private static bool DisabledOptimization(ModuleDef module)
		{
			bool flag = false;
			CustomAttribute customAttribute = module.Assembly.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			if (customAttribute != null)
			{
				if (customAttribute.ConstructorArguments.Count != 1)
				{
					flag |= (bool)customAttribute.ConstructorArguments[1].Value;
				}
				else
				{
					flag |= (((int)customAttribute.ConstructorArguments[0].Value & 256) != 0);
				}
			}
			customAttribute = module.CustomAttributes.Find("System.Diagnostics.DebuggableAttribute");
			if (customAttribute != null)
			{
				if (customAttribute.ConstructorArguments.Count != 1)
				{
					flag |= (bool)customAttribute.ConstructorArguments[1].Value;
				}
				else
				{
					flag |= (((int)customAttribute.ConstructorArguments[0].Value & 256) != 0);
				}
			}
			return flag;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000F124 File Offset: 0x0000D324
		private static ManglerBase GetMangler(CFType type)
		{
			ManglerBase result;
			if (type > CFType.Switch)
			{
				result = ControlFlowPhase.Jump;
			}
			else
			{
				result = ControlFlowPhase.Switch;
			}
			return result;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000F148 File Offset: 0x0000D348
		private CFContext ParseParameters(MethodDef method, ConfuserContext context, ProtectionParameters parameters, RandomGenerator random, bool disableOpti)
		{
			CFContext cfcontext = new CFContext();
			cfcontext.Type = parameters.GetParameter<CFType>(context, method, "type", CFType.Switch);
			cfcontext.Predicate = parameters.GetParameter<PredicateType>(context, method, "predicate", PredicateType.Expression);
			int parameter = parameters.GetParameter<int>(context, method, "intensity", 120);
			cfcontext.Intensity = (double)parameter / 120.0;
			cfcontext.Depth = parameters.GetParameter<int>(context, method, "depth", 20);
			cfcontext.JunkCode = (parameters.GetParameter<bool>(context, method, "junk", true) && !disableOpti);
			cfcontext.Protection = (ControlFlowProtection)base.Parent;
			cfcontext.Random = random;
			cfcontext.Method = method;
			cfcontext.Context = context;
			cfcontext.DynCipher = context.Registry.GetService<IDynCipherService>();
			if (cfcontext.Predicate == PredicateType.x86 && (context.CurrentModule.Cor20HeaderFlags & ComImageFlags.ILOnly) > (ComImageFlags)0U)
			{
				Cor20HeaderOptions cor20HeaderOptions = context.CurrentModuleWriterOptions.Cor20HeaderOptions;
				ComImageFlags? flags = cor20HeaderOptions.Flags;
				ComImageFlags? flags2;
				if (flags != null)
				{
					flags2 = new ComImageFlags?((flags.GetValueOrDefault() & ComImageFlags.ILLibrary) | ComImageFlags.StrongNameSigned | ComImageFlags.NativeEntryPoint | ComImageFlags.TrackDebugData);
				}
				else
				{
					flags2 = null;
				}
				cor20HeaderOptions.Flags = flags2;
			}
			return cfcontext;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000F274 File Offset: 0x0000D474
		private void ProcessMethod(CilBody body, CFContext ctx)
		{
			uint num;
			if (!MaxStackCalculator.GetMaxStack(body.Instructions, body.ExceptionHandlers, out num))
			{
				ctx.Context.Logger.Error("Failed to calcuate maxstack.");
				throw new ConfuserException(null);
			}
			body.MaxStack = (ushort)num;
			ScopeBlock scopeBlock = BlockParser.ParseBody(body);
			ControlFlowPhase.GetMangler(ctx.Type).Mangle(body, scopeBlock, ctx);
			body.Instructions.Clear();
			scopeBlock.ToBody(body);
			foreach (ExceptionHandler exceptionHandler in body.ExceptionHandlers)
			{
				int num2 = body.Instructions.IndexOf(exceptionHandler.TryEnd) + 1;
				ExceptionHandler exceptionHandler2 = exceptionHandler;
				Instruction tryEnd;
				if (num2 < body.Instructions.Count)
				{
					tryEnd = body.Instructions[num2];
				}
				else
				{
					tryEnd = null;
				}
				exceptionHandler2.TryEnd = tryEnd;
				num2 = body.Instructions.IndexOf(exceptionHandler.HandlerEnd) + 1;
				ExceptionHandler exceptionHandler3 = exceptionHandler;
				Instruction handlerEnd;
				if (num2 < body.Instructions.Count)
				{
					handlerEnd = body.Instructions[num2];
				}
				else
				{
					handlerEnd = null;
				}
				exceptionHandler3.HandlerEnd = handlerEnd;
			}
			body.KeepOldMaxStack = true;
		}

		// Token: 0x040000E4 RID: 228
		private static readonly JumpMangler Jump = new JumpMangler();

		// Token: 0x040000E5 RID: 229
		private static readonly SwitchMangler Switch = new SwitchMangler();
	}
}
