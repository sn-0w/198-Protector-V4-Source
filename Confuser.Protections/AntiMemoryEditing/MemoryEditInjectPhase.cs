using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Renamer;
using Confuser.Runtime.AntiMemoryEditing;
using dnlib.DotNet;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x02000109 RID: 265
	internal class MemoryEditInjectPhase : ProtectionPhase
	{
		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000471 RID: 1137 RVA: 0x000025B0 File Offset: 0x000007B0
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Fields;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06000472 RID: 1138 RVA: 0x000037F5 File Offset: 0x000019F5
		public override string Name
		{
			get
			{
				return "Memory obfuscation type injection";
			}
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00002136 File Offset: 0x00000336
		public MemoryEditInjectPhase(ConfuserComponent parent) : base(parent)
		{
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001D788 File Offset: 0x0001B988
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = !parameters.Targets.Any(delegate(IDnlibDef a)
			{
				FieldDef fieldDef = a as FieldDef;
				return fieldDef != null && fieldDef.Module == context.CurrentModule;
			});
			if (!flag)
			{
				ModuleDefMD currentModule = context.CurrentModule;
				IMemoryEditService service = context.Registry.GetService<IMemoryEditService>();
				IMarkerService service2 = context.Registry.GetService<IMarkerService>();
				INameService service3 = context.Registry.GetService<INameService>();
				TypeDef runtimeType = MemoryEditInjectPhase.GetRuntimeType(typeof(ObfuscatedValue<>));
				TypeDefUser typeDefUser = new TypeDefUser(runtimeType.Namespace, runtimeType.Name, new Importer(currentModule).Import(typeof(object)));
				typeDefUser.GenericParameters.Add(new GenericParamUser(0, GenericParamAttributes.NonVariant, "T"));
				currentModule.Types.Add(typeDefUser);
				IEnumerable<IDnlibDef> enumerable = InjectHelper.Inject(runtimeType, typeDefUser, currentModule);
				service.SetWrapperType(currentModule, typeDefUser);
				MethodDef[] array = typeDefUser.FindMethods("op_Implicit").ToArray<MethodDef>();
				service.SetReadMethod(currentModule, array[0]);
				service.SetWriteMethod(currentModule, array[1]);
				service3.MarkHelper(typeDefUser, service2, base.Parent);
				foreach (IDnlibDef member in enumerable)
				{
					service2.Mark(member, base.Parent);
				}
			}
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000037FC File Offset: 0x000019FC
		public static TypeDef GetRuntimeType(Type t)
		{
			return MemoryEditInjectPhase.RuntimeModule.Value.Find(t.FullName, true);
		}

		// Token: 0x04000252 RID: 594
		private static readonly Lazy<ModuleDefMD> RuntimeModule = new Lazy<ModuleDefMD>(() => ModuleDefMD.Load(typeof(ObfuscatedValue<>).Module));
	}
}
