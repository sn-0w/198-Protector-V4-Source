using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000068 RID: 104
	internal static class PropertyPathExtensions
	{
		// Token: 0x0600029A RID: 666 RVA: 0x00024C10 File Offset: 0x00022E10
		internal static string GetTypeName(this SourceValueInfo info)
		{
			string name = info.name;
			string text = (name != null) ? name.Trim() : null;
			bool flag = text != null && text.StartsWith("(") && text.EndsWith(")");
			string result;
			if (flag)
			{
				int num = text.LastIndexOf(".");
				bool flag2 = num < 0;
				if (flag2)
				{
					result = null;
				}
				else
				{
					result = text.Substring(1, num - 1);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00024C84 File Offset: 0x00022E84
		internal static string GetPropertyName(this SourceValueInfo info)
		{
			string result;
			switch (info.type)
			{
			case SourceValueType.Property:
			{
				string name = info.name;
				string text = (name != null) ? name.Trim() : null;
				bool flag = text != null && text.StartsWith("(") && text.EndsWith(")");
				if (flag)
				{
					int num = text.LastIndexOf(".");
					bool flag2 = num < 0;
					if (flag2)
					{
						result = text.Substring(1, text.Length - 2);
					}
					else
					{
						result = text.Substring(num + 1, text.Length - num - 2);
					}
				}
				else
				{
					result = text;
				}
				break;
			}
			case SourceValueType.Indexer:
				result = "Item";
				break;
			case SourceValueType.Direct:
				result = null;
				break;
			default:
				throw new InvalidOperationException("Unexpected SourceValueType.");
			}
			return result;
		}
	}
}
