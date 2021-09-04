using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx.ViewModel
{
	// Token: 0x0200001F RID: 31
	internal class AboutTabVM : TabViewModel
	{
		// Token: 0x060000B4 RID: 180 RVA: 0x00004C90 File Offset: 0x00004C90
		public AboutTabVM(AppVM app)
		{
			IconBitmapDecoder iconBitmapDecoder = new IconBitmapDecoder(new Uri("pack://application:,,,/ConfuserEx.ico"), BitmapCreateOptions.DelayCreation, BitmapCacheOption.Default);
			this.Icon = iconBitmapDecoder.Frames.First((BitmapFrame frame) => frame.Width == 64.0);
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00004CE8 File Offset: 0x00004CE8
		public ICommand LaunchBrowser
		{
			get
			{
				return new RelayCommand<string>(delegate(string site)
				{
					Process.Start(site);
				}, false);
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x00002521 File Offset: 0x00002521
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x00002529 File Offset: 0x00002529
		public BitmapSource Icon { get; private set; }
	}
}
