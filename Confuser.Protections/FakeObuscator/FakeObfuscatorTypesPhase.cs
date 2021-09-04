using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using Confuser.Runtime.FakeObfuscator;
using dnlib.DotNet;

namespace Confuser.Protections.FakeObuscator
{
	// Token: 0x02000080 RID: 128
	public class FakeObfuscatorTypesPhase : ProtectionPhase
	{
		// Token: 0x0600024D RID: 589 RVA: 0x00002136 File Offset: 0x00000336
		public FakeObfuscatorTypesPhase(FakeObfuscatorProtection parent) : base(parent)
		{
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600024E RID: 590 RVA: 0x00002141 File Offset: 0x00000341
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Modules;
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600024F RID: 591 RVA: 0x00002D9C File Offset: 0x00000F9C
		public override string Name
		{
			get
			{
				return "Fake obfuscator type addition";
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000E2D8 File Offset: 0x0000C4D8
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			IMarkerService service = context.Registry.GetService<IMarkerService>();
			INameService service2 = context.Registry.GetService<INameService>();
			List<IDnlibDef> list = new List<IDnlibDef>();
			Type[][] enumerable = new Type[][]
			{
				BabelDotNet.GetTypes(),
				CodeFort.GetTypes(),
				CodeWall.GetTypes(),
				CryptoObfuscator.GetTypes(),
				Dotfuscator.GetTypes(),
				EazfuscatorDotNet.GetTypes(),
				GoliathDotNet.GetTypes(),
				Xenocode.GetTypes()
			};
			foreach (ModuleDef m in parameters.Targets.Cast<ModuleDef>().WithProgress(context.Logger))
			{
				foreach (Type[] types in enumerable.WithProgress(context.Logger))
				{
					list.AddRange(FakeObfuscatorTypesPhase.InjectType(m, context.Logger, types));
				}
				foreach (IDnlibDef dnlibDef in list)
				{
					service.Mark(dnlibDef, base.Parent);
					service2.SetCanRename(dnlibDef, false);
				}
			}
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000E454 File Offset: 0x0000C654
		private static IEnumerable<IDnlibDef> InjectType(ModuleDef m, Confuser.Core.ILogger l, params Type[] types)
		{
			List<IDnlibDef> list = new List<IDnlibDef>();
			foreach (TypeDef typeDef in types.Select(new Func<Type, TypeDef>(FakeObfuscatorTypesPhase.GetRuntimeType)))
			{
				TypeDefUser typeDefUser = new TypeDefUser(m.GlobalType.Namespace, typeDef.Name);
				m.Types.Add(typeDefUser);
				l.Debug("Added type " + typeDefUser);
				list.Add(typeDefUser);
				list.AddRange(InjectHelper.Inject(typeDef, typeDefUser, m));
			}
			return list;
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00002DA3 File Offset: 0x00000FA3
		public static TypeDef GetRuntimeType(Type t)
		{
			return FakeObfuscatorTypesPhase.RuntimeModule.Value.Find(t.FullName, true);
		}

		// Token: 0x040000D1 RID: 209
		private static readonly Lazy<ModuleDefMD> RuntimeModule = new Lazy<ModuleDefMD>(() => ModuleDefMD.Load(typeof(BabelDotNet).Module));
	}
}
