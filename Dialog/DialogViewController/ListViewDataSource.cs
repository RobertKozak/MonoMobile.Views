// 
//  ListViewDataSource.cs
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
namespace MonoMobile.Views
{	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	

	[Preserve(AllMembers = true)]
	public abstract class ListViewDataSource<T> : UITableViewSource
	{
		private TableCellFactory<UITableViewCell> cellFactory = new TableCellFactory<UITableViewCell>();
		private readonly string _Id;

		protected DialogViewController Controller;
		protected UITableViewCell Cell { get; set; }
		protected IList<T> DataContext { get; set; }
		
		public string NibName { get; set; }
		
		public ListViewDataSource(string cellId, DialogViewController controller) : base()
		{
			_Id = cellId;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		public abstract void InitializeData(object data);

		public override int RowsInSection(UITableView tableview, int section)
		{
			if (DataContext != null)
				return DataContext.Count;

			return 0;

		}

		public override int NumberOfSections(UITableView tableView)
		{
			if (DataContext != null)
				return DataContext.Count;
		
			return 0;
		}
		
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			cellFactory.CellId = _Id;
			Cell = cellFactory.GetCell(tableView, NibName, ()=>NewCell());

			UpdateCell(Cell, indexPath);

			return Cell;
		}

		protected UITableViewCell NewCell()
		{
			var cellStyle = UITableViewCellStyle.Subtitle;
			var cell = new UITableViewCell(cellStyle, _Id) { };
			
			return cell;
		}
		
		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			Controller.Selected(indexPath);
		}

		protected virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var selectable = this as ISelectable;
			Cell.SelectionStyle = selectable != null ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
		}
	}
}

