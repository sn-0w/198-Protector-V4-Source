using System;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000093 RID: 147
	public class IsPublicFunction : PatternFunction
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000391 RID: 913 RVA: 0x000158E4 File Offset: 0x00013AE4
		public override string Name
		{
			get
			{
				return "is-public";
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000392 RID: 914 RVA: 0x000158FC File Offset: 0x00013AFC
		public override int ArgumentCount
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00015910 File Offset: 0x00013B10
		public override object Evaluate(IDnlibDef definition)
		{
			IMemberDef memberDef = definition as IMemberDef;
			bool flag = memberDef == null;
			object result;
			if (flag)
			{
				result = false;
			}
			else
			{
				for (TypeDef declaringType = ((IMemberDef)definition).DeclaringType; declaringType != null; declaringType = declaringType.DeclaringType)
				{
					bool flag2 = !declaringType.IsPublic;
					if (flag2)
					{
						return false;
					}
				}
				bool flag3 = memberDef is MethodDef;
				if (flag3)
				{
					result = ((MethodDef)memberDef).IsPublic;
				}
				else
				{
					bool flag4 = memberDef is FieldDef;
					if (flag4)
					{
						result = ((FieldDef)memberDef).IsPublic;
					}
					else
					{
						bool flag5 = memberDef is PropertyDef;
						if (flag5)
						{
							result = ((PropertyDef)memberDef).IsPublic();
						}
						else
						{
							bool flag6 = memberDef is EventDef;
							if (flag6)
							{
								result = ((EventDef)memberDef).IsPublic();
							}
							else
							{
								bool flag7 = memberDef is TypeDef;
								if (!flag7)
								{
									throw new NotSupportedException();
								}
								result = (((TypeDef)memberDef).IsPublic || ((TypeDef)memberDef).IsNestedPublic);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0400026B RID: 619
		internal const string FnName = "is-public";
	}
}
