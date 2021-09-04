using System;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001E RID: 30
	public class StringItem : IViewModel<string>
	{
		// Token: 0x060000AF RID: 175 RVA: 0x000024FE File Offset: 0x000024FE
		public StringItem(string item)
		{
			this.Item = item;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00002510 File Offset: 0x00002510
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x00002518 File Offset: 0x00002518
		public string Item { get; private set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00004C78 File Offset: 0x00004C78
		string IViewModel<string>.Model
		{
			get
			{
				return this.Item;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00004C78 File Offset: 0x00004C78
		public override string ToString()
		{
			return this.Item;
		}
	}
}
