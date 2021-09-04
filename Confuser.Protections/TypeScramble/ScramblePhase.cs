using System;
using System.Diagnostics;
using Confuser.Core;
using Confuser.Protections.TypeScramble.Scrambler;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble
{
	// Token: 0x02000035 RID: 53
	internal sealed class ScramblePhase : ProtectionPhase
	{
		// Token: 0x06000126 RID: 294 RVA: 0x00002136 File Offset: 0x00000336
		public ScramblePhase(TypeScrambleProtection parent) : base(parent)
		{
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000022E7 File Offset: 0x000004E7
		public override ProtectionTargets Targets
		{
			get
			{
				return ProtectionTargets.Types | ProtectionTargets.Methods;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000271F File Offset: 0x0000091F
		public override string Name
		{
			get
			{
				return "Type scrambler";
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00006CB4 File Offset: 0x00004EB4
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
			Debug.Assert(service != null, "service != null");
			bool flag3 = !service.ScrambledAnything;
			if (!flag3)
			{
				TypeRewriter typeRewriter = new TypeRewriter(context);
				typeRewriter.ApplyGenerics();
				foreach (IDnlibDef dnlibDef in context.CurrentModule.FindDefinitions().WithProgress(context.Logger))
				{
					IDnlibDef dnlibDef2 = dnlibDef;
					IDnlibDef dnlibDef3 = dnlibDef2;
					MethodDef methodDef = dnlibDef3 as MethodDef;
					if (methodDef != null)
					{
						bool hasBody = methodDef.HasBody;
						if (hasBody)
						{
							typeRewriter.Process(methodDef);
						}
					}
					context.CheckCancellation();
				}
			}
		}
	}
}
