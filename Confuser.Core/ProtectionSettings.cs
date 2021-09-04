using System;
using System.Collections.Generic;

namespace Confuser.Core
{
	// Token: 0x02000066 RID: 102
	public class ProtectionSettings : Dictionary<ConfuserComponent, Dictionary<string, string>>
	{
		// Token: 0x0600026B RID: 619 RVA: 0x00003073 File Offset: 0x00001273
		public ProtectionSettings()
		{
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00011694 File Offset: 0x0000F894
		public ProtectionSettings(ProtectionSettings settings)
		{
			bool flag = settings == null;
			if (!flag)
			{
				foreach (KeyValuePair<ConfuserComponent, Dictionary<string, string>> keyValuePair in settings)
				{
					base.Add(keyValuePair.Key, new Dictionary<string, string>(keyValuePair.Value));
				}
			}
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0001170C File Offset: 0x0000F90C
		public bool IsEmpty()
		{
			return base.Count == 0;
		}
	}
}
