using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace ConfuserEx.ViewModel
{
	// Token: 0x02000032 RID: 50
	public static class Utils
	{
		// Token: 0x06000181 RID: 385 RVA: 0x000066B0 File Offset: 0x000066B0
		public static ObservableCollection<T> Wrap<T>(IList<T> list)
		{
			ObservableCollection<T> observableCollection = new ObservableCollection<T>(list);
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<T> observableCollection2 = (ObservableCollection<T>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						list.Insert(e.NewStartingIndex + i, (T)((object)e.NewItems[i]));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					list[e.NewStartingIndex] = (T)((object)e.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Move:
					list.RemoveAt(e.OldStartingIndex);
					list.Insert(e.NewStartingIndex, (T)((object)e.NewItems[0]));
					break;
				case NotifyCollectionChangedAction.Reset:
					list.Clear();
					foreach (T item in observableCollection2)
					{
						list.Add(item);
					}
					break;
				}
			};
			return observableCollection;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000066F0 File Offset: 0x000066F0
		public static ObservableCollection<TViewModel> Wrap<TModel, TViewModel>(IList<TModel> list, Func<TModel, TViewModel> transform) where TViewModel : IViewModel<TModel>
		{
			ObservableCollection<TViewModel> observableCollection = new ObservableCollection<TViewModel>(from item in list
			select transform(item));
			observableCollection.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
			{
				ObservableCollection<TViewModel> observableCollection2 = (ObservableCollection<TViewModel>)sender;
				switch (e.Action)
				{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						IList<TModel> list2 = list;
						int index = e.NewStartingIndex + i;
						TViewModel tviewModel = (TViewModel)((object)e.NewItems[i]);
						list2.Insert(index, tviewModel.Model);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for (int j = 0; j < e.OldItems.Count; j++)
					{
						list.RemoveAt(e.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Replace:
				{
					IList<TModel> list3 = list;
					int newStartingIndex = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list3[newStartingIndex] = tviewModel.Model;
					break;
				}
				case NotifyCollectionChangedAction.Move:
				{
					list.RemoveAt(e.OldStartingIndex);
					IList<TModel> list4 = list;
					int newStartingIndex2 = e.NewStartingIndex;
					TViewModel tviewModel = (TViewModel)((object)e.NewItems[0]);
					list4.Insert(newStartingIndex2, tviewModel.Model);
					break;
				}
				case NotifyCollectionChangedAction.Reset:
					list.Clear();
					foreach (TViewModel tviewModel2 in observableCollection2)
					{
						list.Add(tviewModel2.Model);
					}
					break;
				}
			};
			return observableCollection;
		}
	}
}
