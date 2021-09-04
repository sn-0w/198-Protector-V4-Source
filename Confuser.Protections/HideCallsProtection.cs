using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x0200012D RID: 301
	internal class HideCallsProtection : Protection
	{
		// Token: 0x1700016B RID: 363
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x00003BFF File Offset: 0x00001DFF
		public override string Name
		{
			get
			{
				return "Hide Calls Protection";
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x00003C06 File Offset: 0x00001E06
		public override string Description
		{
			get
			{
				return "This protection crash .cctor.";
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x00003C0D File Offset: 0x00001E0D
		public override string Id
		{
			get
			{
				return "Hide Calls";
			}
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x0600052B RID: 1323 RVA: 0x00003C14 File Offset: 0x00001E14
		public override string FullId
		{
			get
			{
				return "Ki.Hcs";
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00003C1B File Offset: 0x00001E1B
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new HideCallsProtection.HideCallsPhase(this));
		}

		// Token: 0x0400027B RID: 635
		public const string _Id = "Hide Calls";

		// Token: 0x0400027C RID: 636
		public const string _FullId = "Ki.Hcs";

		// Token: 0x0200012E RID: 302
		private class HideCallsPhase : ProtectionPhase
		{
			// Token: 0x06000530 RID: 1328 RVA: 0x00002E01 File Offset: 0x00001001
			public HideCallsPhase(HideCallsProtection parent) : base(parent)
			{
			}

			// Token: 0x17000171 RID: 369
			// (get) Token: 0x06000531 RID: 1329 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000172 RID: 370
			// (get) Token: 0x06000532 RID: 1330 RVA: 0x00003C2A File Offset: 0x00001E2A
			public override string Name
			{
				get
				{
					return "Hide Calls Injection";
				}
			}

			// Token: 0x06000533 RID: 1331 RVA: 0x0001EA28 File Offset: 0x0001CC28
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					MethodDef methodDef = moduleDef.GlobalType.FindStaticConstructor();
					Local local = new Local(moduleDef.Import(typeof(int)).ToTypeSig(true));
					Local local2 = new Local(moduleDef.Import(typeof(bool)).ToTypeSig(true));
					methodDef.Body.Variables.Add(local);
					methodDef.Body.Variables.Add(local2);
					Instruction operand = null;
					Instruction instruction = new Instruction(OpCodes.Ret);
					Instruction instruction2 = new Instruction(OpCodes.Ldc_I4_1);
					methodDef.Body.Instructions.Insert(0, new Instruction(OpCodes.Ldc_I4_0));
					methodDef.Body.Instructions.Insert(1, new Instruction(OpCodes.Stloc, local));
					methodDef.Body.Instructions.Insert(2, new Instruction(OpCodes.Br, instruction2));
					Instruction instruction3 = new Instruction(OpCodes.Ldloc, local);
					methodDef.Body.Instructions.Insert(3, instruction3);
					methodDef.Body.Instructions.Insert(4, new Instruction(OpCodes.Ldc_I4_0));
					methodDef.Body.Instructions.Insert(5, new Instruction(OpCodes.Ceq));
					methodDef.Body.Instructions.Insert(6, new Instruction(OpCodes.Ldc_I4_1));
					methodDef.Body.Instructions.Insert(7, new Instruction(OpCodes.Ceq));
					methodDef.Body.Instructions.Insert(8, new Instruction(OpCodes.Stloc, local2));
					methodDef.Body.Instructions.Insert(9, new Instruction(OpCodes.Ldloc, local2));
					methodDef.Body.Instructions.Insert(10, new Instruction(OpCodes.Brtrue, methodDef.Body.Instructions[10]));
					methodDef.Body.Instructions.Insert(11, new Instruction(OpCodes.Ret));
					methodDef.Body.Instructions.Insert(12, new Instruction(OpCodes.Calli));
					methodDef.Body.Instructions.Insert(13, new Instruction(OpCodes.Sizeof, operand));
					methodDef.Body.Instructions.Insert(methodDef.Body.Instructions.Count, instruction2);
					methodDef.Body.Instructions.Insert(methodDef.Body.Instructions.Count, new Instruction(OpCodes.Stloc, local2));
					methodDef.Body.Instructions.Insert(methodDef.Body.Instructions.Count, new Instruction(OpCodes.Br, instruction3));
					methodDef.Body.Instructions.Insert(methodDef.Body.Instructions.Count, instruction);
					ExceptionHandler item = new ExceptionHandler(ExceptionHandlerType.Finally)
					{
						HandlerStart = methodDef.Body.Instructions[10],
						HandlerEnd = methodDef.Body.Instructions[11],
						TryEnd = methodDef.Body.Instructions[14],
						TryStart = methodDef.Body.Instructions[12]
					};
					if (!methodDef.Body.HasExceptionHandlers)
					{
						methodDef.Body.ExceptionHandlers.Add(item);
					}
					operand = new Instruction(OpCodes.Br, instruction);
					methodDef.Body.OptimizeBranches();
					methodDef.Body.OptimizeMacros();
				}
			}
		}
	}
}
