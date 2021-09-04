using System;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001D RID: 29
	public class ProjectSettingVM<T> : ViewModelBase, IViewModel<SettingItem<T>>
	{
		// Token: 0x060000A7 RID: 167 RVA: 0x000024C8 File Offset: 0x000024C8
		public ProjectSettingVM(ProjectVM parent, SettingItem<T> setting)
		{
			this.parent = parent;
			this.setting = setting;
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00004B90 File Offset: 0x00004B90
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00004BB0 File Offset: 0x00004BB0
		public string Id
		{
			get
			{
				return this.setting.Id;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.setting.Id != value, delegate(string val)
				{
					this.setting.Id = val;
				}, value, "Id");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00004BF8 File Offset: 0x00004BF8
		// (set) Token: 0x060000AB RID: 171 RVA: 0x00004C18 File Offset: 0x00004C18
		public SettingItemAction Action
		{
			get
			{
				return this.setting.Action;
			}
			set
			{
				bool flag = base.SetProperty<SettingItemAction>(this.setting.Action != value, delegate(SettingItemAction val)
				{
					this.setting.Action = val;
				}, value, "Action");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000AC RID: 172 RVA: 0x00004C60 File Offset: 0x00004C60
		SettingItem<T> IViewModel<SettingItem<!0>>.Model
		{
			get
			{
				return this.setting;
			}
		}

		// Token: 0x0400004C RID: 76
		private readonly ProjectVM parent;

		// Token: 0x0400004D RID: 77
		private readonly SettingItem<T> setting;
	}
}
