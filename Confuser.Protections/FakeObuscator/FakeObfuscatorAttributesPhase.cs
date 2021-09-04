using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.FakeObuscator
{
	// Token: 0x0200007F RID: 127
	public class FakeObfuscatorAttributesPhase : ProtectionPhase
	{
		// Token: 0x06000248 RID: 584 RVA: 0x00002136 File Offset: 0x00000336
		public FakeObfuscatorAttributesPhase(FakeObfuscatorProtection parent) : base(parent)
		{
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00002141 File Offset: 0x00000341
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Modules;
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00002D95 File Offset: 0x00000F95
		public override string Name
		{
			get
			{
				return "Fake obfuscator attribute addition";
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000DD98 File Offset: 0x0000BF98
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			IMarkerService service = context.Registry.GetService<IMarkerService>();
			INameService service2 = context.Registry.GetService<INameService>();
			List<IDnlibDef> list = new List<IDnlibDef>();
			ModuleDefMD currentModule = context.CurrentModule;
			TypeDefUser[] enumerable = new TypeDefUser[]
			{
				new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByCliSecureAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByAgileDotNetAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("BabelAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("CryptoObfuscator", "ProtectedWithCryptoObfuscatorAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("DotfuscatorAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("ObfuscatedByGoliath", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("SmartAssembly.Attributes", "PoweredByAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("NineRays.Obfuscator", "SoftwareWatermarkAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("NineRays.Obfuscator", "Evaluation", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("VMProtect", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("Xenocode.Client.Attributes.AssemblyAttributes", "ProcessedByXenocode", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("ZYXDNGuarder", currentModule.CorLibTypes.GetTypeRef("System", "Attribute")),
				new TypeDefUser("YanoAttribute", currentModule.CorLibTypes.GetTypeRef("System", "Attribute"))
			};
			foreach (ModuleDef m in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger))
			{
				foreach (TypeDefUser typeDefUser in enumerable.WithProgress(context.Logger))
				{
					list.AddRange(this.InjectType(m, context, new TypeDefUser[]
					{
						typeDefUser
					}));
				}
				foreach (IDnlibDef dnlibDef in list)
				{
					service.Mark(dnlibDef, base.Parent);
					service2.SetCanRename(dnlibDef, false);
				}
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000E128 File Offset: 0x0000C328
		private IEnumerable<IDnlibDef> InjectType(ModuleDef m, ConfuserContext context, params TypeDefUser[] types)
		{
			List<IDnlibDef> list = new List<IDnlibDef>();
			IMarkerService service = context.Registry.GetService<IMarkerService>();
			INameService service2 = context.Registry.GetService<INameService>();
			foreach (TypeDefUser typeDefUser in types)
			{
				MethodDefUser methodDefUser = new MethodDefUser(".ctor", MethodSig.CreateInstance(m.CorLibTypes.Void, m.CorLibTypes.String), MethodImplAttributes.IL, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
				methodDefUser.Body = new CilBody();
				methodDefUser.Body.MaxStack = 1;
				methodDefUser.Body.Instructions.Add(OpCodes.Ldarg_0.ToInstruction());
				methodDefUser.Body.Instructions.Add(OpCodes.Call.ToInstruction(new MemberRefUser(m, ".ctor", MethodSig.CreateInstance(m.CorLibTypes.Void), m.CorLibTypes.GetTypeRef("System", "Attribute"))));
				methodDefUser.Body.Instructions.Add(OpCodes.Ret.ToInstruction());
				typeDefUser.Methods.Add(methodDefUser);
				CustomAttribute customAttribute = new CustomAttribute(methodDefUser);
				customAttribute.ConstructorArguments.Add(new CAArgument(m.CorLibTypes.String, service2.RandomName(RenameMode.Letters)));
				m.CustomAttributes.Add(customAttribute);
				m.Types.Add(typeDefUser);
				context.Logger.Debug("Added attribute " + typeDefUser);
				list.Add(methodDefUser);
				list.Add(typeDefUser);
			}
			return list;
		}
	}
}
