using System;
using System.Linq;

namespace Confuser.Renamer.BAML
{
	// Token: 0x02000066 RID: 102
	internal class PropertyPathPart
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0002443B File Offset: 0x0002263B
		public PropertyPathPart(bool isIndexer, bool? isHiera, string name)
		{
			this.IsIndexer = isIndexer;
			this.IsHierarchical = isHiera;
			this.Name = name;
			this.IndexerArguments = null;
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000289 RID: 649 RVA: 0x00024465 File Offset: 0x00022665
		// (set) Token: 0x0600028A RID: 650 RVA: 0x0002446D File Offset: 0x0002266D
		public bool IsIndexer { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600028B RID: 651 RVA: 0x00024476 File Offset: 0x00022676
		// (set) Token: 0x0600028C RID: 652 RVA: 0x0002447E File Offset: 0x0002267E
		public bool? IsHierarchical { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x0600028D RID: 653 RVA: 0x00024487 File Offset: 0x00022687
		// (set) Token: 0x0600028E RID: 654 RVA: 0x0002448F File Offset: 0x0002268F
		public string Name { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x0600028F RID: 655 RVA: 0x00024498 File Offset: 0x00022698
		// (set) Token: 0x06000290 RID: 656 RVA: 0x000244A0 File Offset: 0x000226A0
		public PropertyPathIndexer[] IndexerArguments { get; set; }

		// Token: 0x06000291 RID: 657 RVA: 0x000244AC File Offset: 0x000226AC
		public bool IsAttachedDP()
		{
			return !this.IsIndexer && this.Name.Length >= 2 && this.Name[0] == '(' && this.Name[this.Name.Length - 1] == ')';
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00024504 File Offset: 0x00022704
		public void ExtractAttachedDP(out string type, out string property)
		{
			string text = this.Name.Substring(1, this.Name.Length - 2);
			bool flag = !text.Contains('.');
			if (flag)
			{
				type = null;
				property = text.Trim();
			}
			else
			{
				int num = text.LastIndexOf('.');
				type = text.Substring(0, num).Trim();
				property = text.Substring(num + 1).Trim();
			}
		}
	}
}
