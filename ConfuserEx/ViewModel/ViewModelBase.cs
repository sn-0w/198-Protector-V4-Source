using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000035 RID: 53
	public class ViewModelBase : INotifyPropertyChanged
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000188 RID: 392 RVA: 0x00006A60 File Offset: 0x00006A60
		// (remove) Token: 0x06000189 RID: 393 RVA: 0x00006A98 File Offset: 0x00006A98
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x0600018A RID: 394 RVA: 0x00006AD0 File Offset: 0x00006AD0
		protected virtual void OnPropertyChanged(string property)
		{
			bool flag = this.PropertyChanged != null;
			if (flag)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00006B00 File Offset: 0x00006B00
		protected bool SetProperty<T>(ref T field, T value, string property)
		{
			bool flag = !EqualityComparer<T>.Default.Equals(field, value);
			bool result;
			if (flag)
			{
				field = value;
				this.OnPropertyChanged(property);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00006B40 File Offset: 0x00006B40
		protected bool SetProperty<T>(bool changed, Action<T> setter, T value, string property)
		{
			bool result;
			if (changed)
			{
				setter(value);
				this.OnPropertyChanged(property);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
