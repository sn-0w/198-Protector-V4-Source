using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002D RID: 45
	internal class SettingsTabVM : TabViewModel
	{
		// Token: 0x06000161 RID: 353 RVA: 0x00002BC0 File Offset: 0x00002BC0
		public SettingsTabVM(AppVM app) : base(app, "Settings")
		{
			app.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				bool flag = e.PropertyName == "Project";
				if (flag)
				{
					this.InitProject();
				}
			};
			this.InitProject();
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000162 RID: 354 RVA: 0x0000615C File Offset: 0x0000615C
		// (set) Token: 0x06000163 RID: 355 RVA: 0x00002BEA File Offset: 0x00002BEA
		public bool HasPacker
		{
			get
			{
				return this.hasPacker;
			}
			set
			{
				base.SetProperty<bool>(ref this.hasPacker, value, "HasPacker");
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00002C00 File Offset: 0x00002C00
		// (set) Token: 0x06000165 RID: 357 RVA: 0x00002C08 File Offset: 0x00002C08
		public IList ModulesView { get; private set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000166 RID: 358 RVA: 0x00006174 File Offset: 0x00006174
		// (set) Token: 0x06000167 RID: 359 RVA: 0x0000618C File Offset: 0x0000618C
		public IRuleContainer SelectedList
		{
			get
			{
				return this.selectedList;
			}
			set
			{
				bool flag = base.SetProperty<IRuleContainer>(ref this.selectedList, value, "SelectedList");
				if (flag)
				{
					this.SelectedRuleIndex = -1;
				}
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000168 RID: 360 RVA: 0x000061B8 File Offset: 0x000061B8
		// (set) Token: 0x06000169 RID: 361 RVA: 0x00002C11 File Offset: 0x00002C11
		public int SelectedRuleIndex
		{
			get
			{
				return this.selectedRuleIndex;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedRuleIndex, value, "SelectedRuleIndex");
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000061D0 File Offset: 0x000061D0
		public ICommand Add
		{
			get
			{
				RelayCommand cmd = new RelayCommand(delegate()
				{
					Debug.Assert(this.SelectedList != null);
					ProjectRuleVM projectRuleVM = new ProjectRuleVM(this.App.Project, new Rule("true", ProtectionPreset.None, false));
					projectRuleVM.Pattern = "true";
					this.SelectedList.Rules.Add(projectRuleVM);
					this.SelectedRuleIndex = this.SelectedList.Rules.Count - 1;
				}, () => this.SelectedList != null, false);
				base.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
				{
					bool flag = args.PropertyName == "SelectedList";
					if (flag)
					{
						cmd.RaiseCanExecuteChanged();
					}
				};
				return cmd;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600016B RID: 363 RVA: 0x0000622C File Offset: 0x0000622C
		public ICommand Remove
		{
			get
			{
				RelayCommand cmd = new RelayCommand(delegate()
				{
					int num = this.SelectedRuleIndex;
					Debug.Assert(this.SelectedList != null);
					Debug.Assert(num != -1);
					ProjectRuleVM projectRuleVM = this.SelectedList.Rules[num];
					this.SelectedList.Rules.RemoveAt(num);
					this.SelectedRuleIndex = ((num >= this.SelectedList.Rules.Count) ? (this.SelectedList.Rules.Count - 1) : num);
				}, () => this.SelectedRuleIndex != -1 && this.SelectedList != null, false);
				base.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
				{
					bool flag = args.PropertyName == "SelectedList" || args.PropertyName == "SelectedRuleIndex";
					if (flag)
					{
						cmd.RaiseCanExecuteChanged();
					}
				};
				return cmd;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00006288 File Offset: 0x00006288
		public ICommand Edit
		{
			get
			{
				RelayCommand cmd = new RelayCommand(delegate()
				{
					Debug.Assert(this.SelectedRuleIndex != -1);
					ProjectRuleView projectRuleView = new ProjectRuleView(this.App.Project, this.SelectedList.Rules[this.SelectedRuleIndex]);
					projectRuleView.Owner = Application.Current.MainWindow;
					projectRuleView.ShowDialog();
					projectRuleView.Cleanup();
				}, () => this.SelectedRuleIndex != -1 && this.SelectedList != null, false);
				base.PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
				{
					bool flag = args.PropertyName == "SelectedList" || args.PropertyName == "SelectedRuleIndex";
					if (flag)
					{
						cmd.RaiseCanExecuteChanged();
					}
				};
				return cmd;
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x000062E4 File Offset: 0x000062E4
		private void InitProject()
		{
			this.ModulesView = new CompositeCollection
			{
				base.App.Project,
				new CollectionContainer
				{
					Collection = base.App.Project.Modules
				}
			};
			this.OnPropertyChanged("ModulesView");
			this.HasPacker = (base.App.Project.Packer != null);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000635C File Offset: 0x0000635C
		protected override void OnPropertyChanged(string property)
		{
			bool flag = property == "HasPacker";
			if (flag)
			{
				bool flag2 = this.hasPacker && base.App.Project.Packer == null;
				if (flag2)
				{
					base.App.Project.Packer = new ProjectSettingVM<Packer>(base.App.Project, new SettingItem<Packer>(null, SettingItemAction.Add)
					{
						Id = base.App.Project.Packers[0].Id
					});
				}
				else
				{
					bool flag3 = !this.hasPacker;
					if (flag3)
					{
						base.App.Project.Packer = null;
					}
				}
			}
			base.OnPropertyChanged(property);
		}

		// Token: 0x04000084 RID: 132
		private bool hasPacker;

		// Token: 0x04000085 RID: 133
		private IRuleContainer selectedList;

		// Token: 0x04000086 RID: 134
		private int selectedRuleIndex;
	}
}
