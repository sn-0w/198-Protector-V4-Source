using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Confuser.Core;

namespace ConfuserEx
{
	// Token: 0x02000005 RID: 5
	internal class ComponentConverter : Freezable, IValueConverter
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002ED0 File Offset: 0x00002ED0
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002155 File Offset: 0x00002155
		public IList<ConfuserComponent> Components
		{
			get
			{
				return (IList<ConfuserComponent>)base.GetValue(ComponentConverter.ComponentsProperty);
			}
			set
			{
				base.SetValue(ComponentConverter.ComponentsProperty, value);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002EF4 File Offset: 0x00002EF4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is string || value == null);
			Debug.Assert(targetType == typeof(ConfuserComponent));
			Debug.Assert(this.Components != null);
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = this.Components.Single((ConfuserComponent comp) => comp.Id == (string)value);
			}
			return result;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002F80 File Offset: 0x00002F80
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.Assert(value is ConfuserComponent || value == null);
			Debug.Assert(targetType == typeof(string));
			bool flag = value == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = ((ConfuserComponent)value).Id;
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002FD4 File Offset: 0x00002FD4
		protected override Freezable CreateInstanceCore()
		{
			return new ComponentConverter();
		}

		// Token: 0x04000007 RID: 7
		public static readonly DependencyProperty ComponentsProperty = DependencyProperty.Register("Components", typeof(IList<ConfuserComponent>), typeof(ComponentConverter), new UIPropertyMetadata(null));
	}
}
