using System;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections.Resources
{
	// Token: 0x0200005A RID: 90
	internal class REContext
	{
		// Token: 0x0400006D RID: 109
		public ConfuserContext Context;

		// Token: 0x0400006E RID: 110
		public FieldDef DataField;

		// Token: 0x0400006F RID: 111
		public TypeDef DataType;

		// Token: 0x04000070 RID: 112
		public IDynCipherService DynCipher;

		// Token: 0x04000071 RID: 113
		public MethodDef InitMethod;

		// Token: 0x04000072 RID: 114
		public IMarkerService Marker;

		// Token: 0x04000073 RID: 115
		public Mode Mode;

		// Token: 0x04000074 RID: 116
		public IEncodeMode ModeHandler;

		// Token: 0x04000075 RID: 117
		public ModuleDef Module;

		// Token: 0x04000076 RID: 118
		public INameService Name;

		// Token: 0x04000077 RID: 119
		public RandomGenerator Random;
	}
}
