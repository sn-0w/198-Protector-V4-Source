using System;
using Confuser.Core;
using Confuser.Renamer.BAML;
using dnlib.DotNet;

namespace Confuser.Renamer.References
{
	// Token: 0x02000014 RID: 20
	internal class BAMLPathTypeReference : INameReference<TypeDef>, INameReference
	{
		// Token: 0x06000099 RID: 153 RVA: 0x00009CF3 File Offset: 0x00007EF3
		private BAMLPathTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig)
		{
			this.xmlnsCtx = xmlnsCtx;
			this.sig = sig;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00009D0B File Offset: 0x00007F0B
		public BAMLPathTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, IndexerParamInfo indexerInfo) : this(xmlnsCtx, sig)
		{
			this.indexerInfo = new IndexerParamInfo?(indexerInfo);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00009D22 File Offset: 0x00007F22
		public BAMLPathTypeReference(BAMLAnalyzer.XmlNsContext xmlnsCtx, TypeSig sig, SourceValueInfo propertyInfo) : this(xmlnsCtx, sig)
		{
			this.propertyInfo = new SourceValueInfo?(propertyInfo);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00009D3C File Offset: 0x00007F3C
		public bool UpdateNameReference(ConfuserContext context, INameService service)
		{
			string text = this.sig.ReflectionName;
			string prefix = this.xmlnsCtx.GetPrefix(this.sig.ReflectionNamespace, this.sig.ToBasicTypeDefOrRef().ResolveTypeDefThrow().Module.Assembly);
			bool flag = !string.IsNullOrEmpty(prefix);
			if (flag)
			{
				text = prefix + ":" + text;
			}
			bool flag2 = this.indexerInfo != null;
			if (flag2)
			{
				IndexerParamInfo value = this.indexerInfo.Value;
				value.parenString = text;
				this.indexerInfo = new IndexerParamInfo?(value);
			}
			else
			{
				SourceValueInfo value2 = this.propertyInfo.Value;
				string propertyName = value2.GetPropertyName();
				value2.name = string.Format("({0}.{1})", text, propertyName);
				this.propertyInfo = new SourceValueInfo?(value2);
			}
			return true;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00009E16 File Offset: 0x00008016
		public bool ShouldCancelRename()
		{
			return false;
		}

		// Token: 0x0400003E RID: 62
		private SourceValueInfo? propertyInfo;

		// Token: 0x0400003F RID: 63
		private IndexerParamInfo? indexerInfo;

		// Token: 0x04000040 RID: 64
		private TypeSig sig;

		// Token: 0x04000041 RID: 65
		private BAMLAnalyzer.XmlNsContext xmlnsCtx;
	}
}
