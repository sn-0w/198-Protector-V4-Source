using System;
using System.Collections.Generic;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ControlFlow
{
	// Token: 0x02000096 RID: 150
	internal abstract class ManglerBase
	{
		// Token: 0x0600029F RID: 671 RVA: 0x00002FE7 File Offset: 0x000011E7
		protected static IEnumerable<InstrBlock> GetAllBlocks(ScopeBlock scope)
		{
			foreach (BlockBase blockBase in scope.Children)
			{
				if (!(blockBase is InstrBlock))
				{
					foreach (InstrBlock instrBlock in ManglerBase.GetAllBlocks((ScopeBlock)blockBase))
					{
						yield return instrBlock;
					}
					IEnumerator<InstrBlock> enumerator2 = null;
				}
				else
				{
					yield return (InstrBlock)blockBase;
				}
			}
			List<BlockBase>.Enumerator enumerator = default(List<BlockBase>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x060002A0 RID: 672
		public abstract void Mangle(CilBody body, ScopeBlock root, CFContext ctx);
	}
}
