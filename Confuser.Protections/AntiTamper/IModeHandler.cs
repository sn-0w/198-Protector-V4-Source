using System;
using Confuser.Core;

namespace Confuser.Protections.AntiTamper
{
	// Token: 0x020000F7 RID: 247
	internal interface IModeHandler
	{
		// Token: 0x060003FF RID: 1023
		void HandleInject(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters);

		// Token: 0x06000400 RID: 1024
		void HandleMD(AntiTamperProtection parent, ConfuserContext context, ProtectionParameters parameters);
	}
}
