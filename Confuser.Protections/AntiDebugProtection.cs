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
	// Token: 0x0200001B RID: 27
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class AntiDebugProtection : Protection
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00005810 File Offset: 0x00003A10
		public override string Name
		{
			get
			{
				return "Anti Debug Protection";
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600008C RID: 140 RVA: 0x00005828 File Offset: 0x00003A28
		public override string Description
		{
			get
			{
				return "This protection prevents the assembly from being debugged or profiled.";
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600008D RID: 141 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00005840 File Offset: 0x00003A40
		public override string Id
		{
			get
			{
				return "anti debug";
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00005858 File Offset: 0x00003A58
		public override string FullId
		{
			get
			{
				return "Ki.AntiDebug";
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000090 RID: 144 RVA: 0x00004090 File Offset: 0x00002290
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00002416 File Offset: 0x00000616
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiDebugProtection.AntiDebugPhase(this));
		}

		// Token: 0x04000027 RID: 39
		public const string _Id = "anti debug";

		// Token: 0x04000028 RID: 40
		public const string _FullId = "Ki.AntiDebug";

		// Token: 0x0200001C RID: 28
		private class AntiDebugPhase : ProtectionPhase
		{
			// Token: 0x06000094 RID: 148 RVA: 0x00002136 File Offset: 0x00000336
			public AntiDebugPhase(AntiDebugProtection parent) : base(parent)
			{
			}

			// Token: 0x17000048 RID: 72
			// (get) Token: 0x06000095 RID: 149 RVA: 0x000040A4 File Offset: 0x000022A4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000049 RID: 73
			// (get) Token: 0x06000096 RID: 150 RVA: 0x00005870 File Offset: 0x00003A70
			public override string Name
			{
				get
				{
					return "Anti-debug injection";
				}
			}

			// Token: 0x06000097 RID: 151 RVA: 0x00005888 File Offset: 0x00003A88
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				IRuntimeService service = context.Registry.GetService<IRuntimeService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				INameService service3 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					AntiDebugProtection.AntiDebugPhase.AntiMode parameter = parameters.GetParameter<AntiDebugProtection.AntiDebugPhase.AntiMode>(context, moduleDef, "mode", AntiDebugProtection.AntiDebugPhase.AntiMode.Ultimate);
					TypeDef typeDef = null;
					TypeDef runtimeType;
					switch (parameter)
					{
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Safe:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugSafe");
						break;
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Win32:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugWin32");
						break;
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Antinet:
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
					case AntiDebugProtection.AntiDebugPhase.AntiMode.Ultimate:
						runtimeType = service.GetRuntimeType("Confuser.Runtime.AntiDebugUltimate");
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
						bool flag2 = dnlibDef2 is MethodDef;
						if (flag2)
						{
							MethodDef methodDef2 = (MethodDef)dnlibDef2;
							bool flag3 = methodDef2.Access == MethodAttributes.Public;
							if (flag3)
							{
								methodDef2.Access = MethodAttributes.Assembly;
							}
							bool flag4 = !methodDef2.IsConstructor;
							if (flag4)
							{
								methodDef2.IsSpecialName = false;
							}
							else
							{
								flag = false;
							}
							CustomAttribute customAttribute = methodDef2.CustomAttributes.Find("System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute");
							bool flag5 = customAttribute != null;
							if (flag5)
							{
								customAttribute.Constructor = typeDef.FindMethod(".ctor");
							}
						}
						else
						{
							bool flag6 = dnlibDef2 is FieldDef;
							if (flag6)
							{
								FieldDef fieldDef = (FieldDef)dnlibDef2;
								bool flag7 = fieldDef.Access == FieldAttributes.Public;
								if (flag7)
								{
									fieldDef.Access = FieldAttributes.Assembly;
								}
								bool isLiteral = fieldDef.IsLiteral;
								if (isLiteral)
								{
									fieldDef.DeclaringType.Fields.Remove(fieldDef);
									continue;
								}
							}
						}
						bool flag8 = flag;
						if (flag8)
						{
							dnlibDef2.Name = service3.ObfuscateName(dnlibDef2.Name, RenameMode.RealNames);
							service3.SetCanRename(dnlibDef2, false);
						}
					}
				}
			}

			// Token: 0x0200001D RID: 29
			private enum AntiMode
			{
				// Token: 0x0400002A RID: 42
				Safe,
				// Token: 0x0400002B RID: 43
				Win32,
				// Token: 0x0400002C RID: 44
				Antinet,
				// Token: 0x0400002D RID: 45
				Ultimate
			}
		}
	}
}
