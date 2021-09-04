using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000017 RID: 23
	internal class BAMLAttributeReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000A4 RID: 164 RVA: 0x00009ECB File Offset: 0x000080CB
		public BAMLAttributeReference(IDnlibDef member, AttributeInfoRecord rec)
		{
			this.member = member;
			this.attrRec = rec;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00009EE3 File Offset: 0x000080E3
		public BAMLAttributeReference(IDnlibDef member, PropertyRecord rec)
		{
			this.member = member;
			this.propRec = rec;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00009EFC File Offset: 0x000080FC
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			bool flag = this.attrRec != null;
			if (flag)
			{
				this.attrRec.Name = this.member.Name;
			}
			else
			{
				this.propRec.Value = this.member.Name;
			}
			return true;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00009F58 File Offset: 0x00008158
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000046 RID: 70
		private readonly AttributeInfoRecord attrRec;

		// Token: 0x04000047 RID: 71
		private readonly IDnlibDef member;

		// Token: 0x04000048 RID: 72
		private readonly PropertyRecord propRec;
	}
}
