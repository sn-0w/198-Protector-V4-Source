using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Confuser.Core;
using Confuser.Renamer;
using Ookii.Dialogs.Wpf;

namespace ConfuserEx
{
	// Token: 0x02000014 RID: 20
	public partial class StackTraceDecoder : Window
	{
		// Token: 0x0600005C RID: 92 RVA: 0x000022D4 File Offset: 0x000022D4
		public StackTraceDecoder()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00003B58 File Offset: 0x00003B58
		private void PathBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool flag = File.Exists(this.PathBox.Text);
			if (flag)
			{
				this.LoadSymMap(this.PathBox.Text);
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003B8C File Offset: 0x00003B8C
		private void LoadSymMap(string path)
		{
			string str = path;
			bool flag = path.Length > 35;
			if (flag)
			{
				str = "..." + path.Substring(path.Length - 35, 35);
			}
			try
			{
				this.symMap.Clear();
				using (StreamReader streamReader = new StreamReader(File.OpenRead(path)))
				{
					for (string text = streamReader.ReadLine(); text != null; text = streamReader.ReadLine())
					{
						int num = text.IndexOf('\t');
						bool flag2 = num == -1;
						if (flag2)
						{
							throw new FileFormatException();
						}
						this.symMap.Add(text.Substring(0, num), text.Substring(num + 1));
					}
				}
				this.status.Content = "Loaded symbol map from '" + str + "' successfully.";
			}
			catch
			{
				this.status.Content = "Failed to load symbol map from '" + str + "'.";
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003CA4 File Offset: 0x00003CA4
		private void ChooseMapPath(object sender, RoutedEventArgs e)
		{
			VistaOpenFileDialog vistaOpenFileDialog = new VistaOpenFileDialog();
			vistaOpenFileDialog.Filter = "Symbol maps (*.map)|*.map|All Files (*.*)|*.*";
			bool valueOrDefault = vistaOpenFileDialog.ShowDialog().GetValueOrDefault();
			if (valueOrDefault)
			{
				this.PathBox.Text = vistaOpenFileDialog.FileName;
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003CEC File Offset: 0x00003CEC
		private void Decode_Click(object sender, RoutedEventArgs e)
		{
			string text = this.stackTrace.Text;
			bool flag = this.optSym.IsChecked ?? true;
			if (flag)
			{
				this.stackTrace.Text = this.mapSymbolMatcher.Replace(text, new MatchEvaluator(this.DecodeSymbolMap));
			}
			else
			{
				this.renamer = new ReversibleRenamer(this.PassBox.Text);
				this.stackTrace.Text = this.passSymbolMatcher.Replace(text, new MatchEvaluator(this.DecodeSymbolPass));
			}
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003D8C File Offset: 0x00003D8C
		private string DecodeSymbolMap(Match match)
		{
			string value = match.Value;
			return this.symMap.GetValueOrDefault(value, value);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003DB4 File Offset: 0x00003DB4
		private string DecodeSymbolPass(Match match)
		{
			string value = match.Value;
			string result;
			try
			{
				result = this.renamer.Decrypt(value);
			}
			catch
			{
				result = value;
			}
			return result;
		}

		// Token: 0x04000024 RID: 36
		private readonly Dictionary<string, string> symMap = new Dictionary<string, string>();

		// Token: 0x04000025 RID: 37
		private readonly Regex mapSymbolMatcher = new Regex("_[a-zA-Z0-9]+");

		// Token: 0x04000026 RID: 38
		private readonly Regex passSymbolMatcher = new Regex("[a-zA-Z0-9_$]{23,}");

		// Token: 0x04000027 RID: 39
		private ReversibleRenamer renamer;
	}
}
