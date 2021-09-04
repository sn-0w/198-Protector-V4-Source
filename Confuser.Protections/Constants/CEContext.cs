using System;
using System.Collections.Generic;
using Confuser.Core;
using Confuser.Core.Services;
using Confuser.DynCipher;
using Confuser.Renamer;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.Constants
{
	// Token: 0x0200009E RID: 158
	internal class CEContext
	{
		// Token: 0x04000122 RID: 290
		public ConfuserContext Context;

		// Token: 0x04000123 RID: 291
		public ConstantProtection Protection;

		// Token: 0x04000124 RID: 292
		public ModuleDef Module;

		// Token: 0x04000125 RID: 293
		public FieldDef BufferField;

		// Token: 0x04000126 RID: 294
		public FieldDef DataField;

		// Token: 0x04000127 RID: 295
		public TypeDef DataType;

		// Token: 0x04000128 RID: 296
		public MethodDef XorShit;

		// Token: 0x04000129 RID: 297
		public MethodDef InitMethod;

		// Token: 0x0400012A RID: 298
		public int DecoderCount;

		// Token: 0x0400012B RID: 299
		public List<Tuple<MethodDef, DecoderDesc>> Decoders;

		// Token: 0x0400012C RID: 300
		public EncodeElements Elements;

		// Token: 0x0400012D RID: 301
		public List<uint> EncodedBuffer;

		// Token: 0x0400012E RID: 302
		public Mode Mode;

		// Token: 0x0400012F RID: 303
		public IEncodeMode ModeHandler;

		// Token: 0x04000130 RID: 304
		public IDynCipherService DynCipher;

		// Token: 0x04000131 RID: 305
		public IMarkerService Marker;

		// Token: 0x04000132 RID: 306
		public INameService Name;

		// Token: 0x04000133 RID: 307
		public RandomGenerator Random;

		// Token: 0x04000134 RID: 308
		public TypeDef CfgCtxType;

		// Token: 0x04000135 RID: 309
		public MethodDef CfgCtxCtor;

		// Token: 0x04000136 RID: 310
		public MethodDef CfgCtxNext;

		// Token: 0x04000137 RID: 311
		public Dictionary<MethodDef, List<Tuple<Instruction, uint, IMethod>>> ReferenceRepl;
	}
}
