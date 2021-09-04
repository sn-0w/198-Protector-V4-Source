using System;
using System.Linq;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections
{
	// Token: 0x02000004 RID: 4
	internal class AntiILDasmProtection : Protection
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00004018 File Offset: 0x00002218
		public override string Name
		{
			get
			{
				return "Anti IL Dasm Protection";
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00004030 File Offset: 0x00002230
		public override string Description
		{
			get
			{
				return "This protection marks the module with a attribute that discourage ILDasm from disassembling it.";
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00004048 File Offset: 0x00002248
		public override string Author
		{
			get
			{
				return "Ki (yck1509)";
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00004060 File Offset: 0x00002260
		public override string Id
		{
			get
			{
				return "anti ildasm";
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00004078 File Offset: 0x00002278
		public override string FullId
		{
			get
			{
				return "Ki.AntiILDasm";
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00004090 File Offset: 0x00002290
		public override ProtectionPreset Preset
		{
			get
			{
				return ProtectionPreset.Minimum;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000211A File Offset: 0x0000031A
		protected override void Initialize(ConfuserContext context)
		{
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000214C File Offset: 0x0000034C
		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			pipeline.InsertPreStage(PipelineStage.ProcessModule, new AntiILDasmProtection.AntiILDasmPhase(this));
		}

		// Token: 0x04000001 RID: 1
		public const string _Id = "anti ildasm";

		// Token: 0x04000002 RID: 2
		public const string _FullId = "Ki.AntiILDasm";

		// Token: 0x02000005 RID: 5
		private class AntiILDasmPhase : ProtectionPhase
		{
			// Token: 0x06000017 RID: 23 RVA: 0x00002136 File Offset: 0x00000336
			public AntiILDasmPhase(AntiILDasmProtection parent) : base(parent)
			{
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000018 RID: 24 RVA: 0x000040A4 File Offset: 0x000022A4
			public override ProtectionTargets Targets
			{
				get
				{
					return ProtectionTargets.Modules;
				}
			}

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x06000019 RID: 25 RVA: 0x000040B8 File Offset: 0x000022B8
			public override string Name
			{
				get
				{
					return "Anti-ILDasm marking";
				}
			}

			// Token: 0x0600001A RID: 26 RVA: 0x000040D0 File Offset: 0x000022D0
			protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
			{
				foreach (ModuleDef moduleDef in parameters.Targets.OfType<ModuleDef>())
				{
					TypeRef typeRef = moduleDef.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressIldasmAttribute");
					MemberRefUser ctor = new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef);
					CustomAttribute item = new CustomAttribute(ctor);
					moduleDef.CustomAttributes.Add(item);
					TypeRef typeRef2 = moduleDef.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "SuppressUnmanagedCodeSecurity");
					MemberRefUser ctor2 = new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef2);
					CustomAttribute item2 = new CustomAttribute(ctor2);
					moduleDef.CustomAttributes.Add(item2);
					TypeRef typeRef3 = moduleDef.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "UnsafeValueTypeAttribute");
					MemberRefUser ctor3 = new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef3);
					CustomAttribute item3 = new CustomAttribute(ctor3);
					moduleDef.CustomAttributes.Add(item3);
					TypeRef typeRef4 = moduleDef.CorLibTypes.GetTypeRef("System.Runtime.CompilerServices", "RuntimeWrappedException");
					MemberRefUser ctor4 = new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef4);
					CustomAttribute item4 = new CustomAttribute(ctor4);
					moduleDef.CustomAttributes.Add(item4);
					TypeRef typeRef5 = moduleDef.CorLibTypes.GetTypeRef("System.Security", "UnverifiableCodeAttribute");
					MemberRefUser ctor5 = new MemberRefUser(moduleDef, ".ctor", MethodSig.CreateInstance(moduleDef.CorLibTypes.Void), typeRef5);
					CustomAttribute item5 = new CustomAttribute(ctor5);
					moduleDef.CustomAttributes.Add(item5);
				}
			}
		}
	}
}
