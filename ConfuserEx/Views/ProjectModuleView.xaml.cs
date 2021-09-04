using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ConfuserEx.ViewModel;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx.Views
{
	// Token: 0x02000017 RID: 23
	public partial class ProjectModuleView : Window
	{
		// Token: 0x06000070 RID: 112 RVA: 0x0000233E File Offset: 0x0000233E
		public ProjectModuleView(ProjectModuleVM module)
		{
			this.InitializeComponent();
			this.module = module;
			base.DataContext = module;
			this.PwdBox.IsEnabled = !string.IsNullOrEmpty(this.PathBox.Text);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x0000237D File Offset: 0x0000237D
		private void Done(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(true);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x0000238D File Offset: 0x0000238D
		private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.PwdBox.IsEnabled = !string.IsNullOrEmpty(this.PathBox.Text);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004154 File Offset: 0x00004154
		private void ChooseSNKey(object sender, RoutedEventArgs e)
		{
			VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
			vistaOpenFileDialog.Filter = "Supported Key Files (*.snk, *.pfx)|*.snk;*.pfx|All Files (*.*)|*.*";
			bool valueOrDefault = vistaOpenFileDialog.ShowDialog().GetValueOrDefault();
			if (valueOrDefault)
			{
				this.module.SNKeyPath = vistaOpenFileDialog.FileName;
			}
		}

		// Token: 0x04000031 RID: 49
		private readonly ProjectModuleVM module;
	}
}
