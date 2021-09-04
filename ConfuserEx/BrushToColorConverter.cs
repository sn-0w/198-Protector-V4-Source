using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ConfuserEx
{
	// Token: 0x02000003 RID: 3
	public class BrushToColorConverter : IValueConverter
	{
		// Token: 0x06000007 RID: 7 RVA: 0x000020F6 File Offset: 0x000020F6
		private BrushToColorConverter()
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002D44 File Offset: 0x00002D44
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SolidColorBrush solidColorBrush = value as SolidColorBrush;
			bool flag = solidColorBrush != null;
			object result;
			if (flag)
			{
				result = solidColorBrush.Color;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002100 File Offset: 0x00002100
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000001 RID: 1
		public static readonly BrushToColorConverter Instance = new BrushToColorConverter();
	}
}
