using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000018 RID: 24
	internal class BAMLConverterTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x00009F6B File Offset: 0x0000816B
		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, PropertyRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.propRec = rec;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00009F8A File Offset: 0x0000818A
		public BAMLConverterTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, TextRecord rec)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
			this.textRec = rec;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00009FAC File Offset: 0x000081AC
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string text = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				text = prefix + ":" + text;
			}
			bool flag2 = this.propRec != null;
			if (flag2)
			{
				this.propRec.Value = text;
			}
			else
			{
				this.textRec.Value = text;
			}
			return true;
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000A044 File Offset: 0x00008244
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x04000049 RID: 73
		private readonly PropertyRecord propRec;

		// Token: 0x0400004A RID: 74
		private readonly TypeSig sig;

		// Token: 0x0400004B RID: 75
		private readonly TextRecord textRec;

		// Token: 0x0400004C RID: 76
		private readonly BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
