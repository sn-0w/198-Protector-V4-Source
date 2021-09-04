using System;
using System.Collections.Generic;

namespace dnlib.DotNet
{
	// Token: 0x0200007B RID: 123
	internal struct GenericArgumentsStack
	{
		// Token: 0x0600030C RID: 780 RVA: 0x0002A1B3 File Offset: 0x000283B3
		public GenericArgumentsStack(bool isTypeVar)
		{
			this.argsStack = new List<IList<TypeSig>>();
			this.isTypeVar = isTypeVar;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0002A1C8 File Offset: 0x000283C8
		public void Push(IList<TypeSig> args)
		{
			this.argsStack.Add(args);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0002A1D8 File Offset: 0x000283D8
		public IList<TypeSig> Pop()
		{
			int index = this.argsStack.Count - 1;
			IList<TypeSig> result = this.argsStack[index];
			this.argsStack.RemoveAt(index);
			return result;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0002A214 File Offset: 0x00028414
		public TypeSig Resolve(uint number)
		{
			TypeSig result = null;
			int i = this.argsStack.Count - 1;
			while (i >= 0)
			{
				IList<TypeSig> list = this.argsStack[i];
				bool flag = (ulong)number >= (ulong)((long)list.Count);
				TypeSig result2;
				if (flag)
				{
					result2 = null;
				}
				else
				{
					TypeSig typeSig = list[(int)number];
					GenericSig genericSig = typeSig as GenericSig;
					bool flag2 = genericSig == null || genericSig.IsTypeVar != this.isTypeVar;
					if (!flag2)
					{
						result = genericSig;
						number = genericSig.Number;
						i--;
						continue;
					}
					result2 = typeSig;
				}
				return result2;
			}
			return result;
		}

		// Token: 0x0400053D RID: 1341
		private readonly List<IList<TypeSig>> argsStack;

		// Token: 0x0400053E RID: 1342
		private readonly bool isTypeVar;
	}
}
