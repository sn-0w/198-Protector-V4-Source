using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Confuser.Core.Project
{
	// Token: 0x02000086 RID: 134
	public class ProjectValidationException : Exception
	{
		// Token: 0x06000336 RID: 822 RVA: 0x000035F4 File Offset: 0x000017F4
		internal ProjectValidationException(List<XmlSchemaException> exceptions) : base(exceptions[0].Message)
		{
			this.Errors = exceptions;
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000337 RID: 823 RVA: 0x00003612 File Offset: 0x00001812
		// (set) Token: 0x06000338 RID: 824 RVA: 0x0000361A File Offset: 0x0000181A
		public IList<XmlSchemaException> Errors { get; private set; }
	}
}
