using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000019 RID: 25
	internal class BAMLConverterMemberReference : INameReference<IDnlibDef>, INameReference
	{
		// Token: 0x060000AC RID: 172 RVA: 0x0000A057 File Offset: 0x00008257
		public BAMLConverterMemberReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, IDnlibDef member, PropertyRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.member = member;
			this.rec = rec;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000A080 File Offset: 0x00008280
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string text = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				text = prefix + ":" + text;
			}
			this.rec.Value = text + "." + this.member.Name;
			return true;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000A114 File Offset: 0x00008314
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400004D RID: 77
		private readonly IDnlibDef member;

		// Token: 0x0400004E RID: 78
		private readonly PropertyRecord rec;

		// Token: 0x0400004F RID: 79
		private readonly TypeSig sig;

		// Token: 0x04000050 RID: 80
		private readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
