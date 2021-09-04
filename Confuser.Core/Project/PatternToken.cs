using System;

namespace Confuser.Core.Project
{
	// Token: 0x0200008D RID: 141
	public struct PatternToken
	{
		// Token: 0x06000374 RID: 884 RVA: 0x00003874 File Offset: 0x00001A74
		public PatternToken(int pos, TokenType type)
		{
			this.Position = new int?(pos);
			this.Type = type;
			this.Value = null;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00003891 File Offset: 0x00001A91
		public PatternToken(int pos, TokenType type, string value)
		{
			this.Position = new int?(pos);
			this.Type = type;
			this.Value = value;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x000038AE File Offset: 0x00001AAE
		public PatternToken(TokenType type)
		{
			this.Position = null;
			this.Type = type;
			this.Value = null;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000038CB File Offset: 0x00001ACB
		public PatternToken(TokenType type, string value)
		{
			this.Position = null;
			this.Type = type;
			this.Value = value;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000151D0 File Offset: 0x000133D0
		public override string ToString()
		{
			bool flag = this.Position != null;
			string result;
			if (flag)
			{
				bool flag2 = this.Value != null;
				if (flag2)
				{
					result = string.Format("[{0}] {1} @ {2}", this.Type, this.Value, this.Position);
				}
				else
				{
					result = string.Format("[{0}] @ {1}", this.Type, this.Position);
				}
			}
			else
			{
				bool flag3 = this.Value != null;
				if (flag3)
				{
					result = string.Format("[{0}] {1}", this.Type, this.Value);
				}
				else
				{
					result = string.Format("[{0}]", this.Type);
				}
			}
			return result;
		}

		// Token: 0x04000262 RID: 610
		public readonly int? Position;

		// Token: 0x04000263 RID: 611
		public readonly TokenType Type;

		// Token: 0x04000264 RID: 612
		public readonly string Value;
	}
}
