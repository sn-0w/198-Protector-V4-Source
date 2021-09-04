using System;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000096 RID: 150
	public class MemberTypeFunction : PatternFunction
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x0600039D RID: 925 RVA: 0x00015AE8 File Offset: 0x00013CE8
		public override string Name
		{
			get
			{
				return "member-type";
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x0600039E RID: 926 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00015B00 File Offset: 0x00013D00
		public override object Evaluate(IDnlibDef definition)
		{
			string pattern = base.Arguments[0].Evaluate(definition).ToString();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = definition is TypeDef;
			if (flag)
			{
				stringBuilder.Append("type ");
			}
			bool flag2 = definition is MethodDef;
			if (flag2)
			{
				stringBuilder.Append("method ");
				MethodDef methodDef = (MethodDef)definition;
				bool isGetter = methodDef.IsGetter;
				if (isGetter)
				{
					stringBuilder.Append("propertym getter ");
				}
				else
				{
					bool isSetter = methodDef.IsSetter;
					if (isSetter)
					{
						stringBuilder.Append("propertym setter ");
					}
					else
					{
						bool isAddOn = methodDef.IsAddOn;
						if (isAddOn)
						{
							stringBuilder.Append("eventm add ");
						}
						else
						{
							bool isRemoveOn = methodDef.IsRemoveOn;
							if (isRemoveOn)
							{
								stringBuilder.Append("eventm remove ");
							}
							else
							{
								bool isFire = methodDef.IsFire;
								if (isFire)
								{
									stringBuilder.Append("eventm fire ");
								}
								else
								{
									bool isOther = methodDef.IsOther;
									if (isOther)
									{
										stringBuilder.Append("other ");
									}
								}
							}
						}
					}
				}
			}
			bool flag3 = definition is FieldDef;
			if (flag3)
			{
				stringBuilder.Append("field ");
			}
			bool flag4 = definition is PropertyDef;
			if (flag4)
			{
				stringBuilder.Append("property ");
			}
			bool flag5 = definition is EventDef;
			if (flag5)
			{
				stringBuilder.Append("event ");
			}
			bool flag6 = definition is ModuleDef;
			if (flag6)
			{
				stringBuilder.Append("module ");
			}
			return Regex.IsMatch(stringBuilder.ToString(), pattern);
		}

		// Token: 0x0400026E RID: 622
		internal const string FnName = "member-type";
	}
}
