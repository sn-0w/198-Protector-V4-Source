using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Confuser.Core;
using Confuser.Core.Project;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200002B RID: 43
	internal class ProtectTabVM : TabViewModel, ILogger
	{
		// Token: 0x06000142 RID: 322 RVA: 0x00005E10 File Offset: 0x00005E10
		public ProtectTabVM(AppVM app) : base(app, "Protect!")
		{
			this.documentContent = new Paragraph();
			this.LogDocument = new FlowDocument();
			this.LogDocument.Blocks.Add(this.documentContent);
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00005E70 File Offset: 0x00005E70
		public ICommand ProtectCmd
		{
			get
			{
				return new RelayCommand(new Action(this.DoProtect), () => !base.App.NavigationDisabled, false);
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00005EA0 File Offset: 0x00005EA0
		public ICommand CancelCmd
		{
			get
			{
				return new RelayCommand(new Action(this.DoCancel), () => base.App.NavigationDisabled, false);
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00005ED0 File Offset: 0x00005ED0
		// (set) Token: 0x06000146 RID: 326 RVA: 0x0000297B File Offset: 0x0000297B
		public double? Progress
		{
			get
			{
				return this.progress;
			}
			set
			{
				base.SetProperty<double?>(ref this.progress, value, "Progress");
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00002991 File Offset: 0x00002991
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00002999 File Offset: 0x00002999
		public FlowDocument LogDocument { get; private set; }

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00005EE8 File Offset: 0x00005EE8
		// (set) Token: 0x0600014A RID: 330 RVA: 0x000029A2 File Offset: 0x000029A2
		public bool? Result
		{
			get
			{
				return this.result;
			}
			set
			{
				base.SetProperty<bool?>(ref this.result, value, "Result");
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00005F00 File Offset: 0x00005F00
		private void DoProtect()
		{
			ConfuserParameters confuserParameters = new ConfuserParameters();
			confuserParameters.Project = ((IViewModel<ConfuserProject>)base.App.Project).Model;
			bool flag = File.Exists(base.App.FileName);
			if (flag)
			{
				Environment.CurrentDirectory = Path.GetDirectoryName(base.App.FileName);
			}
			confuserParameters.Logger = this;
			this.documentContent.Inlines.Clear();
			this.cancelSrc = new CancellationTokenSource();
			this.Result = null;
			this.Progress = null;
			this.begin = DateTime.Now;
			base.App.NavigationDisabled = true;
			ConfuserEngine.Run(confuserParameters, new CancellationToken?(this.cancelSrc.Token)).ContinueWith<DispatcherOperation>((Task _) => Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.Progress = new double?(0.0);
				base.App.NavigationDisabled = false;
				CommandManager.InvalidateRequerySuggested();
			}), new object[0]));
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000029B8 File Offset: 0x000029B8
		private void DoCancel()
		{
			this.cancelSrc.Cancel();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00005FDC File Offset: 0x00005FDC
		private void AppendLine(string format, Brush foreground, params object[] args)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.documentContent.Inlines.Add(new Run(string.Format(format, args))
				{
					Foreground = foreground
				});
				this.documentContent.Inlines.Add(new LineBreak());
			}), new object[0]);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000029C7 File Offset: 0x000029C7
		void ILogger.Debug(string msg)
		{
			this.AppendLine("[DEBUG] {0}", Brushes.Gray, new object[]
			{
				msg
			});
		}

		// Token: 0x0600014F RID: 335 RVA: 0x000029E5 File Offset: 0x000029E5
		void ILogger.DebugFormat(string format, params object[] args)
		{
			this.AppendLine("[DEBUG] {0}", Brushes.Gray, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00002A09 File Offset: 0x00002A09
		void ILogger.Info(string msg)
		{
			this.AppendLine(" [INFO] {0}", Brushes.White, new object[]
			{
				msg
			});
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00002A27 File Offset: 0x00002A27
		void ILogger.InfoFormat(string format, params object[] args)
		{
			this.AppendLine(" [INFO] {0}", Brushes.White, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00002A4B File Offset: 0x00002A4B
		void ILogger.Warn(string msg)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				msg
			});
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00002A69 File Offset: 0x00002A69
		void ILogger.WarnFormat(string format, params object[] args)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00002A8D File Offset: 0x00002A8D
		void ILogger.WarnException(string msg, Exception ex)
		{
			this.AppendLine(" [WARN] {0}", Brushes.Yellow, new object[]
			{
				msg
			});
			this.AppendLine("Exception: {0}", Brushes.Yellow, new object[]
			{
				ex
			});
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00002AC6 File Offset: 0x00002AC6
		void ILogger.Error(string msg)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				msg
			});
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00002AE4 File Offset: 0x00002AE4
		void ILogger.ErrorFormat(string format, params object[] args)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				string.Format(format, args)
			});
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00002B08 File Offset: 0x00002B08
		void ILogger.ErrorException(string msg, Exception ex)
		{
			this.AppendLine("[ERROR] {0}", Brushes.Red, new object[]
			{
				msg
			});
			this.AppendLine("Exception: {0}", Brushes.Red, new object[]
			{
				ex
			});
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00002B41 File Offset: 0x00002B41
		void ILogger.Progress(int progress, int overall)
		{
			this.Progress = new double?((double)progress / (double)overall);
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00006030 File Offset: 0x00006030
		void ILogger.EndProgress()
		{
			this.Progress = null;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00006050 File Offset: 0x00006050
		void ILogger.Finish(bool successful)
		{
			DateTime now = DateTime.Now;
			string text = string.Format("at {0}, {1}:{2:d2} elapsed.", now.ToShortTimeString(), (int)now.Subtract(this.begin).TotalMinutes, now.Subtract(this.begin).Seconds);
			if (successful)
			{
				this.AppendLine("Finished {0}", Brushes.Lime, new object[]
				{
					text
				});
			}
			else
			{
				this.AppendLine("Failed {0}", Brushes.Red, new object[]
				{
					text
				});
			}
			this.Result = new bool?(successful);
		}

		// Token: 0x0400007A RID: 122
		private readonly Paragraph documentContent;

		// Token: 0x0400007B RID: 123
		private CancellationTokenSource cancelSrc;

		// Token: 0x0400007C RID: 124
		private double? progress = new double?(0.0);

		// Token: 0x0400007D RID: 125
		private bool? result;

		// Token: 0x0400007F RID: 127
		private DateTime begin;
	}
}
