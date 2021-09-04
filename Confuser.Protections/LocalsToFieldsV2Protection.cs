using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000127 RID: 295
	[BeforeProtection(new string[]
	{
		"Ki.ControlFlow"
	})]
	internal class LocalsToFieldsV2Protection : Protection
	{
		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x00003B5C File Offset: 0x00001D5C
		public override string Name
		{
			get
			{
				return "Locals-to-Field v2 Protection";
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x00003B63 File Offset: 0x00001D63
		public override string Description
		{
			get
			{
				return "This protection converts all locals to fields with randomly selected names.";
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x00003B6A File Offset: 0x00001D6A
		public override string Id
		{
			get
			{
				return "lcltofieldv2";
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x00003B71 File Offset: 0x00001D71
		public override string FullId
		{
			get
			{
				return "Ki.LocalsToFieldv2";
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Maximum;
			}
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x00003B78 File Offset: 0x00001D78
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new LocalsToFieldsV2Protection.ConvertionV2Phase(this));
		}

		// Token: 0x04000273 RID: 627
		public const string _Id = "lcltofieldv2";

		// Token: 0x04000274 RID: 628
		public const string _FullId = "Ki.LocalsToFieldv2";

		// Token: 0x02000128 RID: 296
		private class ConvertionV2Phase : ProtectionPhase
		{
			// Token: 0x06000511 RID: 1297 RVA: 0x00003B87 File Offset: 0x00001D87
			public ConvertionV2Phase(LocalsToFieldsV2Protection parent) : base(parent)
			{
			}

			// Token: 0x17000161 RID: 353
			// (get) Token: 0x06000512 RID: 1298 RVA: 0x000021DB File Offset: 0x000003DB
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Methods;
				}
			}

			// Token: 0x17000162 RID: 354
			// (get) Token: 0x06000513 RID: 1299 RVA: 0x00003B9B File Offset: 0x00001D9B
			public override string Name
			{
				get
				{
					return "l-to-f v2 convertion";
				}
			}

			// Token: 0x06000514 RID: 1300 RVA: 0x0001E720 File Offset: 0x0001C920
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				using (IEnumerator<MethodDef> enumerator = parameters.Targets.OfType<MethodDef>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MethodDef method = enumerator.Current;
						if (method.HasBody)
						{
							method.Body.SimplifyMacros(method.Parameters);
							IList<Instruction> instructions = method.Body.Instructions;
							for (int i = 0; i < instructions.Count; i++)
							{
								Local local;
								if ((local = (instructions[i].Operand as Local)) != null)
								{
									FieldDef fieldDef;
									if (!this.convertedLocals.ContainsKey(local))
									{
										fieldDef = new FieldDefUser(Guid.NewGuid().ToString(), new FieldSig(local.Type), FieldAttributes.FamANDAssem | FieldAttributes.Family | FieldAttributes.Static);
										context.CurrentModule.GlobalType.Fields.Add(fieldDef);
										this.convertedLocals.Add(local, fieldDef);
									}
									else
									{
										fieldDef = this.convertedLocals[local];
									}
									OpCode opCode = null;
									switch (instructions[i].OpCode.Code)
									{
									case Code.Ldloc:
										opCode = OpCodes.Ldsfld;
										break;
									case Code.Ldloca:
										opCode = OpCodes.Ldsflda;
										break;
									case Code.Stloc:
										opCode = OpCodes.Stsfld;
										break;
									}
									instructions[i].OpCode = opCode;
									instructions[i].Operand = fieldDef;
								}
							}
							this.convertedLocals.ToList<KeyValuePair<Local, FieldDef>>().ForEach(delegate(KeyValuePair<Local, FieldDef> x)
							{
								method.Body.Variables.Remove(x.Key);
							});
							this.convertedLocals = new Dictionary<Local, FieldDef>();
						}
					}
				}
			}

			// Token: 0x04000275 RID: 629
			private Dictionary<Local, FieldDef> convertedLocals = new Dictionary<Local, FieldDef>();
		}
	}
}
