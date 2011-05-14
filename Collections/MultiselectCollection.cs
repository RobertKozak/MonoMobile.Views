// 
//  MultiselectCollection.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq; 
	
	public class SelectionItem<T> : ObservableObject
	{		
		public string Description 
		{
			get { return Get(() => Description); }
			set { Set(() => Description, value); }
		}
		public bool IsSelected
        {
            get { return Get(()=>IsSelected); }
            set { Set(()=>IsSelected, value); OnSelectionChanged(); }
        }

        public T Item
        {
            get { return Get(()=>Item); }
            set { Set(()=>Item, value); }
        }
		
		public int Index
		{
			get { return Get(() => Index); }
			set { Set(() => Index, value); }
		}
		
		public event EventHandler SelectionChanged = (sender, args) => { };

        public SelectionItem(T item): this(false, item)
        {
			Description = item.ToString();
        }

        public SelectionItem(bool selected, T item)
        {
            IsSelected = selected;
            Item = item;
        }

        private void OnSelectionChanged()
        {
           SelectionChanged(this, EventArgs.Empty);
        }

		public override string ToString()
		{
			return Description;
		}
	}
	
	public interface IMultiselectCollection<T>: ICollection<SelectionItem<T>>
	{
		IEnumerable<T> SelectedItems { get; }
		IEnumerable<SelectionItem<T>> Selected { get; }
		IEnumerable<T> AllItems { get; }
	}

	public interface IMultiselectCollection : IMultiselectCollection<object>
	{
	}
	
	public class MultiselectCollection<T> : ObservableCollection<SelectionItem<T>>, IMultiselectCollection<T>
    {		
		public IEnumerable<T> SelectedItems
		{
			get { return this.Where(x => x.IsSelected).Select(x => x.Item); }
		}
		public IEnumerable<SelectionItem<T>> Selected
		{
			get { return this.Where(x => x.IsSelected); }
		}

		public IEnumerable<T> AllItems
		{
			get { return this.Select(x => x.Item); }
		}
		
		public MultiselectCollection(): base() { }
		public MultiselectCollection(IEnumerable<T> collection) : base(ToSelectionItemEnumerable(collection))
		{
		}
		
		public void Add(T item)
		{
		    int i = 0;

		    foreach (T existingItem in AllItems)
		    {
				if (EqualityComparer<T>.Default.Equals(item, existingItem)) break;
		        i++;
		    }

		    Insert(i, new SelectionItem<T>(item));
		}
		
		
		public bool Contains(T item)
		{
		    return AllItems.Contains(item);
		}
		
		public void SelectAll()
		{
		    foreach (SelectionItem<T> selectionItem in this)
		    {
		        selectionItem.IsSelected = true;
		    }
		}
		
		public void UnselectAll()
		{
		    foreach (SelectionItem<T> selectionItem in this)
		    {
		        selectionItem.IsSelected = false;
		    }
		}
		
		private static List<SelectionItem<T>> ToSelectionItemEnumerable(IEnumerable<T> items)
		{
		    List<SelectionItem<T>> list = new List<SelectionItem<T>>();

		    foreach (T item in items)
		    {
		        SelectionItem<T> selectionItem = new SelectionItem<T>(item);
		        list.Add(selectionItem);
		    }

		    return list;
		}
    }
}

