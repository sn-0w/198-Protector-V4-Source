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
	// Token: 0x0200012F RID: 303
	internal class OverwritesHeadersProtection : Protection
	{
		// Token: 0x06000534 RID: 1332 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00003C31 File Offset: 0x00001E31
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.EndModule, new OverwritesHeadersProtection.Overwrite(this));
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x00003C40 File Offset: 0x00001E40
		public override string Description
		{
			get
			{
				return "This protection overwrites the whole PE Header.";
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x00003C47 File Offset: 0x00001E47
		public override string FullId
		{
			get
			{
				return "Ki.OTWPH";
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000539 RID: 1337 RVA: 0x00003C4E File Offset: 0x00001E4E
		public override string Id
		{
			get
			{
				return "Headers Protection";
			}
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x00003C4E File Offset: 0x00001E4E
		public override string Name
		{
			get
			{
				return "Headers Protection";
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x0400027D RID: 637
		public const string _FullId = "Ki.OTWPH";

		// Token: 0x0400027E RID: 638
		public const string _Id = "PE Headers Protection";

		// Token: 0x02000130 RID: 304
		private class Overwrite : ProtectionPhase
		{
			// Token: 0x0600053D RID: 1341 RVA: 0x00002E01 File Offset: 0x00001001
			public Overwrite(OverwritesHeadersProtection parent) : base(parent)
			{
			}

			// Token: 0x0600053E RID: 1342 RVA: 0x0001EDEC File Offset: 0x0001CFEC
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				TypeDef runtimeType = context.Registry.GetService<IRuntimeService>().GetRuntimeType("Confuser.Runtime.OverwritesHeaders");
				IMarkerService service = context.Registry.GetService<IMarkerService>();
				INameService service2 = context.Registry.GetService<INameService>();
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, moduleDef.GlobalType, moduleDef);
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					MethodDef method2 = (MethodDef)enumerable.Single((IDnlibDef method) => method.Name == "Initialize");
					methodDef.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Call, method2));
					foreach (IDnlibDef def in enumerable)
					{
						service2.MarkHelper(def, service, (Protection)base.Parent);
					}
				}
			}

			// Token: 0x17000179 RID: 377
			// (get) Token: 0x0600053F RID: 1343 RVA: 0x00003C55 File Offset: 0x00001E55
			public override string Name
			{
				get
				{
					return "Overwriting Headers";
				}
			}

			// Token: 0x1700017A RID: 378
			// (get) Token: 0x06000540 RID: 1344 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}
		}
	}
}
