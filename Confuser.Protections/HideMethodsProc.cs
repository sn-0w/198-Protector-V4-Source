using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000133 RID: 307
	internal class HideMethodsProc : Protection
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x00002314 File Offset: 0x00000514
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00003CD2 File Offset: 0x00001ED2
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.EndModule, new HideMethodsProc.HideMethodsPhase(this));
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x0600054D RID: 1357 RVA: 0x00003CE1 File Offset: 0x00001EE1
		public override string Description
		{
			get
			{
				return "This protection hides methods.";
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x00003CE8 File Offset: 0x00001EE8
		public override string FullId
		{
			get
			{
				return "Ki.HideMethods";
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x00003CEF File Offset: 0x00001EEF
		public override string Id
		{
			get
			{
				return "Hide Methods";
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x00003CF6 File Offset: 0x00001EF6
		public override string Name
		{
			get
			{
				return "hide methods Protection";
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000551 RID: 1361 RVA: 0x000021DB File Offset: 0x000003DB
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Normal;
			}
		}

		// Token: 0x04000283 RID: 643
		public static bool first;

		// Token: 0x04000284 RID: 644
		public const string _FullId = "Ki.HideMethods";

		// Token: 0x04000285 RID: 645
		public const string _Id = "Hide Methods";

		// Token: 0x02000134 RID: 308
		private class HideMethodsPhase : ProtectionPhase
		{
			// Token: 0x06000553 RID: 1363 RVA: 0x00002E01 File Offset: 0x00001001
			public HideMethodsPhase(HideMethodsProc parent) : base(parent)
			{
			}

			// Token: 0x06000554 RID: 1364 RVA: 0x0001EF1C File Offset: 0x0001D11C
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					foreach (TypeDef typeDef in moduleDef.Types)
					{
						foreach (MethodDef methodDef in typeDef.Methods)
						{
							if (methodDef == moduleDef.EntryPoint)
							{
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

			// Token: 0x17000185 RID: 389
			// (get) Token: 0x06000555 RID: 1365 RVA: 0x00003CFD File Offset: 0x00001EFD
			public override string Name
			{
				get
				{
					return "Hiding Methods";
				}
			}

			// Token: 0x17000186 RID: 390
			// (get) Token: 0x06000556 RID: 1366 RVA: 0x00002141 File Offset: 0x00000341
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
