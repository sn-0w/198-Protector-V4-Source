using System;
using System.Collections.Generic;

namespace dnlib.DotNet
{
	// Token: 0x0200007C RID: 124
	public sealed class GenericArguments
	{
		// Token: 0x06000310 RID: 784 RVA: 0x0002A2B4 File Offset: 0x000284B4
		public void PushTypeArgs(IList<TypeSig> typeArgs)
		{
			this.typeArgsStack.Push(typeArgs);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0002A2C4 File Offset: 0x000284C4
		public IList<TypeSig> PopTypeArgs()
		{
			return this.typeArgsStack.Pop();
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0002A2E1 File Offset: 0x000284E1
		public void PushMethodArgs(IList<TypeSig> methodArgs)
		{
			this.methodArgsStack.Push(methodArgs);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0002A2F4 File Offset: 0x000284F4
		public IList<TypeSig> PopMethodArgs()
		{
			return this.methodArgsStack.Pop();
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0002A314 File Offset: 0x00028514
		public TypeSig Resolve(TypeSig typeSig)
		{
			bool flag = typeSig == null;
			TypeSig result;
			if (flag)
			{
				result = null;
			}
			else
			{
				GenericMVar genericMVar = typeSig as GenericMVar;
				bool flag2 = genericMVar != null;
				if (flag2)
				{
					TypeSig typeSig2 = this.methodArgsStack.Resolve(genericMVar.Number);
					bool flag3 = typeSig2 == null || typeSig2 == typeSig;
					if (flag3)
					{
						result = typeSig;
					}
					else
					{
						result = typeSig2;
					}
				}
				else
				{
					GenericVar genericVar = typeSig as GenericVar;
					bool flag4 = genericVar != null;
					if (flag4)
					{
						TypeSig typeSig3 = this.typeArgsStack.Resolve(genericVar.Number);
						bool flag5 = typeSig3 == null || typeSig3 == typeSig;
						if (flag5)
						{
							result = typeSig;
						}
						else
						{
							result = typeSig3;
						}
					}
					else
					{
						result = typeSig;
					}
				}
			}
			return result;
		}

		// Token: 0x0400053F RID: 1343
		private GenericArgumentsStack typeArgsStack = new GenericArgumentsStack(true);

		// Token: 0x04000540 RID: 1344
		private GenericArgumentsStack methodArgsStack = new GenericArgumentsStack(false);
	}
}
