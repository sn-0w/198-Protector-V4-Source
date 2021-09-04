using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading;
using Confuser.Core.Project;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000024 RID: 36
	public class ProjectModuleVM : ViewModelBase, IViewModel<ProjectModule>, IRuleContainer
	{
		// Token: 0x060000DA RID: 218 RVA: 0x000051E8 File Offset: 0x000051E8
		public ProjectModuleVM(ProjectVM parent, ProjectModule module)
		{
			this.parent = parent;
			this.module = module;
			ObservableCollection<ProjectRuleVM> observableCollection = Utils.Wrap<Rule, ProjectRuleVM>(module.Rules, (Rule rule) => new ProjectRuleVM(parent, rule));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				parent.IsModified = true;
			};
			this.Rules = observableCollection;
			bool flag = module.Path != null;
			if (flag)
			{
				this.SimpleName = System.IO.Path.GetFileName(module.Path);
				this.LoadAssemblyName();
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000DB RID: 219 RVA: 0x00005284 File Offset: 0x00005284
		// (set) Token: 0x060000DC RID: 220 RVA: 0x000025F9 File Offset: 0x000025F9
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSelected, value, "IsSelected");
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000DD RID: 221 RVA: 0x0000529C File Offset: 0x0000529C
		public ProjectModule Module
		{
			get
			{
				return this.module;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000052B4 File Offset: 0x000052B4
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000052D4 File Offset: 0x000052D4
		public string Path
		{
			get
			{
				return this.module.Path;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.Path != value, delegate(string val)
				{
					this.module.Path = val;
				}, value, "Path");
				if (flag)
				{
					this.parent.IsModified = true;
					this.SimpleName = System.IO.Path.GetFileName(this.module.Path);
					this.LoadAssemblyName();
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000533C File Offset: 0x0000533C
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x0000260F File Offset: 0x0000260F
		public string SimpleName
		{
			get
			{
				return this.simpleName;
			}
			private set
			{
				base.SetProperty<string>(ref this.simpleName, value, "SimpleName");
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00005354 File Offset: 0x00005354
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00002625 File Offset: 0x00002625
		public string AssemblyName
		{
			get
			{
				return this.asmName;
			}
			private set
			{
				base.SetProperty<string>(ref this.asmName, value, "AssemblyName");
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000536C File Offset: 0x0000536C
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000538C File Offset: 0x0000538C
		public string SNKeyPath
		{
			get
			{
				return this.module.SNKeyPath;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.SNKeyPath != value, delegate(string val)
				{
					this.module.SNKeyPath = val;
				}, value, "SNKeyPath");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000053D4 File Offset: 0x000053D4
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x000053F4 File Offset: 0x000053F4
		public string SNKeyPassword
		{
			get
			{
				return this.module.SNKeyPassword;
			}
			set
			{
				bool flag = base.SetProperty<string>(this.module.SNKeyPassword != value, delegate(string val)
				{
					this.module.SNKeyPassword = val;
				}, value, "SNKeyPassword");
				if (flag)
				{
					this.parent.IsModified = true;
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x0000263B File Offset: 0x0000263B
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00002643 File Offset: 0x00002643
		public IList<ProjectRuleVM> Rules { get; private set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000EA RID: 234 RVA: 0x0000529C File Offset: 0x0000529C
		ProjectModule IViewModel<ProjectModule>.Model
		{
			get
			{
				return this.module;
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000264C File Offset: 0x0000264C
		private void LoadAssemblyName()
		{
			this.AssemblyName = "Loading...";
			ThreadPool.QueueUserWorkItem(delegate(object _)
			{
				try
				{
					string text = System.IO.Path.Combine(this.parent.BaseDirectory, this.Path);
					bool flag = !string.IsNullOrEmpty(this.parent.FileName);
					if (flag)
					{
						text = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.parent.FileName), text);
					}
					AssemblyName assemblyName = System.Reflection.AssemblyName.GetAssemblyName(text);
					this.AssemblyName = assemblyName.FullName;
				}
				catch
				{
					this.AssemblyName = "Unknown";
				}
			});
		}

		// Token: 0x0400005A RID: 90
		private readonly ProjectModule module;

		// Token: 0x0400005B RID: 91
		private readonly ProjectVM parent;

		// Token: 0x0400005C RID: 92
		private string asmName = "Unknown";

		// Token: 0x0400005D RID: 93
		private string simpleName;

		// Token: 0x0400005E RID: 94
		private bool isSelected;
	}
}
