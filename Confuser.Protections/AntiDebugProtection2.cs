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
	// Token: 0x02000137 RID: 311
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDebugProtection2 : Protection
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00003D36 File Offset: 0x00001F36
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDebugProtection2.AntiDebugPhase2(this));
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000567 RID: 1383 RVA: 0x00003D45 File Offset: 0x00001F45
		public override string Description
		{
			get
			{
				return "Working Anti Dn-Spy Debugging | Aptitude";
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x00003D4C File Offset: 0x00001F4C
		public override string FullId
		{
			get
			{
				return "Ki.AntiDebug2";
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x00003D53 File Offset: 0x00001F53
		public override string Id
		{
			get
			{
				return "Anti Debug Antinet";
			}
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x00003D53 File Offset: 0x00001F53
		public override string Name
		{
			get
			{
				return "Anti Debug Antinet";
			}
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x0600056B RID: 1387 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x04000288 RID: 648
		public const string _FullId = "Ki.AntiDebug2";

		// Token: 0x04000289 RID: 649
		public const string _Id = "Anti Debug Antinet";

		// Token: 0x02000138 RID: 312
		private class AntiDebugPhase2 : ProtectionPhase
		{
			// Token: 0x0600056D RID: 1389 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiDebugPhase2(AntiDebugProtection2 parent) : base(parent)
			{
			}

			// Token: 0x0600056E RID: 1390 RVA: 0x0001F430 File Offset: 0x0001D630
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService service = context.Registry.GetService<IRuntimeService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				INameService service3 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					AntiDebugProtection2.AntiDebugPhase2.AntiMode parameter = parameters.GetParameter<AntiDebugProtection2.AntiDebugPhase2.AntiMode>(context, moduleDef, "mode", AntiDebugProtection2.AntiDebugPhase2.AntiMode.Antinet);
					TypeDef typeDef = null;
					TypeDef runtimeType;
					switch (parameter)
					{
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Safe:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugSafe");
						break;
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Win32:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugWin32");
						break;
					case AntiDebugProtection2.AntiDebugPhase2.AntiMode.Antinet:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugAntinet");
						typeDef = service.GetRuntimeType("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
						moduleDef.Types.Add(typeDef = InjectHelper.Inject(typeDef, moduleDef));
						foreach (IDnlibDef dnlibDef in typeDef.FindDefinitions())
						{
							service2.Mark(dnlibDef, (Protection)base.Parent);
							service3.Analyze(dnlibDef);
						}
						service3.SetCanRename(typeDef, true);
						break;
					default:
						throw new UnreachableException();
					}
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
							dnlibDef2.Name = service3.ObfuscateName(dnlibDef2.Name, RenameMode.Sequential);
							service3.SetCanRename(dnlibDef2, true);
						}
					}
				}
			}

			// Token: 0x17000195 RID: 405
			// (get) Token: 0x0600056F RID: 1391 RVA: 0x00003D5A File Offset: 0x00001F5A
			public override string Name
			{
				get
				{
					return "Anti-debug injection";
				}
			}

			// Token: 0x17000196 RID: 406
			// (get) Token: 0x06000570 RID: 1392 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x02000139 RID: 313
			private enum AntiMode
			{
				// Token: 0x0400028B RID: 651
				Safe,
				// Token: 0x0400028C RID: 652
				Win32,
				// Token: 0x0400028D RID: 653
				Antinet
			}
		}
	}
}
