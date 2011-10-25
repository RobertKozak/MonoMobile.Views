// 
//  TableCellFactory.cs
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
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;

namespace MonoMobile.Views
{
	public class TableCellFactory<T> where T : UITableViewCell
	{
		private string _NibName;

		public string CellId { get; set; }
    
		public TableCellFactory(string cellId, string nibName)
		{
			CellId = cellId;
			_NibName = nibName;
		}
    
		public TableCellFactory(string cellId)
		{
			CellId = cellId;
		}
		
		public TableCellFactory()
		{
		}

		public T GetCell(UITableView tableView, Func<T> newCell)
		{
			return GetCell(tableView, string.Empty, newCell);
		}

		public T GetCell(UITableView tableView, string nibName, Func<T> newCell)
		{
			_NibName = nibName;
 
			var cell = tableView.DequeueReusableCell(CellId) as T;
            
			if (cell == null)
			{
				if (newCell != null)
				{
					cell = newCell() as T;
				}
				else
				{
					cell = Activator.CreateInstance<T>();
				}

				if (!string.IsNullOrEmpty(_NibName))
				{
					var views = NSBundle.MainBundle.LoadNib(_NibName, cell, null);
					var nibCell = Runtime.GetNSObject(views.ValueAt(0));
					cell = nibCell as T;
				}
			}
        							
			return cell;
		}
	}
}
