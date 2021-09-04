using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001B RID: 27
	public class TypeRefReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x0000A17F File Offset: 0x0000837F
		public TypeRefReference(TypeRef typeRef, TypeDef typeDef)
		{
			this.typeRef = typeRef;
			this.typeDef = typeDef;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000A198 File Offset: 0x00008398
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.typeRef.Namespace = this.typeDef.Namespace;
			this.typeRef.Name = this.typeDef.Name;
			return true;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000A1DC File Offset: 0x000083DC
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000053 RID: 83
		private readonly TypeDef typeDef;

		// Token: 0x04000054 RID: 84
		private readonly TypeRef typeRef;
	}
}
