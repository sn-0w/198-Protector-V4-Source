using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Confuser.Core
{
	// Token: 0x02000059 RID: 89
	internal struct ObfAttrParser
	{
		// Token: 0x06000225 RID: 549 RVA: 0x00002E70 File Offset: 0x00001070
		public ObfAttrParser(IDictionary items)
		{
			this.items = items;
			this.str = null;
			this.index = -1;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00010580 File Offset: 0x0000E780
		private bool ReadId(StringBuilder sb)
		{
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				char c2 = c;
				switch (c2)
				{
				case '(':
				case ')':
				case '+':
				case ',':
				case '-':
					goto IL_49;
				case '*':
					break;
				default:
					if (c2 == ';' || c2 == '=')
					{
						goto IL_49;
					}
					break;
				}
				string text = this.str;
				int num = this.index;
				this.index = num + 1;
				sb.Append(text[num]);
				continue;
				IL_49:
				return true;
			}
			return false;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00010624 File Offset: 0x0000E824
		private bool ReadString(StringBuilder sb)
		{
			this.Expect('\'');
			while (this.index < this.str.Length)
			{
				char c = this.str[this.index];
				char c2 = c;
				if (c2 == '\'')
				{
					this.index++;
					return true;
				}
				if (c2 != '\\')
				{
					sb.Append(this.str[this.index]);
				}
				else
				{
					string text = this.str;
					int num = this.index + 1;
					this.index = num;
					sb.Append(text[num]);
				}
				this.index++;
			}
			return false;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x000106E4 File Offset: 0x0000E8E4
		private void Expect(char chr)
		{
			bool flag = this.str[this.index] != chr;
			if (flag)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Expect '",
					chr.ToString(),
					"' at position ",
					this.index + 1,
					"."
				}));
			}
			this.index++;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00010760 File Offset: 0x0000E960
		private char Peek()
		{
			return this.str[this.index];
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00002E88 File Offset: 0x00001088
		private void Next()
		{
			this.index++;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00010784 File Offset: 0x0000E984
		private bool IsEnd()
		{
			return this.index == this.str.Length;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000107AC File Offset: 0x0000E9AC
		public void ParseProtectionString(IDictionary<ConfuserComponent, Dictionary<string, string>> settings, string str)
		{
			bool flag = str == null;
			if (!flag)
			{
				this.str = str;
				this.index = 0;
				ObfAttrParser.ParseState parseState = ObfAttrParser.ParseState.Init;
				StringBuilder stringBuilder = new StringBuilder();
				bool flag2 = true;
				string text = null;
				Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				while (parseState != ObfAttrParser.ParseState.End)
				{
					switch (parseState)
					{
					case ObfAttrParser.ParseState.Init:
					{
						this.ReadId(stringBuilder);
						bool flag3 = stringBuilder.ToString().Equals("preset", StringComparison.OrdinalIgnoreCase);
						if (flag3)
						{
							bool flag4 = this.IsEnd();
							if (flag4)
							{
								throw new ArgumentException("Unexpected end of string in Init state.");
							}
							this.Expect('(');
							stringBuilder.Length = 0;
							parseState = ObfAttrParser.ParseState.ReadPreset;
						}
						else
						{
							bool flag5 = stringBuilder.Length == 0;
							if (flag5)
							{
								bool flag6 = this.IsEnd();
								if (flag6)
								{
									throw new ArgumentException("Unexpected end of string in Init state.");
								}
								parseState = ObfAttrParser.ParseState.ReadItemName;
							}
							else
							{
								flag2 = true;
								parseState = ObfAttrParser.ParseState.ProcessItemName;
							}
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadPreset:
					{
						bool flag7 = !this.ReadId(stringBuilder);
						if (flag7)
						{
							throw new ArgumentException("Unexpected end of string in ReadPreset state.");
						}
						this.Expect(')');
						ProtectionPreset preset = (ProtectionPreset)Enum.Parse(typeof(ProtectionPreset), stringBuilder.ToString(), true);
						IEnumerable<Protection> source = this.items.Values.OfType<Protection>();
						Func<Protection, bool> predicate;
						Func<Protection, bool> <>9__0;
						if ((predicate = <>9__0) == null)
						{
							predicate = (<>9__0 = ((Protection prot) => prot.Preset <= preset));
						}
						foreach (Protection protection in source.Where(predicate))
						{
							bool flag8 = protection.Preset != ProtectionPreset.None && settings != null && !settings.ContainsKey(protection);
							if (flag8)
							{
								settings.Add(protection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
							}
						}
						stringBuilder.Length = 0;
						bool flag9 = this.IsEnd();
						if (flag9)
						{
							parseState = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag10 = this.IsEnd();
							if (flag10)
							{
								parseState = ObfAttrParser.ParseState.End;
							}
							else
							{
								parseState = ObfAttrParser.ParseState.ReadItemName;
							}
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadItemName:
					{
						flag2 = true;
						bool flag11 = this.Peek() == '+';
						if (flag11)
						{
							flag2 = true;
							this.Next();
						}
						else
						{
							bool flag12 = this.Peek() == '-';
							if (flag12)
							{
								flag2 = false;
								this.Next();
							}
						}
						this.ReadId(stringBuilder);
						parseState = ObfAttrParser.ParseState.ProcessItemName;
						break;
					}
					case ObfAttrParser.ParseState.ProcessItemName:
					{
						text = stringBuilder.ToString();
						stringBuilder.Length = 0;
						bool flag13 = this.IsEnd() || this.Peek() == ';';
						if (flag13)
						{
							parseState = ObfAttrParser.ParseState.EndItem;
						}
						else
						{
							bool flag14 = this.Peek() == '(';
							if (!flag14)
							{
								throw new ArgumentException("Unexpected character in ProcessItemName state at " + this.index + ".");
							}
							bool flag15 = !flag2;
							if (flag15)
							{
								throw new ArgumentException("No parameters is allowed when removing protection.");
							}
							this.Next();
							parseState = ObfAttrParser.ParseState.ReadParam;
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadParam:
					{
						bool flag16 = !this.ReadId(stringBuilder);
						if (flag16)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string key = stringBuilder.ToString();
						stringBuilder.Length = 0;
						this.Expect('=');
						bool flag17 = !((this.Peek() == '\'') ? this.ReadString(stringBuilder) : this.ReadId(stringBuilder));
						if (flag17)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string value = stringBuilder.ToString();
						stringBuilder.Length = 0;
						dictionary.Add(key, value);
						bool flag18 = this.Peek() == ',';
						if (flag18)
						{
							this.Next();
							parseState = ObfAttrParser.ParseState.ReadParam;
						}
						else
						{
							bool flag19 = this.Peek() == ')';
							if (!flag19)
							{
								throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
							}
							this.Next();
							parseState = ObfAttrParser.ParseState.EndItem;
						}
						break;
					}
					case ObfAttrParser.ParseState.EndItem:
					{
						bool flag20 = settings != null;
						if (flag20)
						{
							bool flag21 = !this.items.Contains(text);
							if (flag21)
							{
								throw new KeyNotFoundException("Cannot find protection with id '" + text + "'.");
							}
							bool flag22 = flag2;
							if (flag22)
							{
								bool flag23 = settings.ContainsKey((Protection)this.items[text]);
								if (flag23)
								{
									Dictionary<string, string> dictionary2 = settings[(Protection)this.items[text]];
									foreach (KeyValuePair<string, string> keyValuePair in dictionary)
									{
										dictionary2[keyValuePair.Key] = keyValuePair.Value;
									}
								}
								else
								{
									settings[(Protection)this.items[text]] = dictionary;
								}
							}
							else
							{
								settings.Remove((Protection)this.items[text]);
							}
						}
						dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
						bool flag24 = this.IsEnd();
						if (flag24)
						{
							parseState = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag25 = this.IsEnd();
							if (flag25)
							{
								parseState = ObfAttrParser.ParseState.End;
							}
							else
							{
								parseState = ObfAttrParser.ParseState.ReadItemName;
							}
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00010CD0 File Offset: 0x0000EED0
		public void ParsePackerString(string str, out Packer packer, out Dictionary<string, string> packerParams)
		{
			packer = null;
			packerParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			bool flag = str == null;
			if (!flag)
			{
				this.str = str;
				this.index = 0;
				ObfAttrParser.ParseState parseState = ObfAttrParser.ParseState.ReadItemName;
				StringBuilder stringBuilder = new StringBuilder();
				ProtectionSettings protectionSettings = new ProtectionSettings();
				while (parseState != ObfAttrParser.ParseState.End)
				{
					switch (parseState)
					{
					case ObfAttrParser.ParseState.ReadItemName:
					{
						this.ReadId(stringBuilder);
						string text = stringBuilder.ToString();
						bool flag2 = !this.items.Contains(text);
						if (flag2)
						{
							throw new KeyNotFoundException("Cannot find packer with id '" + text + "'.");
						}
						packer = (Packer)this.items[text];
						stringBuilder.Length = 0;
						bool flag3 = this.IsEnd() || this.Peek() == ';';
						if (flag3)
						{
							parseState = ObfAttrParser.ParseState.EndItem;
						}
						else
						{
							bool flag4 = this.Peek() == '(';
							if (!flag4)
							{
								throw new ArgumentException("Unexpected character in ReadItemName state at " + this.index + ".");
							}
							this.Next();
							parseState = ObfAttrParser.ParseState.ReadParam;
						}
						break;
					}
					case ObfAttrParser.ParseState.ReadParam:
					{
						bool flag5 = !this.ReadId(stringBuilder);
						if (flag5)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string key = stringBuilder.ToString();
						stringBuilder.Length = 0;
						this.Expect('=');
						bool flag6 = !this.ReadId(stringBuilder);
						if (flag6)
						{
							throw new ArgumentException("Unexpected end of string in ReadParam state.");
						}
						string value = stringBuilder.ToString();
						stringBuilder.Length = 0;
						packerParams.Add(key, value);
						bool flag7 = this.Peek() == ',';
						if (flag7)
						{
							this.Next();
							parseState = ObfAttrParser.ParseState.ReadParam;
						}
						else
						{
							bool flag8 = this.Peek() == ')';
							if (!flag8)
							{
								throw new ArgumentException("Unexpected character in ReadParam state at " + this.index + ".");
							}
							this.Next();
							parseState = ObfAttrParser.ParseState.EndItem;
						}
						break;
					}
					case ObfAttrParser.ParseState.EndItem:
					{
						bool flag9 = this.IsEnd();
						if (flag9)
						{
							parseState = ObfAttrParser.ParseState.End;
						}
						else
						{
							this.Expect(';');
							bool flag10 = !this.IsEnd();
							if (flag10)
							{
								throw new ArgumentException("Unexpected character in EndItem state at " + this.index + ".");
							}
							parseState = ObfAttrParser.ParseState.End;
						}
						break;
					}
					}
				}
			}
		}

		// Token: 0x040001C0 RID: 448
		private readonly IDictionary items;

		// Token: 0x040001C1 RID: 449
		private string str;

		// Token: 0x040001C2 RID: 450
		private int index;

		// Token: 0x0200005A RID: 90
		private enum ParseState
		{
			// Token: 0x040001C4 RID: 452
			Init,
			// Token: 0x040001C5 RID: 453
			ReadPreset,
			// Token: 0x040001C6 RID: 454
			ReadItemName,
			// Token: 0x040001C7 RID: 455
			ProcessItemName,
			// Token: 0x040001C8 RID: 456
			ReadParam,
			// Token: 0x040001C9 RID: 457
			EndItem,
			// Token: 0x040001CA RID: 458
			End
		}
	}
}
