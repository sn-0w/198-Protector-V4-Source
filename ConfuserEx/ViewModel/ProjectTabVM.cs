using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Confuser.Core;
using Confuser.Core.Project;
using ConfuserEx.Views;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000026 RID: 38
	public class ProjectTabVM : TabViewModel
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x000026B7 File Offset: 0x000026B7
		public ProjectTabVM(AppVM app) : base(app, "Project")
		{
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000F4 RID: 244 RVA: 0x000054C8 File Offset: 0x000054C8
		public ICommand DragDrop
		{
			get
			{
				return new RelayCommand<IDataObject>(delegate(IDataObject data)
				{
					foreach (string file in (string[])data.GetData(DataFormats.FileDrop))
					{
						this.AddModule(file);
					}
				}, delegate(IDataObject data)
				{
					bool flag = !data.GetDataPresent(DataFormats.FileDrop);
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						string[] source = (string[])data.GetData(DataFormats.FileDrop);
						bool flag2 = source.All((string file) => File.Exists(file));
						result = flag2;
					}
					return result;
				}, false);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000550C File Offset: 0x0000550C
		public ICommand ChooseBaseDir
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
					vistaFolderBrowserDialog.SelectedPath = base.App.Project.BaseDirectory;
					bool valueOrDefault = vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault();
					if (valueOrDefault)
					{
						base.App.Project.BaseDirectory = vistaFolderBrowserDialog.SelectedPath;
						base.App.Project.OutputDirectory = Path.Combine(base.App.Project.BaseDirectory, "Confused");
					}
				}, false);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00005530 File Offset: 0x00005530
		public ICommand ChooseOutputDir
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
					vistaFolderBrowserDialog.SelectedPath = base.App.Project.OutputDirectory;
					bool valueOrDefault = vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault();
					if (valueOrDefault)
					{
						base.App.Project.OutputDirectory = vistaFolderBrowserDialog.SelectedPath;
					}
				}, false);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00005554 File Offset: 0x00005554
		public ICommand Add
		{
			get
			{
				return new RelayCommand(delegate()
				{
					VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
					vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
					vistaOpenFileDialog.Multiselect = true;
					bool valueOrDefault = vistaOpenFileDialog.ShowDialog().GetValueOrDefault();
					if (valueOrDefault)
					{
						foreach (string file in vistaOpenFileDialog.FileNames)
						{
							this.AddModule(file);
						}
					}
				}, false);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x000026C7 File Offset: 0x000026C7
		public ICommand Remove
		{
			get
			{
				return new RelayCommand(delegate()
				{
					if (MessageBox.Show("Are you sure to remove selected modules?\r\nAll settings specific to it would be lost!", "GLOBALFALL V2", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						foreach (ProjectModuleVM item in (from m in base.App.Project.Modules
						where m.IsSelected
						select m).ToList<ProjectModuleVM>())
						{
							base.App.Project.Modules.Remove(item);
						}
					}
				}, () => base.App.Project.Modules.Any((ProjectModuleVM m) => m.IsSelected), false);
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00005578 File Offset: 0x00005578
		public ICommand Edit
		{
			get
			{
				return new RelayCommand(delegate()
				{
					Debug.Assert(base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1);
					new ProjectModuleView(base.App.Project.Modules.Single((ProjectModuleVM m) => m.IsSelected))
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				}, () => base.App.Project.Modules.Count((ProjectModuleVM m) => m.IsSelected) == 1, false);
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000FA RID: 250 RVA: 0x000055A8 File Offset: 0x000055A8
		public ICommand Advanced
		{
			get
			{
				return new RelayCommand(delegate()
				{
					new ProjectTabAdvancedView(base.App.Project)
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				}, false);
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x000055CC File Offset: 0x000055CC
		private void AddModule(string file)
		{
			bool flag = !File.Exists(file);
			if (flag)
			{
				MessageBox.Show(string.Format("File '{0}' does not exists!", file), "GLOBALFALL V2", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			else
			{
				bool flag2 = string.IsNullOrEmpty(base.App.Project.BaseDirectory);
				if (flag2)
				{
					string directoryName = Path.GetDirectoryName(file);
					base.App.Project.BaseDirectory = directoryName;
					base.App.Project.OutputDirectory = Path.Combine(directoryName, "Protected");
				}
				ProjectModuleVM projectModuleVM = new ProjectModuleVM(base.App.Project, new ProjectModule());
				try
				{
					projectModuleVM.Path = Utils.GetRelativePath(file, base.App.Project.BaseDirectory);
				}
				catch
				{
					projectModuleVM.Path = file;
				}
				base.App.Project.Modules.Add(projectModuleVM);
			}
		}
	}
}
