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
	// Token: 0x02000140 RID: 320
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class SuperAntiDe4Dot : Protection
	{
		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000584 RID: 1412 RVA: 0x00003DAB File Offset: 0x00001FAB
		public override string Name
		{
			get
			{
				return "SuperAntiDe4Dot";
			}
		}

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x00003DB2 File Offset: 0x00001FB2
		public override string Description
		{
			get
			{
				return "This will fuck de4dot in the ass @ renaming";
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x00003DAB File Offset: 0x00001FAB
		public override string Id
		{
			get
			{
				return "SuperAntiDe4Dot";
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x00003DB9 File Offset: 0x00001FB9
		public override string FullId
		{
			get
			{
				return "Ki.SuperAntiDe4Dot";
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000589 RID: 1417 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00003DC0 File Offset: 0x00001FC0
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new SuperAntiDe4Dot.AntiDe4dot(this));
		}

		// Token: 0x0400029C RID: 668
		public const string _Id = "SuperAntiDe4Dot";

		// Token: 0x0400029D RID: 669
		public const string _FullId = "Ki.SuperAntiDe4Dot";

		// Token: 0x02000141 RID: 321
		private class AntiDe4dot : ProtectionPhase
		{
			// Token: 0x0600058D RID: 1421 RVA: 0x00002E01 File Offset: 0x00001001
			public AntiDe4dot(SuperAntiDe4Dot parent) : base(parent)
			{
			}

			// Token: 0x170001A5 RID: 421
			// (get) Token: 0x0600058E RID: 1422 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x170001A6 RID: 422
			// (get) Token: 0x0600058F RID: 1423 RVA: 0x00003DCF File Offset: 0x00001FCF
			public override string Name
			{
				get
				{
					return "SuperAntiDe4Dot by Aptitude Injection";
				}
			}

			// Token: 0x06000590 RID: 1424 RVA: 0x0001FA54 File Offset: 0x0001DC54
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService service = context.Registry.GetService<IRuntimeService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				INameService service3 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					SuperAntiDe4Dot.AntiDe4dot.AntiMode parameter = parameters.GetParameter<SuperAntiDe4Dot.AntiDe4dot.AntiMode>(context, moduleDef, "mode", SuperAntiDe4Dot.AntiDe4dot.AntiMode.Safe);
					TypeDef typeDef = null;
					TypeDef runtimeType;
					switch (parameter)
					{
					case SuperAntiDe4Dot.AntiDe4dot.AntiMode.Safe:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.SuperAntiDe4Dot");
						break;
					case SuperAntiDe4Dot.AntiDe4dot.AntiMode.Win32:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugWin32");
						break;
					case SuperAntiDe4Dot.AntiDe4dot.AntiMode.Antinet:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugAntinet");
						typeDef = service.GetRuntimeType("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
						moduleDef.Types.Add(typeDef = InjectHelper.Inject(typeDef, moduleDef));
						foreach (IDnlibDef dnlibDef in typeDef.FindDefinitions())
						{
							service2.Mark(dnlibDef, (Protection)base.Parent);
							service3.Analyze(dnlibDef);
						}
						service3.SetCanRename(typeDef, false);
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
							dnlibDef2.Name = service3.ObfuscateName(dnlibDef2.Name, RenameMode.Unicode);
							service3.SetCanRename(dnlibDef2, false);
						}
					}
				}
			}

			// Token: 0x02000142 RID: 322
			private enum AntiMode
			{
				// Token: 0x0400029F RID: 671
				Safe,
				// Token: 0x040002A0 RID: 672
				Win32,
				// Token: 0x040002A1 RID: 673
				Antinet
			}
		}
	}
}
