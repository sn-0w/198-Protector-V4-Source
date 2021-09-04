using System;
using System.Linq;
using System.Text.RegularExpressions;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections
{
	// Token: 0x02000124 RID: 292
	public class RandomOutLineMethods : Protection
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060004F5 RID: 1269 RVA: 0x0000264B File Offset: 0x0000084B
		public override string Author
		{
			get
			{
				return "Aptitude#2684";
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x000020F4 File Offset: 0x000002F4
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x00003AE9 File Offset: 0x00001CE9
		public override string Name
		{
			get
			{
				return "Random OutLine Methods ";
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00003AF0 File Offset: 0x00001CF0
		public override string Description
		{
			get
			{
				return "Adds random outlines";
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00003AF7 File Offset: 0x00001CF7
		public override string Id
		{
			get
			{
				return "Ki.OutLine";
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x00003AFE File Offset: 0x00001CFE
		public override string FullId
		{
			get
			{
				return "Ki.Outline";
			}
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00002B94 File Offset: 0x00000D94
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00003B05 File Offset: 0x00001D05
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.WriteModule, new RandomOutLineMethods.RandomOutLineMethodsPhase(this));
		}

		// Token: 0x02000125 RID: 293
		private class RandomOutLineMethodsPhase : ProtectionPhase
		{
			// Token: 0x060004FE RID: 1278 RVA: 0x00002E01 File Offset: 0x00001001
			public RandomOutLineMethodsPhase(RandomOutLineMethods parent) : base(parent)
			{
			}

			// Token: 0x17000159 RID: 345
			// (get) Token: 0x060004FF RID: 1279 RVA: 0x00002141 File Offset: 0x00000341
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x1700015A RID: 346
			// (get) Token: 0x06000500 RID: 1280 RVA: 0x00002145 File Offset: 0x00000345
			public override string Name
			{
				get
				{
					return "Anti De4Dot";
				}
			}

			// Token: 0x06000501 RID: 1281 RVA: 0x0001E4A4 File Offset: 0x0001C6A4
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					foreach (TypeDef typeDef in moduleDef.Types)
					{
						foreach (MethodDef source_method in typeDef.Methods.ToArray<MethodDef>())
						{
							MethodDef item = RandomOutLineMethods.RandomOutLineMethodsPhase.CreateReturnMethodDef(RandomOutLineMethods.RandomOutLineMethodsPhase.RandomString(), source_method);
							MethodDef item2 = RandomOutLineMethods.RandomOutLineMethodsPhase.CreateReturnMethodDef(RandomOutLineMethods.RandomOutLineMethodsPhase.RandomInt(), source_method);
							typeDef.Methods.Add(item);
							typeDef.Methods.Add(item2);
						}
					}
				}
			}

			// Token: 0x06000502 RID: 1282 RVA: 0x00003B14 File Offset: 0x00001D14
			public static string RandomString()
			{
				return new string((from s in Enumerable.Repeat<string>("abcdefghijklmnopqrstuv!@#$%^&*()/[]", 10)
				select s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray<char>());
			}

			// Token: 0x06000503 RID: 1283 RVA: 0x0001E588 File Offset: 0x0001C788
			public static int RandomInt()
			{
				return new Random(Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), "\\d+").Value)).Next(0, 99999999);
			}

			// Token: 0x06000504 RID: 1284 RVA: 0x0001E5CC File Offset: 0x0001C7CC
			private static MethodDef CreateReturnMethodDef(object value, MethodDef source_method)
			{
				CorLibTypeSig retType = null;
				if (value is int)
				{
					retType = source_method.Module.CorLibTypes.Int32;
				}
				else if (value is float)
				{
					retType = source_method.Module.CorLibTypes.Single;
				}
				else if (value is string)
				{
					retType = source_method.Module.CorLibTypes.String;
				}
				MethodDef methodDef = new MethodDefUser(RandomOutLineMethods.RandomOutLineMethodsPhase.RandomString(), MethodSig.CreateStatic(retType), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.HideBySig)
				{
					Body = new CilBody()
				};
				if (value is int)
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, (int)value));
				}
				else if (value is float)
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (double)value));
				}
				else if (value is string)
				{
					methodDef.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, (string)value));
				}
				methodDef.Body.Instructions.Add(new Instruction(OpCodes.Ret));
				return methodDef;
			}
		}
	}
}
