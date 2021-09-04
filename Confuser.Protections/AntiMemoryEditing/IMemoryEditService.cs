using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x0200010D RID: 269
	internal interface IMemoryEditService
	{
		// Token: 0x06000485 RID: 1157
		void AddToList(FieldDef d);

		// Token: 0x06000486 RID: 1158
		IEnumerable<FieldDef> GetFields();

		// Token: 0x06000487 RID: 1159
		TypeDef GetWrapperType(ModuleDef m);

		// Token: 0x06000488 RID: 1160
		void SetWrapperType(ModuleDef m, TypeDef t);

		// Token: 0x06000489 RID: 1161
		IMethod GetReadMethod(ModuleDef mod);

		// Token: 0x0600048A RID: 1162
		IMethod GetWriteMethod(ModuleDef mod);

		// Token: 0x0600048B RID: 1163
		void SetReadMethod(ModuleDef mod, IMethod m);

		// Token: 0x0600048C RID: 1164
		void SetWriteMethod(ModuleDef mod, IMethod m);
	}
}
