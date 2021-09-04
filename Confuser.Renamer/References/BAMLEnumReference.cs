using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000013 RID: 19
	internal class BAMLEnumReference : INameReference<FieldDef>, INameReference
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00009C98 File Offset: 0x00007E98
		public BAMLEnumReference(FieldDef enumField, PropertyRecord rec)
		{
			this.enumField = enumField;
			this.rec = rec;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00009CB0 File Offset: 0x00007EB0
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			this.rec.Value = this.enumField.Name;
			return true;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00009CE0 File Offset: 0x00007EE0
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400003C RID: 60
		private readonly FieldDef enumField;

		// Token: 0x0400003D RID: 61
		private readonly PropertyRecord rec;
	}
}
