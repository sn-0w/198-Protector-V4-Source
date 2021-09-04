using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Protections.AntiMemoryEditing
{
	// Token: 0x0200010E RID: 270
	internal class MemoryEditService : IMemoryEditService
	{
		// Token: 0x0600048D RID: 1165 RVA: 0x000038BF File Offset: 0x00001ABF
		public void AddToList(FieldDef d)
		{
			this._fields.Add(d);
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x000038CE File Offset: 0x00001ACE
		public IEnumerable<FieldDef> GetFields()
		{
			return this._fields;
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x000038D6 File Offset: 0x00001AD6
		public TypeDef GetWrapperType(ModuleDef mod)
		{
			return this._wrapperTypes.ContainsKey(mod) ? this._wrapperTypes[mod] : null;
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x000038F5 File Offset: 0x00001AF5
		public void SetWrapperType(ModuleDef mod, TypeDef t)
		{
			this._wrapperTypes[mod] = t;
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00003905 File Offset: 0x00001B05
		public IMethod GetReadMethod(ModuleDef mod)
		{
			return this._readMethods[mod];
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00003913 File Offset: 0x00001B13
		public IMethod GetWriteMethod(ModuleDef mod)
		{
			return this._writeMethods[mod];
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00003921 File Offset: 0x00001B21
		public void SetReadMethod(ModuleDef mod, IMethod m)
		{
			this._readMethods[mod] = m;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00003931 File Offset: 0x00001B31
		public void SetWriteMethod(ModuleDef mod, IMethod m)
		{
			this._writeMethods[mod] = m;
		}

		// Token: 0x04000255 RID: 597
		private readonly List<FieldDef> _fields = new List<FieldDef>();

		// Token: 0x04000256 RID: 598
		private readonly Dictionary<ModuleDef, IMethod> _readMethods = new Dictionary<ModuleDef, IMethod>();

		// Token: 0x04000257 RID: 599
		private readonly Dictionary<ModuleDef, IMethod> _writeMethods = new Dictionary<ModuleDef, IMethod>();

		// Token: 0x04000258 RID: 600
		private readonly Dictionary<ModuleDef, TypeDef> _wrapperTypes = new Dictionary<ModuleDef, TypeDef>();
	}
}
