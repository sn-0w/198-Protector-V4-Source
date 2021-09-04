using System;
using Confuser.Core;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001A RID: 26
	public class StringTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000AF RID: 175 RVA: 0x0000A127 File Offset: 0x00008327
		public StringTypeReference(Instruction reference, TypeDef typeDef)
		{
			this.reference = reference;
			this.typeDef = typeDef;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000A140 File Offset: 0x00008340
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.reference.Operand = this.typeDef.ReflectionFullName;
			return true;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000A16C File Offset: 0x0000836C
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000051 RID: 81
		private readonly Instruction reference;

		// Token: 0x04000052 RID: 82
		private readonly TypeDef typeDef;
	}
}
