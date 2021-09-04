using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confuser.Protections.TypeScramble.Scrambler;
using dnlib.DotNet;

namespace Confuser.Protections.TypeScramble
{
	// Token: 0x02000037 RID: 55
	internal sealed class TypeService
	{
		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00002749 File Offset: 0x00000949
		internal bool ScrambledAnything
		{
			get
			{
				return this.GenericsMapper.Any<KeyValuePair<IMemberDef, ScannedItem>>();
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00002756 File Offset: 0x00000956
		internal void AddScannedItem(ScannedMethod m)
		{
			this.AddScannedItemGeneral(m);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000211A File Offset: 0x0000031A
		internal void AddScannedItem(ScannedType m)
		{
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00006E38 File Offset: 0x00005038
		private void AddScannedItemGeneral(ScannedItem m)
		{
			m.Scan();
			bool flag = !this.GenericsMapper.ContainsKey(m.GetMemberDef());
			if (flag)
			{
				this.GenericsMapper.Add(m.GetMemberDef(), m);
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00006E7C File Offset: 0x0000507C
		internal void PrepareItems()
		{
			foreach (ScannedItem scannedItem in this.GenericsMapper.Values)
			{
				scannedItem.PrepareGenerics();
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00006EDC File Offset: 0x000050DC
		private ScannedItem GetItemInternal(IMemberDef memberDef)
		{
			Debug.Assert(memberDef != null, "memberDef != null");
			ScannedItem scannedItem;
			bool flag = this.GenericsMapper.TryGetValue(memberDef, out scannedItem);
			ScannedItem result;
			if (flag)
			{
				result = scannedItem;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00002760 File Offset: 0x00000960
		internal ScannedMethod GetItem(MethodDef methodDef)
		{
			return this.GetItemInternal(methodDef) as ScannedMethod;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000276E File Offset: 0x0000096E
		internal ScannedType GetItem(TypeDef typeDef)
		{
			return this.GetItemInternal(typeDef) as ScannedType;
		}

		// Token: 0x0400003F RID: 63
		private Dictionary<IMemberDef, ScannedItem> GenericsMapper = new Dictionary<IMemberDef, ScannedItem>();
	}
}
