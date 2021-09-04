using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ConfuserEx.ViewModel;
using GalaSoft.MvvmLight.Command;

namespace ConfuserEx
{
	// Token: 0x0200000E RID: 14
	public class FileDragDrop
	{
		// Token: 0x06000039 RID: 57 RVA: 0x00003400 File Offset: 0x00003400
		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(FileDragDrop.CommandProperty);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002237 File Offset: 0x00002237
		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(FileDragDrop.CommandProperty, value);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003424 File Offset: 0x00003424
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			UIElement uielement = (UIElement)d;
			bool flag = e.NewValue != null;
			if (flag)
			{
				uielement.AllowDrop = true;
				uielement.PreviewDragOver += FileDragDrop.OnDragOver;
				uielement.PreviewDrop += FileDragDrop.OnDrop;
			}
			else
			{
				uielement.AllowDrop = false;
				uielement.PreviewDragOver -= FileDragDrop.OnDragOver;
				uielement.PreviewDrop -= FileDragDrop.OnDrop;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000034AC File Offset: 0x000034AC
		private static void OnDragOver(object sender, DragEventArgs e)
		{
			ICommand command = FileDragDrop.GetCommand((DependencyObject)sender);
			e.Effects = DragDropEffects.None;
			bool flag = command is FileDragDrop.DragDropCommand;
			if (flag)
			{
				bool flag2 = command.CanExecute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				if (flag2)
				{
					e.Effects = DragDropEffects.Link;
				}
			}
			else
			{
				bool flag3 = command.CanExecute(e.Data);
				if (flag3)
				{
					e.Effects = DragDropEffects.Link;
				}
			}
			e.Handled = true;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003524 File Offset: 0x00003524
		private static void OnDrop(object sender, DragEventArgs e)
		{
			ICommand command = FileDragDrop.GetCommand((DependencyObject)sender);
			bool flag = command is FileDragDrop.DragDropCommand;
			if (flag)
			{
				bool flag2 = command.CanExecute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				if (flag2)
				{
					command.Execute(Tuple.Create<UIElement, IDataObject>((UIElement)sender, e.Data));
				}
			}
			else
			{
				bool flag3 = command.CanExecute(e.Data);
				if (flag3)
				{
					command.Execute(e.Data);
				}
			}
			e.Handled = true;
		}

		// Token: 0x04000016 RID: 22
		public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(FileDragDrop), new UIPropertyMetadata(null, new PropertyChangedCallback(FileDragDrop.OnCommandChanged)));

		// Token: 0x04000017 RID: 23
		public static ICommand FileCmd = new FileDragDrop.DragDropCommand(delegate(Tuple<UIElement, IDataObject> data)
		{
			Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
			bool flag = data.Item1 is TextBox;
			if (flag)
			{
				string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
				Debug.Assert(File.Exists(text));
				((TextBox)data.Item1).Text = text;
			}
			else
			{
				bool flag2 = data.Item1 is ListBox;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
				Debug.Assert(array.All((string file) => File.Exists(file)));
				IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
				foreach (string item in array)
				{
					list.Add(new StringItem(item));
				}
			}
		}, delegate(Tuple<UIElement, IDataObject> data)
		{
			bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string file) => File.Exists(file));
			}
			return result;
		});

		// Token: 0x04000018 RID: 24
		public static ICommand DirectoryCmd = new FileDragDrop.DragDropCommand(delegate(Tuple<UIElement, IDataObject> data)
		{
			Debug.Assert(data.Item2.GetDataPresent(DataFormats.FileDrop));
			bool flag = data.Item1 is TextBox;
			if (flag)
			{
				string text = ((string[])data.Item2.GetData(DataFormats.FileDrop))[0];
				Debug.Assert(Directory.Exists(text));
				((TextBox)data.Item1).Text = text;
			}
			else
			{
				bool flag2 = data.Item1 is ListBox;
				if (!flag2)
				{
					throw new NotSupportedException();
				}
				string[] array = (string[])data.Item2.GetData(DataFormats.FileDrop);
				Debug.Assert(array.All((string dir) => Directory.Exists(dir)));
				IList<StringItem> list = (IList<StringItem>)((ListBox)data.Item1).ItemsSource;
				foreach (string item in array)
				{
					list.Add(new StringItem(item));
				}
			}
		}, delegate(Tuple<UIElement, IDataObject> data)
		{
			bool flag = !data.Item2.GetDataPresent(DataFormats.FileDrop);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				result = ((string[])data.Item2.GetData(DataFormats.FileDrop)).All((string dir) => Directory.Exists(dir));
			}
			return result;
		});

		// Token: 0x0200000F RID: 15
		private class DragDropCommand : RelayCommand<Tuple<UIElement, IDataObject>>
		{
			// Token: 0x06000040 RID: 64 RVA: 0x00002247 File Offset: 0x00002247
			public DragDropCommand(Action<Tuple<UIElement, IDataObject>> execute, Func<Tuple<UIElement, IDataObject>, bool> canExecute) : base(execute, canExecute, false)
			{
			}
		}
	}
}
