using System;

namespace Confuser.Renamer.BAML
{
	// Token: 0x0200005F RID: 95
	internal interface IBAMLReference
	{
		// Token: 0x0600026A RID: 618
		bool CanRename(string oldName, string newName);

		// Token: 0x0600026B RID: 619
		void Rename(string oldName, string newName);
	}
}
