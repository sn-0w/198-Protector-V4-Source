using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x0200013C RID: 316
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class DebuggerDetection : Protection
	{
		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x00003D6D File Offset: 0x00001F6D
		public override string Name
		{
			get
			{
				return "DebuggerDetection";
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x00003D74 File Offset: 0x00001F74
		public override string Author
		{
			get
			{
				return "Aptitude";
			}
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x00003D7B File Offset: 0x00001F7B
		public override string Description
		{
			get
			{
				return "Detect Dnspy,Ollydbg,hex,dumpers,jetbrains,Reflector";
			}
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x00003D6D File Offset: 0x00001F6D
		public override string Id
		{
			get
			{
				return "DebuggerDetection";
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000578 RID: 1400 RVA: 0x00003D82 File Offset: 0x00001F82
		public override string FullId
		{
			get
			{
				return "Ki.DebuggerDetection";
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x00003D89 File Offset: 0x00001F89
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new DebuggerDetection.AntiDebugPhase(this));
		}

		// Token: 0x04000294 RID: 660
		public const string _Id = "DebuggerDetection";

		// Token: 0x04000295 RID: 661
		public const string _FullId = "Ki.DebuggerDetection";

		// Token: 0x0200013D RID: 317
		private class AntiDebugPhase : ProtectionPhase
		{
			// Token: 0x0600057D RID: 1405 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiDebugPhase(DebuggerDetection parent) : base(parent)
			{
			}

			// Token: 0x1700019D RID: 413
			// (get) Token: 0x0600057E RID: 1406 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700019E RID: 414
			// (get) Token: 0x0600057F RID: 1407 RVA: 0x00003D98 File Offset: 0x00001F98
			public override string Name
			{
				get
				{
					return "DebuggerDetection injection";
				}
			}

			// Token: 0x06000580 RID: 1408 RVA: 0x0001F760 File Offset: 0x0001D960
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService service = context.Registry.GetService<IRuntimeService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				INameService service3 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					bool parameter = parameters.GetParameter<DebuggerDetection.AntiDebugPhase.AntiMode>(context, moduleDef, "mode", DebuggerDetection.AntiDebugPhase.AntiMode.Safe) != DebuggerDetection.AntiDebugPhase.AntiMode.Safe;
					TypeDef typeDef = null;
					if (parameter)
					{
						throw new UnreachableException();
					}
					TypeDef runtimeType = service.GetRuntimeType("Confuser.Runtime.RealAntiDebugger");
					typeDef = service.GetRuntimeType("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
					moduleDef.Types.Add(typeDef = InjectHelper.Inject(typeDef, moduleDef));
					foreach (IDnlibDef dnlibDef in typeDef.FindDefinitions())
					{
						service2.Mark(dnlibDef, (Protection)base.Parent);
						service3.Analyze(dnlibDef);
					}
					service3.SetCanRename(typeDef, false);
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Initialize");
					methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method2));
					foreach (IDnlibDef dnlibDef2 in enumerable)
					{
						service2.Mark(dnlibDef2, (Protection)base.Parent);
						service3.Analyze(dnlibDef2);
						bool flag = true;
						if (dnlibDef2 is MethodDef)
						{
							MethodDef methodDef2 = (MethodDef)dnlibDef2;
							if (methodDef2.Access == MethodAttributes.Public)
							{
								methodDef2.Access = MethodAttributes.Assembly;
							}
							if (!methodDef2.IsConstructor)
							{
								methodDef2.IsSpecialName = false;
							}
							else
							{
								flag = false;
							}
							CustomAttribute customAttribute = methodDef2.CustomAttributes.Find("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
							if (customAttribute != null)
							{
								customAttribute.Constructor = typeDef.FindMethod(".ctor");
							}
						}
						else if (dnlibDef2 is FieldDef)
						{
							FieldDef fieldDef = (FieldDef)dnlibDef2;
							if (fieldDef.Access == FieldAttributes.Public)
							{
								fieldDef.Access = FieldAttributes.Assembly;
							}
							if (fieldDef.IsLiteral)
							{
								fieldDef.DeclaringType.Fields.Remove(fieldDef);
								continue;
							}
						}
						if (flag)
						{
							dnlibDef2.Name = service3.ObfuscateName(dnlibDef2.Name, RenameMode.Unicode);
							service3.SetCanRename(dnlibDef2, false);
						}
					}
				}
			}

			// Token: 0x0200013E RID: 318
			private enum AntiMode
			{
				// Token: 0x04000297 RID: 663
				Safe,
				// Token: 0x04000298 RID: 664
				Win32,
				// Token: 0x04000299 RID: 665
				Antinet
			}
		}
	}
}
