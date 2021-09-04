using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000021 RID: 33
	public class AppVM : ViewModelBase
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00004D20 File Offset: 0x00004D20
		// (set) Token: 0x060000BD RID: 189 RVA: 0x0000255A File Offset: 0x0000255A
		public bool NavigationDisabled
		{
			get
			{
				return this.navDisabled;
			}
			set
			{
				base.SetProperty<bool>(ref this.navDisabled, value, "NavigationDisabled");
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00004D38 File Offset: 0x00004D38
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00004D50 File Offset: 0x00004D50
		public ProjectVM Project
		{
			get
			{
				return this.proj;
			}
			set
			{
				bool flag = this.proj != null;
				if (flag)
				{
					this.proj.PropertyChanged -= this.OnProjectPropertyChanged;
				}
				base.SetProperty<ProjectVM>(ref this.proj, value, "Project");
				bool flag2 = this.proj != null;
				if (flag2)
				{
					this.proj.PropertyChanged += this.OnProjectPropertyChanged;
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x00004DBC File Offset: 0x00004DBC
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00002570 File Offset: 0x00002570
		public string FileName
		{
			get
			{
				return this.fileName;
			}
			set
			{
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x00002572 File Offset: 0x00002572
		public string Title
		{
			get
			{
				return string.Format("{0}{1} - {2}", Path.GetFileName(this.fileName), this.proj.IsModified ? "" : "", "198 Protector V4 by Aptitude#2684 // Private Edition -");
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x00004DD4 File Offset: 0x00004DD4
		public IList<TabViewModel> Tabs
		{
			get
			{
				return this.tabs;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x00004DEC File Offset: 0x00004DEC
		public ICommand NewProject
		{
			get
			{
				return new RelayCommand(new Action(this.NewProj), () => !this.NavigationDisabled, false);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00004E1C File Offset: 0x00004E1C
		public ICommand OpenProject
		{
			get
			{
				return new RelayCommand(new Action(this.OpenProj), () => !this.NavigationDisabled, false);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00004E4C File Offset: 0x00004E4C
		public ICommand SaveProject
		{
			get
			{
				return new RelayCommand(delegate()
				{
					this.SaveProj();
				}, () => !this.NavigationDisabled, false);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00004E7C File Offset: 0x00004E7C
		public ICommand Decode
		{
			get
			{
				return new RelayCommand(delegate()
				{
					new StackTraceDecoder
					{
						Owner = Application.Current.MainWindow
					}.ShowDialog();
				}, () => !this.NavigationDisabled, false);
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00004EC0 File Offset: 0x00004EC0
		public bool OnWindowClosing()
		{
			return this.PromptSave();
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00004ED8 File Offset: 0x00004ED8
		private bool SaveProj()
		{
			bool flag = !this.firstSaved || !File.Exists(this.FileName);
			if (flag)
			{
				VistaSaveFileDialog vistaSaveFileDialog = new VistaSaveFileDialog();
				vistaSaveFileDialog.FileName = this.FileName;
				vistaSaveFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
				vistaSaveFileDialog.DefaultExt = ".crproj";
				vistaSaveFileDialog.AddExtension = true;
				bool flag2 = !vistaSaveFileDialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault() || vistaSaveFileDialog.FileName == null;
				if (flag2)
				{
					return false;
				}
				this.FileName = vistaSaveFileDialog.FileName;
			}
			ConfuserProject model = ((IViewModel<ConfuserProject>)this.Project).Model;
			model.Save().Save(this.FileName);
			this.Project.IsModified = false;
			this.firstSaved = true;
			return true;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00004FB0 File Offset: 0x00004FB0
		private bool PromptSave()
		{
			bool result;
			if (!this.Project.IsModified)
			{
				result = true;
			}
			else
			{
				MessageBoxResult messageBoxResult = MessageBox.Show("The current project has unsaved changes. Do you want to save them?", "198 Protector V4", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				if (messageBoxResult != MessageBoxResult.Cancel)
				{
					if (messageBoxResult != MessageBoxResult.Yes)
					{
						result = (messageBoxResult == MessageBoxResult.No);
					}
					else
					{
						result = this.SaveProj();
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00004FFC File Offset: 0x00004FFC
		private void NewProj()
		{
			bool flag = !this.PromptSave();
			if (!flag)
			{
				this.Project = new ProjectVM(new ConfuserProject(), null);
				this.FileName = "Unnamed.crproj";
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005038 File Offset: 0x00005038
		private void OpenProj()
		{
			if (this.PromptSave())
			{
				VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
				vistaOpenFileDialog.Filter = "ConfuserEx Projects (*.crproj)|*.crproj|All Files (*.*)|*.*";
				if (vistaOpenFileDialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault() && vistaOpenFileDialog.FileName != null)
				{
					string filename = vistaOpenFileDialog.FileName;
					try
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.Load(filename);
						ConfuserProject confuserProject = new ConfuserProject();
						confuserProject.Load(xmlDocument);
						this.Project = new ProjectVM(confuserProject, filename);
						this.FileName = filename;
					}
					catch
					{
						MessageBox.Show("Invalid project!", "198 Protector V3", MessageBoxButton.OK, MessageBoxImage.Hand);
					}
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000050EC File Offset: 0x000050EC
		private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			bool flag = e.PropertyName == "IsModified";
			if (flag)
			{
				this.OnPropertyChanged("Title");
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000511C File Offset: 0x0000511C
		protected override void OnPropertyChanged(string property)
		{
			base.OnPropertyChanged(property);
			bool flag = property == "Project";
			if (flag)
			{
				this.LoadPlugins();
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005148 File Offset: 0x00005148
		private void LoadPlugins()
		{
			foreach (StringItem stringItem in this.Project.Plugins)
			{
				try
				{
					ComponentDiscovery.LoadComponents(this.Project.Protections, this.Project.Packers, stringItem.Item);
				}
				catch
				{
					MessageBox.Show("Failed to load plugin '" + stringItem + "'.");
				}
			}
		}

		// Token: 0x04000053 RID: 83
		private readonly IList<TabViewModel> tabs = new ObservableCollection<TabViewModel>();

		// Token: 0x04000054 RID: 84
		private string fileName;

		// Token: 0x04000055 RID: 85
		private bool navDisabled;

		// Token: 0x04000056 RID: 86
		private bool firstSaved;

		// Token: 0x04000057 RID: 87
		private ProjectVM proj;
	}
}
