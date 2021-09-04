using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using Confuser.Core;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000029 RID: 41
	public class ProjectVM : ViewModelBase, IViewModel<ConfuserProject>, IRuleContainer
	{
		// Token: 0x06000115 RID: 277 RVA: 0x00005B1C File Offset: 0x00005B1C
		public ProjectVM(ConfuserProject proj, string fileName)
		{
			this.proj = proj;
			this.FileName = fileName;
			ObservableCollection<ProjectModuleVM> observableCollection = Utils.Wrap<ProjectModule, ProjectModuleVM>(proj, (ProjectModule module) => new ProjectModuleVM(this, module));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Modules = observableCollection;
			ObservableCollection<StringItem> observableCollection2 = Utils.Wrap<string, StringItem>(proj.PluginPaths, (string path) => new StringItem(path));
			observableCollection2.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Plugins = observableCollection2;
			ObservableCollection<StringItem> observableCollection3 = Utils.Wrap<string, StringItem>(proj.ProbePaths, (string path) => new StringItem(path));
			observableCollection3.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.ProbePaths = observableCollection3;
			ObservableCollection<ProjectRuleVM> observableCollection4 = Utils.Wrap<Rule, ProjectRuleVM>(proj.Rules, (Rule rule) => new ProjectRuleVM(this, rule));
			observableCollection4.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				this.IsModified = true;
			};
			this.Rules = observableCollection4;
			this.Protections = new ObservableCollection<ConfuserComponent>();
			this.Packers = new ObservableCollection<ConfuserComponent>();
			ComponentDiscovery.LoadComponents(this.Protections, this.Packers, Assembly.Load("Confuser.Protections").Location);
			ComponentDiscovery.LoadComponents(this.Protections, this.Packers, Assembly.Load("Confuser.Renamer").Location);
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000116 RID: 278 RVA: 0x00005C88 File Offset: 0x00005C88
		public ConfuserProject Project
		{
			get
			{
				return this.proj;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00005CA0 File Offset: 0x00005CA0
		// (set) Token: 0x06000118 RID: 280 RVA: 0x000027B4 File Offset: 0x000027B4
		public bool IsModified
		{
			get
			{
				return this.modified;
			}
			set
			{
				base.SetProperty<bool>(ref this.modified, value, "IsModified");
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00005CB8 File Offset: 0x00005CB8
		// (set) Token: 0x0600011A RID: 282 RVA: 0x000027CA File Offset: 0x000027CA
		public string Seed
		{
			get
			{
				return this.proj.Seed;
			}
			set
			{
				base.SetProperty<string>(this.proj.Seed != value, delegate(string val)
				{
					this.proj.Seed = val;
				}, value, "Seed");
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x0600011B RID: 283 RVA: 0x00005CD8 File Offset: 0x00005CD8
		// (set) Token: 0x0600011C RID: 284 RVA: 0x000027F7 File Offset: 0x000027F7
		public bool Debug
		{
			get
			{
				return this.proj.Debug;
			}
			set
			{
				base.SetProperty<bool>(this.proj.Debug != value, delegate(bool val)
				{
					this.proj.Debug = val;
				}, value, "Debug");
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00005CF8 File Offset: 0x00005CF8
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00002824 File Offset: 0x00002824
		public string BaseDirectory
		{
			get
			{
				return this.proj.BaseDirectory;
			}
			set
			{
				base.SetProperty<string>(this.proj.BaseDirectory != value, delegate(string val)
				{
					this.proj.BaseDirectory = val;
				}, value, "BaseDirectory");
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00005D18 File Offset: 0x00005D18
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00002851 File Offset: 0x00002851
		public string OutputDirectory
		{
			get
			{
				return this.proj.OutputDirectory;
			}
			set
			{
				base.SetProperty<string>(this.proj.OutputDirectory != value, delegate(string val)
				{
					this.proj.OutputDirectory = val;
				}, value, "OutputDirectory");
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00005D38 File Offset: 0x00005D38
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00005D84 File Offset: 0x00005D84
		public ProjectSettingVM<Packer> Packer
		{
			get
			{
				bool flag = this.proj.Packer == null;
				if (flag)
				{
					this.packer = null;
				}
				else
				{
					this.packer = new ProjectSettingVM<Packer>(this, this.proj.Packer);
				}
				return this.packer;
			}
			set
			{
				bool changed = (value == null && this.proj.Packer != null) || (value != null && this.proj.Packer != ((IViewModel<SettingItem<Packer>>)value).Model);
				base.SetProperty<IViewModel<SettingItem<Packer>>>(changed, delegate(IViewModel<SettingItem<Packer>> val)
				{
					this.proj.Packer = ((val == null) ? null : val.Model);
				}, value, "Packer");
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000287E File Offset: 0x0000287E
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00002886 File Offset: 0x00002886
		public IList<ProjectModuleVM> Modules { get; private set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000288F File Offset: 0x0000288F
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00002897 File Offset: 0x00002897
		public IList<StringItem> Plugins { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000028A0 File Offset: 0x000028A0
		// (set) Token: 0x06000128 RID: 296 RVA: 0x000028A8 File Offset: 0x000028A8
		public IList<StringItem> ProbePaths { get; private set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000028B1 File Offset: 0x000028B1
		// (set) Token: 0x0600012A RID: 298 RVA: 0x000028B9 File Offset: 0x000028B9
		public ObservableCollection<ConfuserComponent> Protections { get; private set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600012B RID: 299 RVA: 0x000028C2 File Offset: 0x000028C2
		// (set) Token: 0x0600012C RID: 300 RVA: 0x000028CA File Offset: 0x000028CA
		public ObservableCollection<ConfuserComponent> Packers { get; private set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600012D RID: 301 RVA: 0x000028D3 File Offset: 0x000028D3
		// (set) Token: 0x0600012E RID: 302 RVA: 0x000028DB File Offset: 0x000028DB
		public IList<ProjectRuleVM> Rules { get; private set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600012F RID: 303 RVA: 0x000028E4 File Offset: 0x000028E4
		// (set) Token: 0x06000130 RID: 304 RVA: 0x000028EC File Offset: 0x000028EC
		public string FileName { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00005C88 File Offset: 0x00005C88
		ConfuserProject IViewModel<ConfuserProject>.Model
		{
			get
			{
				return this.proj;
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00005DE0 File Offset: 0x00005DE0
		protected override void OnPropertyChanged(string property)
		{
			base.OnPropertyChanged(property);
			bool flag = property != "IsModified";
			if (flag)
			{
				this.IsModified = true;
			}
		}

		// Token: 0x0400006D RID: 109
		private readonly ConfuserProject proj;

		// Token: 0x0400006E RID: 110
		private bool modified;

		// Token: 0x0400006F RID: 111
		private ProjectSettingVM<Packer> packer;
	}
}
