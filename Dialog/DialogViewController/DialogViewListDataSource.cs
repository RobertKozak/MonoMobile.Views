// 
//  DialogViewListDataSource.cs
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	
	
	public class DialogViewListDataSource : DialogViewDataSource
	{
		private Dictionary<int, IList> _Data;
		public Type ElementType { get; set; }

		protected UITableViewCell Cell { get; set; }
		protected IDictionary<int, IList> Data { get { return _Data; } }

		public DialogViewListDataSource(DialogViewController container) : base(container)
		{
			if (container != null && container.RootView != null)
			{
				var notifyDataContextChanged = container.RootView as INotifyDataContextChanged; 
				if (notifyDataContextChanged != null)
				{
					notifyDataContextChanged.DataContextChanged += DataContextChanged;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				var notifyDataContextChanged = Container.RootView as INotifyDataContextChanged;
				if (notifyDataContextChanged != null)
				{
					notifyDataContextChanged.DataContextChanged -= DataContextChanged;
				}
			}

			base.Dispose(disposing);
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			if (Data != null && Data[section] != null)
				return Data[section].Count;

			return base.RowsInSection(tableview, section);
		}

		public override int NumberOfSections(UITableView tableView)
		{
			if (Data != null)
				return Data.Keys.Count;

			return base.NumberOfSections(tableView);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			if (Data != null)
			{
				string cellIdentifier = "CellId";

				Cell = tableView.DequeueReusableCell(cellIdentifier) ?? NewCell(cellIdentifier, indexPath);
				
				var elementCell = Cell as UITableViewElementCell;
				elementCell.IndexPath = indexPath;

				var element = GetElement(indexPath);
				if (element == null)
					element = Root;

				element.Cell = Cell as UITableViewElementCell;
				element.Cell.Element = element;
				element.TableView = tableView;

				element.InitializeTheme();
				element.InitializeCell(tableView);
				
				UpdateCell(Cell, indexPath);

				return Cell;
			}

			return base.GetCell(tableView, indexPath);
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{			
			var cell = Cell as UITableViewElementCell;
			if (cell != null && cell.Element != null)
			{
				var sizable = cell.Element as ISizeable;
				if (sizable != null)
					return sizable.GetHeight(tableView, indexPath);
			}

			return tableView.RowHeight;
		}

		protected UITableViewCell NewCell(string cellId, NSIndexPath indexPath)
		{
			var element = GetElement(indexPath);
	
			if (element == null)
				element = Root;

			var cell = new UITableViewElementCell(Root.Theme.CellStyle, cellId, element) { IndexPath = indexPath };
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			
			return cell;
		}

		protected virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var updateable = Container.RootView as IUpdateable;
			if (updateable != null)
				updateable.UpdateCell(cell, indexPath);
		}
		
		private void DataContextChanged(object sender, DataContextChangedEventArgs e)
		{
			if (Container != null)
			{
				if (e.OldDataContext != null)
				{
					var notify = e.OldDataContext as INotifyCollectionChanged;
					if (notify != null)
					{
						notify.CollectionChanged -= DataCollectionChanged;
					}
				}

				if (e.NewDataContext != null)
				{
					var notify = e.NewDataContext as INotifyCollectionChanged;
					if (notify != null)
					{
						notify.CollectionChanged += DataCollectionChanged;
					}
				}

				DataCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

				Console.WriteLine("DataContextChanged");
			}	
		}

		private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Container != null)
			{
				var dataContext = Container.RootView as IDataContext;
				if (dataContext != null)
				{
					if (dataContext.DataContext is IList)
					{
						_Data = new Dictionary<int, IList>() { {0, dataContext.DataContext as IList} };
					}
					else if (dataContext.DataContext is IDictionary)
					{
						_Data = new Dictionary<int, IList>(dataContext.DataContext as IDictionary<int, IList>);		
					}
					else 
					{
						_Data = null;
					}
				}
				
				Container.TableView.ReloadData();
			}
		}
	}
}

