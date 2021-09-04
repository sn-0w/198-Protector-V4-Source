using System;
using System.Collections.Generic;
using Confuser.Core.Project.Patterns;

namespace Confuser.Core.Project
{
	// Token: 0x0200008A RID: 138
	public class PatternParser
	{
		// Token: 0x06000354 RID: 852 RVA: 0x00014AFC File Offset: 0x00012CFC
		static PatternParser()
		{
			PatternParser.fns.Add("module", () => new ModuleFunction());
			PatternParser.fns.Add("decl-type", () => new DeclTypeFunction());
			PatternParser.fns.Add("namespace", () => new NamespaceFunction());
			PatternParser.fns.Add("name", () => new NameFunction());
			PatternParser.fns.Add("full-name", () => new FullNameFunction());
			PatternParser.fns.Add("match", () => new MatchFunction());
			PatternParser.fns.Add("match-name", () => new MatchNameFunction());
			PatternParser.fns.Add("match-type-name", () => new MatchTypeNameFunction());
			PatternParser.fns.Add("member-type", () => new MemberTypeFunction());
			PatternParser.fns.Add("is-public", () => new IsPublicFunction());
			PatternParser.fns.Add("inherits", () => new InheritsFunction());
			PatternParser.fns.Add("is-type", () => new IsTypeFunction());
			PatternParser.fns.Add("has-attr", () => new HasAttrFunction());
			PatternParser.ops = new Dictionary<string, Func<PatternOperator>>(StringComparer.OrdinalIgnoreCase);
			PatternParser.ops.Add("and", () => new AndOperator());
			PatternParser.ops.Add("or", () => new OrOperator());
			PatternParser.ops.Add("not", () => new NotOperator());
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00014D28 File Offset: 0x00012F28
		public PatternExpression Parse(string pattern)
		{
			bool flag = pattern == null;
			if (flag)
			{
				throw new ArgumentNullException("pattern");
			}
			PatternExpression result;
			try
			{
				this.tokenizer.Initialize(pattern);
				this.lookAhead = this.tokenizer.NextToken();
				PatternExpression patternExpression = this.ParseExpression(true);
				bool flag2 = this.PeekToken() != null;
				if (flag2)
				{
					throw new InvalidPatternException("Extra tokens beyond the end of pattern.");
				}
				result = patternExpression;
			}
			catch (Exception ex)
			{
				bool flag3 = ex is InvalidPatternException;
				if (flag3)
				{
					throw;
				}
				throw new InvalidPatternException("Invalid pattern.", ex);
			}
			return result;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00014DC8 File Offset: 0x00012FC8
		private static bool IsFunction(PatternToken token)
		{
			bool flag = token.Type > TokenType.Identifier;
			return !flag && PatternParser.fns.ContainsKey(token.Value);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x00014DFC File Offset: 0x00012FFC
		private static bool IsOperator(PatternToken token)
		{
			bool flag = token.Type > TokenType.Identifier;
			return !flag && PatternParser.ops.ContainsKey(token.Value);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00003727 File Offset: 0x00001927
		private Exception UnexpectedEnd()
		{
			throw new InvalidPatternException("Unexpected end of pattern.");
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00003734 File Offset: 0x00001934
		private Exception MismatchParens(int position)
		{
			throw new InvalidPatternException(string.Format("Mismatched parentheses at position {0}.", position));
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0000374C File Offset: 0x0000194C
		private Exception UnknownToken(PatternToken token)
		{
			throw new InvalidPatternException(string.Format("Unknown token '{0}' at position {1}.", token.Value, token.Position));
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0000376F File Offset: 0x0000196F
		private Exception UnexpectedToken(PatternToken token)
		{
			throw new InvalidPatternException(string.Format("Unexpected token '{0}' at position {1}.", token.Value, token.Position));
		}

		// Token: 0x0600035C RID: 860 RVA: 0x00003792 File Offset: 0x00001992
		private Exception UnexpectedToken(PatternToken token, char expect)
		{
			throw new InvalidPatternException(string.Format("Unexpected token '{0}' at position {1}. Expected '{2}'.", token.Value, token.Position, expect));
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000037BB File Offset: 0x000019BB
		private Exception BadArgCount(PatternToken token, int expected)
		{
			throw new InvalidPatternException(string.Format("Invalid argument count for '{0}' at position {1}. Expected {2}", token.Value, token.Position, expected));
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00014E30 File Offset: 0x00013030
		private PatternToken ReadToken()
		{
			bool flag = this.lookAhead == null;
			if (flag)
			{
				throw this.UnexpectedEnd();
			}
			PatternToken value = this.lookAhead.Value;
			this.lookAhead = this.tokenizer.NextToken();
			return value;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00014E7C File Offset: 0x0001307C
		private PatternToken? PeekToken()
		{
			return this.lookAhead;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x00014E94 File Offset: 0x00013094
		private PatternExpression ParseExpression(bool readBinOp = false)
		{
			PatternToken patternToken = this.ReadToken();
			PatternExpression patternExpression;
			switch (patternToken.Type)
			{
			case TokenType.Identifier:
			{
				bool flag = PatternParser.IsOperator(patternToken);
				if (flag)
				{
					PatternOperator patternOperator = PatternParser.ops[patternToken.Value]();
					bool flag2 = !patternOperator.IsUnary;
					if (flag2)
					{
						throw this.UnexpectedToken(patternToken);
					}
					patternOperator.OperandA = this.ParseExpression(false);
					patternExpression = patternOperator;
				}
				else
				{
					bool flag3 = PatternParser.IsFunction(patternToken);
					if (flag3)
					{
						PatternFunction patternFunction = PatternParser.fns[patternToken.Value]();
						PatternToken patternToken2 = this.ReadToken();
						bool flag4 = patternToken2.Type != TokenType.LParens;
						if (flag4)
						{
							throw this.UnexpectedToken(patternToken2, '(');
						}
						patternFunction.Arguments = new List<PatternExpression>(patternFunction.ArgumentCount);
						for (int i = 0; i < patternFunction.ArgumentCount; i++)
						{
							bool flag5 = this.PeekToken() == null;
							if (flag5)
							{
								throw this.UnexpectedEnd();
							}
							bool flag6 = this.PeekToken().Value.Type == TokenType.RParens;
							if (flag6)
							{
								throw this.BadArgCount(patternToken, patternFunction.ArgumentCount);
							}
							bool flag7 = i != 0;
							if (flag7)
							{
								PatternToken patternToken3 = this.ReadToken();
								bool flag8 = patternToken3.Type != TokenType.Comma;
								if (flag8)
								{
									throw this.UnexpectedToken(patternToken3, ',');
								}
							}
							patternFunction.Arguments.Add(this.ParseExpression(false));
						}
						patternToken2 = this.ReadToken();
						bool flag9 = patternToken2.Type == TokenType.Comma;
						if (flag9)
						{
							throw this.BadArgCount(patternToken, patternFunction.ArgumentCount);
						}
						bool flag10 = patternToken2.Type != TokenType.RParens;
						if (flag10)
						{
							throw this.MismatchParens(patternToken2.Position.Value);
						}
						patternExpression = patternFunction;
					}
					else
					{
						bool flag12;
						bool flag11 = bool.TryParse(patternToken.Value, out flag12);
						if (!flag11)
						{
							throw this.UnknownToken(patternToken);
						}
						patternExpression = new LiteralExpression(flag12);
					}
				}
				break;
			}
			case TokenType.Literal:
				patternExpression = new LiteralExpression(patternToken.Value);
				break;
			case TokenType.LParens:
			{
				patternExpression = this.ParseExpression(true);
				PatternToken patternToken4 = this.ReadToken();
				bool flag13 = patternToken4.Type != TokenType.RParens;
				if (flag13)
				{
					throw this.MismatchParens(patternToken.Position.Value);
				}
				break;
			}
			default:
				throw this.UnexpectedToken(patternToken);
			}
			bool flag14 = !readBinOp;
			PatternExpression result;
			if (flag14)
			{
				result = patternExpression;
			}
			else
			{
				for (PatternToken? patternToken5 = this.PeekToken(); patternToken5 != null; patternToken5 = this.PeekToken())
				{
					bool flag15 = patternToken5.Value.Type > TokenType.Identifier;
					if (flag15)
					{
						break;
					}
					bool flag16 = !PatternParser.IsOperator(patternToken5.Value);
					if (flag16)
					{
						break;
					}
					PatternToken patternToken6 = this.ReadToken();
					PatternOperator patternOperator2 = PatternParser.ops[patternToken6.Value]();
					bool isUnary = patternOperator2.IsUnary;
					if (isUnary)
					{
						throw this.UnexpectedToken(patternToken6);
					}
					patternOperator2.OperandA = patternExpression;
					patternOperator2.OperandB = this.ParseExpression(false);
					patternExpression = patternOperator2;
				}
				result = patternExpression;
			}
			return result;
		}

		// Token: 0x04000257 RID: 599
		private static readonly Dictionary<string, Func<PatternFunction>> fns = new Dictionary<string, Func<PatternFunction>>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000258 RID: 600
		private static readonly Dictionary<string, Func<PatternOperator>> ops;

		// Token: 0x04000259 RID: 601
		private readonly PatternTokenizer tokenizer = new PatternTokenizer();

		// Token: 0x0400025A RID: 602
		private PatternToken? lookAhead;
	}
}
