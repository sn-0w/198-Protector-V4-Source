using System;
using System.Collections.Generic;
using System.Diagnostics;
using dnlib.DotNet;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000022 RID: 34
	internal class BamlElement
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000EB RID: 235 RVA: 0x0000C1E6 File Offset: 0x0000A3E6
		// (set) Token: 0x060000EC RID: 236 RVA: 0x0000C1EE File Offset: 0x0000A3EE
		public BamlElement Parent { get; private set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000ED RID: 237 RVA: 0x0000C1F7 File Offset: 0x0000A3F7
		// (set) Token: 0x060000EE RID: 238 RVA: 0x0000C1FF File Offset: 0x0000A3FF
		public BamlRecord Header { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000EF RID: 239 RVA: 0x0000C208 File Offset: 0x0000A408
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x0000C210 File Offset: 0x0000A410
		public IList<BamlRecord> Body { get; private set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000C219 File Offset: 0x0000A419
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x0000C221 File Offset: 0x0000A421
		public IList<BamlElement> Children { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x0000C22A File Offset: 0x0000A42A
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x0000C232 File Offset: 0x0000A432
		public BamlRecord Footer { get; private set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000C23B File Offset: 0x0000A43B
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x0000C243 File Offset: 0x0000A443
		public TypeDef Type { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x0000C24C File Offset: 0x0000A44C
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x0000C254 File Offset: 0x0000A454
		public IDnlibDef Attribute { get; set; }

		// Token: 0x060000F9 RID: 249 RVA: 0x0000C260 File Offset: 0x0000A460
		private static bool IsHeader(BamlRecord rec)
		{
			BamlRecordType type = rec.Type;
			BamlRecordType bamlRecordType = type;
			if (bamlRecordType <= BamlRecordType.PropertyDictionaryStart)
			{
				if (bamlRecordType != BamlRecordType.DocumentStart && bamlRecordType != BamlRecordType.ElementStart)
				{
					switch (bamlRecordType)
					{
					case BamlRecordType.PropertyComplexStart:
					case BamlRecordType.PropertyArrayStart:
					case BamlRecordType.PropertyListStart:
					case BamlRecordType.PropertyDictionaryStart:
						break;
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
						goto IL_5C;
					default:
						goto IL_5C;
					}
				}
			}
			else if (bamlRecordType != BamlRecordType.KeyElementStart && bamlRecordType != BamlRecordType.ConstructorParametersStart && bamlRecordType - BamlRecordType.NamedElementStart > 1)
			{
				goto IL_5C;
			}
			return true;
			IL_5C:
			return false;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000C2D0 File Offset: 0x0000A4D0
		private static bool IsFooter(BamlRecord rec)
		{
			BamlRecordType type = rec.Type;
			BamlRecordType bamlRecordType = type;
			if (bamlRecordType <= BamlRecordType.PropertyDictionaryEnd)
			{
				if (bamlRecordType != BamlRecordType.DocumentEnd && bamlRecordType != BamlRecordType.ElementEnd)
				{
					switch (bamlRecordType)
					{
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
					case BamlRecordType.PropertyDictionaryEnd:
						break;
					case BamlRecordType.PropertyArrayStart:
					case BamlRecordType.PropertyListStart:
					case BamlRecordType.PropertyDictionaryStart:
						goto IL_5A;
					default:
						goto IL_5A;
					}
				}
			}
			else if (bamlRecordType != BamlRecordType.KeyElementEnd && bamlRecordType != BamlRecordType.ConstructorParametersEnd && bamlRecordType != BamlRecordType.StaticResourceEnd)
			{
				goto IL_5A;
			}
			return true;
			IL_5A:
			return false;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000C33C File Offset: 0x0000A53C
		private static bool IsMatch(BamlRecord header, BamlRecord footer)
		{
			BamlRecordType type = header.Type;
			BamlRecordType bamlRecordType = type;
			if (bamlRecordType <= BamlRecordType.PropertyDictionaryStart)
			{
				if (bamlRecordType == BamlRecordType.DocumentStart)
				{
					return footer.Type == BamlRecordType.DocumentEnd;
				}
				if (bamlRecordType != BamlRecordType.ElementStart)
				{
					switch (bamlRecordType)
					{
					case BamlRecordType.PropertyComplexStart:
						return footer.Type == BamlRecordType.PropertyComplexEnd;
					case BamlRecordType.PropertyComplexEnd:
					case BamlRecordType.PropertyArrayEnd:
					case BamlRecordType.PropertyListEnd:
						goto IL_DD;
					case BamlRecordType.PropertyArrayStart:
						return footer.Type == BamlRecordType.PropertyArrayEnd;
					case BamlRecordType.PropertyListStart:
						return footer.Type == BamlRecordType.PropertyListEnd;
					case BamlRecordType.PropertyDictionaryStart:
						return footer.Type == BamlRecordType.PropertyDictionaryEnd;
					default:
						goto IL_DD;
					}
				}
			}
			else if (bamlRecordType <= BamlRecordType.ConstructorParametersStart)
			{
				if (bamlRecordType == BamlRecordType.KeyElementStart)
				{
					return footer.Type == BamlRecordType.KeyElementEnd;
				}
				if (bamlRecordType != BamlRecordType.ConstructorParametersStart)
				{
					goto IL_DD;
				}
				return footer.Type == BamlRecordType.ConstructorParametersEnd;
			}
			else if (bamlRecordType != BamlRecordType.NamedElementStart)
			{
				if (bamlRecordType != BamlRecordType.StaticResourceStart)
				{
					goto IL_DD;
				}
				return footer.Type == BamlRecordType.StaticResourceEnd;
			}
			return footer.Type == BamlRecordType.ElementEnd;
			IL_DD:
			return false;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000C42C File Offset: 0x0000A62C
		public static BamlElement Read(BamlDocument document)
		{
			Debug.Assert(document.Count > 0 && document[0].Type == BamlRecordType.DocumentStart);
			BamlElement bamlElement = null;
			Stack<BamlElement> stack = new Stack<BamlElement>();
			for (int i = 0; i < document.Count; i++)
			{
				bool flag = BamlElement.IsHeader(document[i]);
				if (flag)
				{
					BamlElement bamlElement2 = bamlElement;
					bamlElement = new BamlElement();
					bamlElement.Header = document[i];
					bamlElement.Body = new List<BamlRecord>();
					bamlElement.Children = new List<BamlElement>();
					bool flag2 = bamlElement2 != null;
					if (flag2)
					{
						bamlElement2.Children.Add(bamlElement);
						bamlElement.Parent = bamlElement2;
						stack.Push(bamlElement2);
					}
				}
				else
				{
					bool flag3 = BamlElement.IsFooter(document[i]);
					if (flag3)
					{
						bool flag4 = bamlElement == null;
						if (flag4)
						{
							throw new Exception("Unexpected footer.");
						}
						while (!BamlElement.IsMatch(bamlElement.Header, document[i]))
						{
							bool flag5 = stack.Count > 0;
							if (flag5)
							{
								bamlElement = stack.Pop();
							}
						}
						bamlElement.Footer = document[i];
						bool flag6 = stack.Count > 0;
						if (flag6)
						{
							bamlElement = stack.Pop();
						}
					}
					else
					{
						bamlElement.Body.Add(document[i]);
					}
				}
			}
			Debug.Assert(stack.Count == 0);
			return bamlElement;
		}
	}
}
