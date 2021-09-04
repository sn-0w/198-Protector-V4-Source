using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Xml;
using Confuser.Core.Project;
using ConfuserEx.ViewModel;

namespace ConfuserEx
{
	// Token: 0x02000016 RID: 22
	public partial class MainWindow : Window
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00003F9C File Offset: 0x00003F9C
		public MainWindow()
		{
			this.InitializeComponent();
			AppVM appVM = new AppVM();
			appVM.Project = new ProjectVM(new ConfuserProject(), null);
			appVM.FileName = "Unnamed.crproj";
			appVM.Tabs.Add(new ProjectTabVM(appVM));
			appVM.Tabs.Add(new SettingsTabVM(appVM));
			appVM.Tabs.Add(new ProtectTabVM(appVM));
			this.LoadProj(appVM);
			base.DataContext = appVM;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004028 File Offset: 0x00004028
		private void OpenMenu(object sender, RoutedEventArgs e)
		{
			Button button = (Button)sender;
			ContextMenu contextMenu = button.ContextMenu;
			contextMenu.PlacementTarget = button;
			contextMenu.Placement = PlacementMode.MousePoint;
			contextMenu.IsOpen = true;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x0000405C File Offset: 0x0000405C
		private void LoadProj(AppVM app)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length == 2 && File.Exists(commandLineArgs[1]))
			{
				string fullPath = Path.GetFullPath(commandLineArgs[1]);
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load(fullPath);
					ConfuserProject confuserProject = new ConfuserProject();
					confuserProject.Load(xmlDocument);
					app.Project = new ProjectVM(confuserProject, fullPath);
					app.FileName = fullPath;
				}
				catch
				{
					MessageBox.Show("Invalid project!", "198 Protector V4", MessageBoxButton.OK, MessageBoxImage.Hand);
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00002319 File Offset: 0x00002319
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			e.Cancel = !((AppVM)base.DataContext).OnWindowClosing();
		}
	}
}
