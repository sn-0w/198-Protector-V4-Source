using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Confuser.Renamer.Properties;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200006B RID: 107
	internal sealed class PropertyPathParser
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00024D5C File Offset: 0x00022F5C
		// (set) Token: 0x0600029E RID: 670 RVA: 0x00024D64 File Offset: 0x00022F64
		internal string Error { get; private set; }

		// Token: 0x0600029F RID: 671 RVA: 0x00024D70 File Offset: 0x00022F70
		private void SetError(string message, params string[] args)
		{
			this.Error = string.Format(CultureInfo.CurrentUICulture, message, args);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x00024D94 File Offset: 0x00022F94
		public SourceValueInfo[] Parse(string path)
		{
			this._path = ((path != null) ? path.Trim() : string.Empty);
			this._n = this._path.Length;
			bool flag = this._n == 0;
			SourceValueInfo[] result;
			if (flag)
			{
				result = new SourceValueInfo[]
				{
					new SourceValueInfo(SourceValueType.Direct, DrillIn.Never, null)
				};
			}
			else
			{
				this._index = 0;
				this._drillIn = DrillIn.IfNeeded;
				this._al.Clear();
				this.Error = null;
				this._state = PropertyPathParser.State.Init;
				while (this._state != PropertyPathParser.State.Done)
				{
					char c = (this._index < this._n) ? this._path[this._index] : '\0';
					bool flag2 = char.IsWhiteSpace(c);
					if (flag2)
					{
						this._index++;
					}
					else
					{
						switch (this._state)
						{
						case PropertyPathParser.State.Init:
						{
							char c2 = c;
							char c3 = c2;
							if (c3 != '\0' && c3 != '.' && c3 != '/')
							{
								this._state = PropertyPathParser.State.Prop;
							}
							else
							{
								this._state = PropertyPathParser.State.DrillIn;
							}
							break;
						}
						case PropertyPathParser.State.DrillIn:
						{
							char c4 = c;
							char c5 = c4;
							if (c5 <= '.')
							{
								if (c5 != '\0')
								{
									if (c5 != '.')
									{
										goto IL_176;
									}
									this._drillIn = DrillIn.Never;
									this._index++;
								}
							}
							else if (c5 != '/')
							{
								if (c5 != '[')
								{
									goto IL_176;
								}
							}
							else
							{
								this._drillIn = DrillIn.Always;
								this._index++;
							}
							IL_1BC:
							this._state = PropertyPathParser.State.Prop;
							break;
							goto IL_1BC;
							IL_176:
							this.SetError(Resources.InvalidPathSyntax, new string[]
							{
								this._path.Substring(0, this._index),
								this._path.Substring(this._index)
							});
							return PropertyPathParser.EmptyInfo;
						}
						case PropertyPathParser.State.Prop:
						{
							bool flag3 = false;
							char c6 = c;
							char c7 = c6;
							if (c7 == '[')
							{
								flag3 = true;
							}
							bool flag4 = flag3;
							if (flag4)
							{
								this.AddIndexer();
							}
							else
							{
								this.AddProperty();
							}
							break;
						}
						}
					}
				}
				bool flag5 = this.Error == null;
				SourceValueInfo[] array;
				if (flag5)
				{
					array = new SourceValueInfo[this._al.Count];
					this._al.CopyTo(array);
				}
				else
				{
					array = PropertyPathParser.EmptyInfo;
				}
				result = array;
			}
			return result;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00024FF0 File Offset: 0x000231F0
		private void AddProperty()
		{
			int index = this._index;
			int num = 0;
			while (this._index < this._n && this._path[this._index] == '.')
			{
				this._index++;
			}
			while (this._index < this._n && (num > 0 || PropertyPathParser.SpecialChars.IndexOf(this._path[this._index]) < 0))
			{
				bool flag = this._path[this._index] == '(';
				if (flag)
				{
					num++;
				}
				else
				{
					bool flag2 = this._path[this._index] == ')';
					if (flag2)
					{
						num--;
					}
				}
				this._index++;
			}
			bool flag3 = num > 0;
			if (flag3)
			{
				this.SetError(Resources.UnmatchedParen, new string[]
				{
					this._path.Substring(index)
				});
			}
			else
			{
				bool flag4 = num < 0;
				if (flag4)
				{
					this.SetError(Resources.UnmatchedParen, new string[]
					{
						this._path.Substring(0, this._index)
					});
				}
				else
				{
					string text = this._path.Substring(index, this._index - index).Trim();
					SourceValueInfo sourceValueInfo = (text.Length > 0) ? new SourceValueInfo(SourceValueType.Property, this._drillIn, text) : new SourceValueInfo(SourceValueType.Direct, this._drillIn, null);
					this._al.Add(sourceValueInfo);
					this.StartNewLevel();
				}
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00025194 File Offset: 0x00023394
		private void AddIndexer()
		{
			int num = this._index + 1;
			this._index = num;
			int num2 = num;
			int num3 = 1;
			bool flag = false;
			bool flag2 = false;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			List<IndexerParamInfo> list = new List<IndexerParamInfo>(0);
			PropertyPathParser.IndexerState indexerState = PropertyPathParser.IndexerState.BeginParam;
			while (indexerState != PropertyPathParser.IndexerState.Done)
			{
				bool flag3 = this._index >= this._n;
				if (flag3)
				{
					this.SetError(Resources.UnmatchedBracket, new string[]
					{
						this._path.Substring(num2 - 1)
					});
					return;
				}
				string path = this._path;
				num = this._index;
				this._index = num + 1;
				char c = path[num];
				bool flag4 = c == '^' && !flag;
				if (!flag4)
				{
					switch (indexerState)
					{
					case PropertyPathParser.IndexerState.BeginParam:
					{
						bool flag5 = flag;
						if (flag5)
						{
							indexerState = PropertyPathParser.IndexerState.ValueString;
							goto IL_14C;
						}
						bool flag6 = c == '(';
						if (flag6)
						{
							indexerState = PropertyPathParser.IndexerState.ParenString;
						}
						else
						{
							bool flag7 = char.IsWhiteSpace(c);
							if (!flag7)
							{
								indexerState = PropertyPathParser.IndexerState.ValueString;
								goto IL_14C;
							}
						}
						break;
					}
					case PropertyPathParser.IndexerState.ParenString:
					{
						bool flag8 = flag;
						if (flag8)
						{
							stringBuilder.Append(c);
						}
						else
						{
							bool flag9 = c == ')';
							if (flag9)
							{
								indexerState = PropertyPathParser.IndexerState.ValueString;
							}
							else
							{
								stringBuilder.Append(c);
							}
						}
						break;
					}
					case PropertyPathParser.IndexerState.ValueString:
						goto IL_14C;
					}
					IL_24B:
					flag = false;
					continue;
					IL_14C:
					bool flag10 = flag;
					if (flag10)
					{
						stringBuilder2.Append(c);
						flag2 = false;
					}
					else
					{
						bool flag11 = num3 > 1;
						if (flag11)
						{
							stringBuilder2.Append(c);
							flag2 = false;
							bool flag12 = c == ']';
							if (flag12)
							{
								num3--;
							}
						}
						else
						{
							bool flag13 = char.IsWhiteSpace(c);
							if (flag13)
							{
								stringBuilder2.Append(c);
								flag2 = true;
							}
							else
							{
								bool flag14 = c == ',' || c == ']';
								if (flag14)
								{
									string paren = stringBuilder.ToString();
									string text = stringBuilder2.ToString();
									bool flag15 = flag2;
									if (flag15)
									{
										text = text.TrimEnd(new char[0]);
									}
									list.Add(new IndexerParamInfo(paren, text));
									stringBuilder.Length = 0;
									stringBuilder2.Length = 0;
									flag2 = false;
									indexerState = ((c == ']') ? PropertyPathParser.IndexerState.Done : PropertyPathParser.IndexerState.BeginParam);
								}
								else
								{
									stringBuilder2.Append(c);
									flag2 = false;
									bool flag16 = c == '[';
									if (flag16)
									{
										num3++;
									}
								}
							}
						}
					}
					goto IL_24B;
				}
				flag = true;
			}
			SourceValueInfo sourceValueInfo = new SourceValueInfo(SourceValueType.Indexer, this._drillIn, list);
			this._al.Add(sourceValueInfo);
			this.StartNewLevel();
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0002542A File Offset: 0x0002362A
		private void StartNewLevel()
		{
			this._state = ((this._index < this._n) ? PropertyPathParser.State.DrillIn : PropertyPathParser.State.Done);
			this._drillIn = DrillIn.Never;
		}

		// Token: 0x0400051F RID: 1311
		private PropertyPathParser.State _state;

		// Token: 0x04000520 RID: 1312
		private string _path;

		// Token: 0x04000521 RID: 1313
		private int _index;

		// Token: 0x04000522 RID: 1314
		private int _n;

		// Token: 0x04000523 RID: 1315
		private DrillIn _drillIn;

		// Token: 0x04000524 RID: 1316
		private ArrayList _al = new ArrayList();

		// Token: 0x04000525 RID: 1317
		private const char NullChar = '\0';

		// Token: 0x04000526 RID: 1318
		private const char EscapeChar = '^';

		// Token: 0x04000527 RID: 1319
		private static SourceValueInfo[] EmptyInfo = new SourceValueInfo[0];

		// Token: 0x04000528 RID: 1320
		private static string SpecialChars = "./[]";

		// Token: 0x02000095 RID: 149
		private enum State
		{
			// Token: 0x04000589 RID: 1417
			Init,
			// Token: 0x0400058A RID: 1418
			DrillIn,
			// Token: 0x0400058B RID: 1419
			Prop,
			// Token: 0x0400058C RID: 1420
			Done
		}

		// Token: 0x02000096 RID: 150
		private enum IndexerState
		{
			// Token: 0x0400058E RID: 1422
			BeginParam,
			// Token: 0x0400058F RID: 1423
			ParenString,
			// Token: 0x04000590 RID: 1424
			ValueString,
			// Token: 0x04000591 RID: 1425
			Done
		}
	}
}
