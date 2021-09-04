using System;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Protections.TypeScramble.Scrambler;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble
{
	// Token: 0x02000034 RID: 52
	internal sealed class AnalyzePhase : ProtectionPhase
	{
		// Token: 0x06000122 RID: 290 RVA: 0x00002136 File Offset: 0x00000336
		public AnalyzePhase(TypeScrambleProtection parent) : base(parent)
		{
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000123 RID: 291 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types | ProtectionTargets.Methods;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00002718 File Offset: 0x00000918
		public override string Name
		{
			get
			{
				return "Type scanner";
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00006BB8 File Offset: 0x00004DB8
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			bool flag = context == null;
			if (flag)
			{
				throw new ArgumentNullException("context");
			}
			bool flag2 = parameters == null;
			if (flag2)
			{
				throw new ArgumentNullException("parameters");
			}
			TypeService service = context.Registry.GetService<TypeService>();
			Debug.Assert(service != null, "typeService != null");
			foreach (IDnlibDef dnlibDef in parameters.Targets.WithProgress(context.Logger))
			{
				IDnlibDef dnlibDef2 = dnlibDef;
				IDnlibDef dnlibDef3 = dnlibDef2;
				TypeDef typeDef = dnlibDef3 as TypeDef;
				if (typeDef == null)
				{
					MethodDef methodDef = dnlibDef3 as MethodDef;
					if (methodDef != null)
					{
						bool parameter = parameters.GetParameter<bool>(context, methodDef, "scramblePublic", false);
						service.AddScannedItem(new ScannedMethod(service, methodDef, parameter));
					}
				}
				else
				{
					service.AddScannedItem(new ScannedType(typeDef));
				}
				context.CheckCancellation();
			}
		}
	}
}
