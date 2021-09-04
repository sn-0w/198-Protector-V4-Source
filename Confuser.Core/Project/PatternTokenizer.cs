using System;
using System.Diagnostics;
using System.Text;

namespace Confuser.Core.Project
{
	// Token: 0x0200008E RID: 142
	internal class PatternTokenizer
	{
		// Token: 0x06000379 RID: 889 RVA: 0x000038E8 File Offset: 0x00001AE8
		public void Initialize(string pattern)
		{
			this.rulePattern = pattern;
			this.index = 0;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0001528C File Offset: 0x0001348C
		private void SkipWhitespace()
		{
			while (this.index < this.rulePattern.Length && char.IsWhiteSpace(this.rulePattern[this.index]))
			{
				this.index++;
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000152DC File Offset: 0x000134DC
		private char? PeekChar()
		{
			bool flag = this.index >= this.rulePattern.Length;
			char? result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = new char?(this.rulePattern[this.index]);
			}
			return result;
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0001532C File Offset: 0x0001352C
		private char NextChar()
		{
			bool flag = this.index >= this.rulePattern.Length;
			if (flag)
			{
				throw new InvalidPatternException("Unexpected end of pattern.");
			}
			string text = this.rulePattern;
			int num = this.index;
			this.index = num + 1;
			return text[num];
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00015380 File Offset: 0x00013580
		private string ReadLiteral()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char c = this.NextChar();
			Debug.Assert(c == '"' || c == '\'');
			for (char c2 = this.NextChar(); c2 != c; c2 = this.NextChar())
			{
				bool flag = c2 == '\\';
				if (flag)
				{
					stringBuilder.Append(this.NextChar());
				}
				else
				{
					stringBuilder.Append(c2);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000153FC File Offset: 0x000135FC
		private string ReadIdentifier()
		{
			StringBuilder stringBuilder = new StringBuilder();
			char? c = this.PeekChar();
			for (;;)
			{
				bool flag;
				if (c != null)
				{
					if (char.IsLetterOrDigit(c.Value))
					{
						goto IL_BC;
					}
					char? c2 = c;
					int? num = (c2 != null) ? new int?((int)c2.GetValueOrDefault()) : null;
					int num2 = 95;
					if (num.GetValueOrDefault() == num2 & num != null)
					{
						goto IL_BC;
					}
					c2 = c;
					num = ((c2 != null) ? new int?((int)c2.GetValueOrDefault()) : null);
					num2 = 45;
					flag = (num.GetValueOrDefault() == num2 & num != null);
					goto IL_C0;
					IL_BC:
					flag = true;
				}
				else
				{
					flag = false;
				}
				IL_C0:
				if (!flag)
				{
					break;
				}
				stringBuilder.Append(this.NextChar());
				c = this.PeekChar();
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600037F RID: 895 RVA: 0x000154DC File Offset: 0x000136DC
		public PatternToken? NextToken()
		{
			bool flag = this.rulePattern == null;
			if (flag)
			{
				throw new InvalidOperationException("Tokenizer not initialized.");
			}
			this.SkipWhitespace();
			char? c = this.PeekChar();
			bool flag2 = c == null;
			PatternToken? result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				int num = this.index;
				char value = c.Value;
				char c2 = value;
				if (c2 != '"')
				{
					switch (c2)
					{
					case '\'':
						goto IL_E6;
					case '(':
						this.index++;
						return new PatternToken?(new PatternToken(num, TokenType.LParens));
					case ')':
						this.index++;
						return new PatternToken?(new PatternToken(num, TokenType.RParens));
					case ',':
						this.index++;
						return new PatternToken?(new PatternToken(num, TokenType.Comma));
					}
					bool flag3 = !char.IsLetter(c.Value);
					if (flag3)
					{
						throw new InvalidPatternException(string.Format("Unknown token '{0}' at position {1}.", c, num));
					}
					return new PatternToken?(new PatternToken(num, TokenType.Identifier, this.ReadIdentifier()));
				}
				IL_E6:
				result = new PatternToken?(new PatternToken(num, TokenType.Literal, this.ReadLiteral()));
			}
			return result;
		}

		// Token: 0x04000265 RID: 613
		private int index;

		// Token: 0x04000266 RID: 614
		private string rulePattern;
	}
}
