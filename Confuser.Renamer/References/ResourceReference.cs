using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x0200001E RID: 30
	internal class ResourceReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000BB RID: 187 RVA: 0x0000A28F File Offset: 0x0000848F
		public ResourceReference(Resource resource, TypeDef typeDef, string format)
		{
			this.resource = resource;
			this.typeDef = typeDef;
			this.format = format;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000A2B0 File Offset: 0x000084B0
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.resource.Name = string.Format(this.format, this.typeDef.ReflectionFullName);
			return true;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000A2EC File Offset: 0x000084EC
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000059 RID: 89
		private readonly string format;

		// Token: 0x0400005A RID: 90
		private readonly Resource resource;

		// Token: 0x0400005B RID: 91
		private readonly TypeDef typeDef;
	}
}
