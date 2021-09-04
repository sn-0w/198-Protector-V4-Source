using System;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x02000010 RID: 16
	public class VTableSlot
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00008B80 File Offset: 0x00006D80
		internal VTableSlot(MethodDef def, TypeSig decl, VTableSignature signature) : this(def.DeclaringType.ToTypeSig(true), def, decl, signature, null)
		{
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00008B9A File Offset: 0x00006D9A
		internal VTableSlot(TypeSig defDeclType, MethodDef def, TypeSig decl, VTableSignature signature, VTableSlot overrides)
		{
			this.MethodDefDeclType = defDeclType;
			this.MethodDef = def;
			this.DeclaringType = decl;
			this.Signature = signature;
			this.Overrides = overrides;
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00008BCE File Offset: 0x00006DCE
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00008BD6 File Offset: 0x00006DD6
		public TypeSig DeclaringType { get; internal set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00008BDF File Offset: 0x00006DDF
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00008BE7 File Offset: 0x00006DE7
		public VTableSignature Signature { get; internal set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00008BF0 File Offset: 0x00006DF0
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00008BF8 File Offset: 0x00006DF8
		public TypeSig MethodDefDeclType { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00008C01 File Offset: 0x00006E01
		// (set) Token: 0x0600007B RID: 123 RVA: 0x00008C09 File Offset: 0x00006E09
		public MethodDef MethodDef { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00008C12 File Offset: 0x00006E12
		// (set) Token: 0x0600007D RID: 125 RVA: 0x00008C1A File Offset: 0x00006E1A
		public VTableSlot Overrides { get; private set; }

		// Token: 0x0600007E RID: 126 RVA: 0x00008C24 File Offset: 0x00006E24
		public VTableSlot OverridedBy(MethodDef method)
		{
			return new VTableSlot(method.DeclaringType.ToTypeSig(true), method, this.DeclaringType, this.Signature, this);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00008C58 File Offset: 0x00006E58
		internal VTableSlot Clone()
		{
			return new VTableSlot(this.MethodDefDeclType, this.MethodDef, this.DeclaringType, this.Signature, this.Overrides);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00008C90 File Offset: 0x00006E90
		public override string ToString()
		{
			return this.MethodDef.ToString();
		}
	}
}
