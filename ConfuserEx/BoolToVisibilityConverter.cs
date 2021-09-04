using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ConfuserEx
{
	// Token: 0x0200000C RID: 12
	internal class BoolToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000033 RID: 51 RVA: 0x000020F6 File Offset: 0x000020F6
		private BoolToVisibilityConverter()
		{
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003398 File Offset: 0x00003398
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is bool);
			Debug.Assert(targetType == typeof(Visibility));
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000021EA File Offset: 0x000021EA
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x04000014 RID: 20
		public static readonly BoolToVisibilityConverter Instance = new BoolToVisibilityConverter();
	}
}
