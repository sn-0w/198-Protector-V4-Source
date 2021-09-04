using System;
using System.Text;
using System.Text.RegularExpressions;
using dnlib.DotNet;

namespace Confuser.Core.Project.Patterns
{
	// Token: 0x02000091 RID: 145
	public class IsTypeFunction : PatternFunction
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000389 RID: 905 RVA: 0x000156FC File Offset: 0x000138FC
		public override string Name
		{
			get
			{
				return "is-type";
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600038A RID: 906 RVA: 0x000156AC File Offset: 0x000138AC
		public override int ArgumentCount
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00015714 File Offset: 0x00013914
		public override object Evaluate(IDnlibDef definition)
		{
			TypeDef typeDef = definition as TypeDef;
			bool flag = typeDef == null && definition is IMemberDef;
			if (flag)
			{
				typeDef = ((IMemberDef)definition).DeclaringType;
			}
			bool flag2 = typeDef == null;
			object result;
			if (flag2)
			{
				result = false;
			}
			else
			{
				string pattern = base.Arguments[0].Evaluate(definition).ToString();
				StringBuilder stringBuilder = new StringBuilder();
				bool isEnum = typeDef.IsEnum;
				if (isEnum)
				{
					stringBuilder.Append("enum ");
				}
				bool isInterface = typeDef.IsInterface;
				if (isInterface)
				{
					stringBuilder.Append("interface ");
				}
				bool isValueType = typeDef.IsValueType;
				if (isValueType)
				{
					stringBuilder.Append("valuetype ");
				}
				bool flag3 = typeDef.IsDelegate();
				if (flag3)
				{
					stringBuilder.Append("delegate ");
				}
				bool isAbstract = typeDef.IsAbstract;
				if (isAbstract)
				{
					stringBuilder.Append("abstract ");
				}
				bool isNested = typeDef.IsNested;
				if (isNested)
				{
					stringBuilder.Append("nested ");
				}
				bool isSerializable = typeDef.IsSerializable;
				if (isSerializable)
				{
					stringBuilder.Append("serializable ");
				}
				result = Regex.IsMatch(stringBuilder.ToString(), pattern);
			}
			return result;
		}

		// Token: 0x04000269 RID: 617
		internal const string FnName = "is-type";
	}
}
