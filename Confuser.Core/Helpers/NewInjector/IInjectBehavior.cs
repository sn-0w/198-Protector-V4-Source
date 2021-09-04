using System;
using dnlib.DotNet;

namespace Confuser.Core.Helpers.NewInjector
{
	// Token: 0x020000AF RID: 175
	public interface IInjectBehavior
	{
		// Token: 0x06000407 RID: 1031
		void Process(TypeDef source, TypeDefUser injected, Importer importer);

		// Token: 0x06000408 RID: 1032
		void Process(MethodDef source, MethodDefUser injected, Importer importer);

		// Token: 0x06000409 RID: 1033
		void Process(FieldDef source, FieldDefUser injected, Importer importer);

		// Token: 0x0600040A RID: 1034
		void Process(EventDef source, EventDefUser injected, Importer importer);

		// Token: 0x0600040B RID: 1035
		void Process(PropertyDef source, PropertyDefUser injected, Importer importer);
	}
}
