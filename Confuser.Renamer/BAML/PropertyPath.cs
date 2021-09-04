using System;
using System.Collections.Generic;
using System.Text;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000067 RID: 103
	internal class PropertyPath
	{
		// Token: 0x06000293 RID: 659 RVA: 0x00024574 File Offset: 0x00022774
		public PropertyPath(string path)
		{
			this.parts = PropertyPath.Parse(path);
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0002458C File Offset: 0x0002278C
		public PropertyPathPart[] Parts
		{
			get
			{
				return this.parts;
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000245A4 File Offset: 0x000227A4
		private static PropertyPathPart ReadIndexer(string path, ref int index, bool? isHiera)
		{
			index++;
			List<PropertyPathIndexer> list = new List<PropertyPathIndexer>();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			bool flag = false;
			int num = 0;
			int num2 = 0;
			while (num2 != 3 && index < path.Length)
			{
				char c = path[index];
				switch (num2)
				{
				case 0:
				{
					bool flag2 = c == '(';
					if (flag2)
					{
						index++;
						num2 = 1;
					}
					else
					{
						bool flag3 = c == '^';
						if (flag3)
						{
							StringBuilder stringBuilder3 = stringBuilder2;
							int num3 = index + 1;
							index = num3;
							stringBuilder3.Append(path[num3]);
							index++;
							num2 = 2;
						}
						else
						{
							bool flag4 = char.IsWhiteSpace(c);
							if (flag4)
							{
								index++;
							}
							else
							{
								StringBuilder stringBuilder4 = stringBuilder2;
								int num3 = index;
								index = num3 + 1;
								stringBuilder4.Append(path[num3]);
								num2 = 2;
							}
						}
					}
					break;
				}
				case 1:
				{
					bool flag5 = c == ')';
					if (flag5)
					{
						index++;
						num2 = 2;
					}
					else
					{
						bool flag6 = c == '^';
						if (flag6)
						{
							StringBuilder stringBuilder5 = stringBuilder;
							int num3 = index + 1;
							index = num3;
							stringBuilder5.Append(path[num3]);
							index++;
						}
						else
						{
							StringBuilder stringBuilder6 = stringBuilder;
							int num3 = index;
							index = num3 + 1;
							stringBuilder6.Append(path[num3]);
						}
					}
					break;
				}
				case 2:
				{
					bool flag7 = c == '[';
					if (flag7)
					{
						StringBuilder stringBuilder7 = stringBuilder2;
						int num3 = index;
						index = num3 + 1;
						stringBuilder7.Append(path[num3]);
						num++;
						flag = false;
					}
					else
					{
						bool flag8 = c == '^';
						if (flag8)
						{
							StringBuilder stringBuilder8 = stringBuilder2;
							int num3 = index + 1;
							index = num3;
							stringBuilder8.Append(path[num3]);
							index++;
							flag = false;
						}
						else
						{
							bool flag9 = num > 0 && c == ']';
							if (flag9)
							{
								num--;
								StringBuilder stringBuilder9 = stringBuilder2;
								int num3 = index;
								index = num3 + 1;
								stringBuilder9.Append(path[num3]);
								flag = false;
							}
							else
							{
								bool flag10 = c == ']' || c == ',';
								if (flag10)
								{
									string text = stringBuilder2.ToString();
									bool flag11 = flag;
									if (flag11)
									{
										text.TrimEnd(new char[0]);
									}
									list.Add(new PropertyPathIndexer
									{
										Type = stringBuilder.ToString(),
										Value = text
									});
									stringBuilder2.Length = 0;
									stringBuilder.Length = 0;
									flag = false;
									index++;
									bool flag12 = c == ',';
									if (flag12)
									{
										num2 = 0;
									}
									else
									{
										num2 = 3;
									}
								}
								else
								{
									StringBuilder stringBuilder10 = stringBuilder2;
									int num3 = index;
									index = num3 + 1;
									stringBuilder10.Append(path[num3]);
									bool flag13 = c == ' ' && num == 0;
									flag = flag13;
								}
							}
						}
					}
					break;
				}
				}
			}
			return new PropertyPathPart(true, isHiera, "Item")
			{
				IndexerArguments = list.ToArray()
			};
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0002488C File Offset: 0x00022A8C
		private static PropertyPathPart ReadProperty(string path, ref int index, bool? isHiera)
		{
			int num = index;
			while (index < path.Length && path[index] == '.')
			{
				index++;
			}
			int num2 = 0;
			while (index < path.Length && (num2 > 0 || Array.IndexOf<char>(PropertyPath.SpecialChars, path[index]) == -1))
			{
				bool flag = path[index] == '(';
				if (flag)
				{
					num2++;
				}
				else
				{
					bool flag2 = path[index] == ')';
					if (flag2)
					{
						num2--;
					}
				}
				index++;
			}
			string name = path.Substring(num, index - num).Trim();
			return new PropertyPathPart(false, isHiera, name);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0002494C File Offset: 0x00022B4C
		private static PropertyPathPart[] Parse(string path)
		{
			bool flag = string.IsNullOrEmpty(path);
			PropertyPathPart[] result;
			if (flag)
			{
				result = new PropertyPathPart[]
				{
					new PropertyPathPart(true, null, "")
				};
			}
			else
			{
				List<PropertyPathPart> list = new List<PropertyPathPart>();
				bool? isHiera = null;
				int i = 0;
				while (i < path.Length)
				{
					bool flag2 = char.IsWhiteSpace(path[i]);
					if (flag2)
					{
						i++;
					}
					else
					{
						char c = path[i];
						char c2 = c;
						char c3 = c2;
						if (c3 != '.')
						{
							if (c3 != '/')
							{
								if (c3 != '[')
								{
									list.Add(PropertyPath.ReadProperty(path, ref i, isHiera));
									isHiera = null;
								}
								else
								{
									list.Add(PropertyPath.ReadIndexer(path, ref i, isHiera));
									isHiera = null;
								}
							}
							else
							{
								isHiera = new bool?(true);
								i++;
							}
						}
						else
						{
							isHiera = new bool?(false);
							i++;
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x00024A50 File Offset: 0x00022C50
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyPathPart propertyPathPart in this.parts)
			{
				bool flag = propertyPathPart.IsHierarchical != null;
				if (flag)
				{
					bool value = propertyPathPart.IsHierarchical.Value;
					if (value)
					{
						stringBuilder.Append("/");
					}
					else
					{
						stringBuilder.Append(".");
					}
				}
				stringBuilder.Append(propertyPathPart.Name);
				bool isIndexer = propertyPathPart.IsIndexer;
				if (isIndexer)
				{
					PropertyPathIndexer[] indexerArguments = propertyPathPart.IndexerArguments;
					for (int j = 0; j < indexerArguments.Length; j++)
					{
						bool flag2 = j == 0;
						if (flag2)
						{
							stringBuilder.Append("[");
						}
						else
						{
							stringBuilder.Append(",");
						}
						bool flag3 = !string.IsNullOrEmpty(indexerArguments[j].Type);
						if (flag3)
						{
							stringBuilder.AppendFormat("({0})", indexerArguments[j].Type);
						}
						bool flag4 = !string.IsNullOrEmpty(indexerArguments[j].Value);
						if (flag4)
						{
							foreach (char c in indexerArguments[j].Value)
							{
								bool flag5 = c == '[' || c == ']' || c == ' ';
								if (flag5)
								{
									stringBuilder.Append("^");
								}
								stringBuilder.Append(c);
							}
						}
					}
					stringBuilder.Append("]");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000516 RID: 1302
		private static readonly char[] SpecialChars = new char[]
		{
			'.',
			'/',
			'[',
			']'
		};

		// Token: 0x04000517 RID: 1303
		private readonly PropertyPathPart[] parts;
	}
}
