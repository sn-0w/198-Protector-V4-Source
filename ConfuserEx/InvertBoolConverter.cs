using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace ConfuserEx
{
	// Token: 0x02000011 RID: 17
	internal class InvertBoolConverter : IValueConverter
	{
		// Token: 0x0600004B RID: 75 RVA: 0x000020F6 File Offset: 0x000020F6
		private InvertBoolConverter()
		{
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003944 File Offset: 0x00003944
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is bool);
			Debug.Assert(targetType == typeof(bool));
			return !(bool)value;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000021EA File Offset: 0x000021EA
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		// Token: 0x0400001E RID: 30
		public static readonly InvertBoolConverter Instance = new InvertBoolConverter();
	}
}
