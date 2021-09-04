using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.Views
{
	// Token: 0x02000019 RID: 25
	public partial class ProjectTabAdvancedView : Window
	{
		// Token: 0x06000085 RID: 133 RVA: 0x000023FB File Offset: 0x000023FB
		public ProjectTabAdvancedView(ProjectVM project)
		{
			this.InitializeComponent();
			this.project = project;
			base.DataContext = project;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000458C File Offset: 0x0000458C
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.AddPlugin.Command = new RelayCommand(delegate()
			{
				VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
				vistaOpenFileDialog.Filter = ".NET assemblies (*.exe, *.dll)|*.exe;*.dll|All Files (*.*)|*.*";
				vistaOpenFileDialog.Multiselect = true;
				bool valueOrDefault = vistaOpenFileDialog.ShowDialog().GetValueOrDefault();
				if (valueOrDefault)
				{
					foreach (string text in vistaOpenFileDialog.FileNames)
					{
						try
						{
							ComponentDiscovery.LoadComponents(this.project.Protections, this.project.Packers, text);
							this.project.Plugins.Add(new StringItem(text));
						}
						catch
						{
							MessageBox.Show("Failed to load plugin '" + text + "'.");
						}
					}
				}
			}, false);
			this.RemovePlugin.Command = new RelayCommand(delegate()
			{
				int selectedIndex = this.PluginPaths.SelectedIndex;
				Debug.Assert(selectedIndex != -1);
				string item = this.project.Plugins[selectedIndex].Item;
				ComponentDiscovery.RemoveComponents(this.project.Protections, this.project.Packers, item);
				this.project.Plugins.RemoveAt(selectedIndex);
				this.PluginPaths.SelectedIndex = ((selectedIndex >= this.project.Plugins.Count) ? (this.project.Plugins.Count - 1) : selectedIndex);
			}, () => this.PluginPaths.SelectedIndex != -1, false);
			this.AddProbe.Command = new RelayCommand(delegate()
			{
				VistaFolderBrowserDialog vistaFolderBrowserDialog = new VistaFolderBrowserDialog();
				bool valueOrDefault = vistaFolderBrowserDialog.ShowDialog().GetValueOrDefault();
				if (valueOrDefault)
				{
					this.project.ProbePaths.Add(new StringItem(vistaFolderBrowserDialog.SelectedPath));
				}
			}, false);
			this.RemoveProbe.Command = new RelayCommand(delegate()
			{
				int selectedIndex = this.ProbePaths.SelectedIndex;
				Debug.Assert(selectedIndex != -1);
				this.project.ProbePaths.RemoveAt(selectedIndex);
				this.ProbePaths.SelectedIndex = ((selectedIndex >= this.project.ProbePaths.Count) ? (this.project.ProbePaths.Count - 1) : selectedIndex);
			}, () => this.ProbePaths.SelectedIndex != -1, false);
		}

		// Token: 0x0400003E RID: 62
		private readonly ProjectVM project;
	}
}
