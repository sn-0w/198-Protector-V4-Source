using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.ReferenceProxy
{
	// Token: 0x02000075 RID: 117
	internal class RPContext
	{
		// Token: 0x06000227 RID: 551 RVA: 0x0000CB18 File Offset: 0x0000AD18
		internal void MarkMember(IDnlibDef def)
		{
			bool flag = this.Name == null;
			if (flag)
			{
				this.Marker.Mark(def, this.Protection);
			}
			else
			{
				this.Name.MarkHelper(def, this.Marker, this.Protection);
			}
		}

		// Token: 0x040000B7 RID: 183
		public ReferenceProxyProtection Protection;

		// Token: 0x040000B8 RID: 184
		public CilBody Body;

		// Token: 0x040000B9 RID: 185
		public HashSet<Instruction> BranchTargets;

		// Token: 0x040000BA RID: 186
		public ConfuserContext Context;

		// Token: 0x040000BB RID: 187
		public Dictionary<MethodSig, TypeDef> Delegates;

		// Token: 0x040000BC RID: 188
		public int Depth;

		// Token: 0x040000BD RID: 189
		public IDynCipherService DynCipher;

		// Token: 0x040000BE RID: 190
		public EncodingType Encoding;

		// Token: 0x040000BF RID: 191
		public IRPEncoding EncodingHandler;

		// Token: 0x040000C0 RID: 192
		public int InitCount;

		// Token: 0x040000C1 RID: 193
		public bool InternalAlso;

		// Token: 0x040000C2 RID: 194
		public IMarkerService Marker;

		// Token: 0x040000C3 RID: 195
		public MethodDef Method;

		// Token: 0x040000C4 RID: 196
		public Mode Mode;

		// Token: 0x040000C5 RID: 197
		public RPMode ModeHandler;

		// Token: 0x040000C6 RID: 198
		public ModuleDef Module;

		// Token: 0x040000C7 RID: 199
		public INameService Name;

		// Token: 0x040000C8 RID: 200
		public RandomGenerator Random;

		// Token: 0x040000C9 RID: 201
		public bool TypeErasure;
	}
}
