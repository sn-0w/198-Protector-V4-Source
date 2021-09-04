using System;
using dnlib.DotNet;

namespace Confuser.Renamer
{
	// Token: 0x0200000F RID: 15
	public class VTableSignature
	{
		// Token: 0x06000067 RID: 103 RVA: 0x000089C8 File Offset: 0x00006BC8
		internal VTableSignature(MethodSig sig, string name)
		{
			this.MethodSig = sig;
			this.Name = name;
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000089E2 File Offset: 0x00006BE2
		// (set) Token: 0x06000069 RID: 105 RVA: 0x000089EA File Offset: 0x00006BEA
		public MethodSig MethodSig { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000089F3 File Offset: 0x00006BF3
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000089FB File Offset: 0x00006BFB
		public string Name { get; private set; }

		// Token: 0x0600006C RID: 108 RVA: 0x00008A04 File Offset: 0x00006C04
		public static VTableSignature FromMethod(IMethod method)
		{
			MethodSig methodSig = method.MethodSig;
			TypeSig typeSig = method.DeclaringType.ToTypeSig(true);
			bool flag = typeSig is GenericInstSig;
			if (flag)
			{
				methodSig = GenericArgumentResolver.Resolve(methodSig, ((GenericInstSig)typeSig).GenericArguments);
			}
			return new VTableSignature(methodSig, method.Name);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00008A5C File Offset: 0x00006C5C
		public override bool Equals(object obj)
		{
			VTableSignature vtableSignature = obj as VTableSignature;
			bool flag = vtableSignature == null;
			return !flag && default(SigComparer).Equals(this.MethodSig, vtableSignature.MethodSig) && this.Name.Equals(vtableSignature.Name, StringComparison.Ordinal);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00008AB8 File Offset: 0x00006CB8
		public override int GetHashCode()
		{
			int num = 17;
			num = num * 7 + default(SigComparer).GetHashCode(this.MethodSig);
			return num * 7 + this.Name.GetHashCode();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00008AF8 File Offset: 0x00006CF8
		public static bool operator ==(VTableSignature a, VTableSignature b)
		{
			bool flag = a == b;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = !object.Equals(a, null) && object.Equals(b, null);
				result = (!flag2 && a.Equals(b));
			}
			return result;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00008B38 File Offset: 0x00006D38
		public static bool operator !=(VTableSignature a, VTableSignature b)
		{
			return !(a == b);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00008B54 File Offset: 0x00006D54
		public override string ToString()
		{
			return FullNameFactory.MethodFullName("", this.Name, this.MethodSig, null, null, null, null);
		}
	}
}
