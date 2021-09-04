using System;
using System.Diagnostics;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000023 RID: 35
	internal class BAMLPropertyReference : IBAMLReference
	{
		// Token: 0x060000FE RID: 254 RVA: 0x0000C5AB File Offset: 0x0000A7AB
		public BAMLPropertyReference(PropertyRecord rec)
		{
			this.rec = rec;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000C5BC File Offset: 0x0000A7BC
		public bool CanRename(string oldName, string newName)
		{
			return true;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000C5D0 File Offset: 0x0000A7D0
		public void Rename(string oldName, string newName)
		{
			string text = this.rec.Value;
			for (;;)
			{
				bool flag = text.EndsWith(oldName, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					break;
				}
				bool flag2 = oldName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);
				if (!flag2)
				{
					return;
				}
				oldName = BAMLPropertyReference.ToXaml(oldName);
				newName = BAMLPropertyReference.ToXaml(newName);
			}
			text = text.Substring(0, text.Length - oldName.Length) + newName;
			this.rec.Value = text;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000C650 File Offset: 0x0000A850
		private static string ToXaml(string refName)
		{
			Debug.Assert(refName.EndsWith(".baml"));
			return refName.Substring(0, refName.Length - 5) + ".xaml";
		}

		// Token: 0x0400007D RID: 125
		private PropertyRecord rec;
	}
}
