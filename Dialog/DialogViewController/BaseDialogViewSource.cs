// 
//  BaseDialogViewSource.cs
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
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public abstract class BaseDialogViewSource : UITableViewSource, ISearchBar
	{
		protected IEnumerable<Type> ViewTypes;
		protected IDictionary<UITableViewCell, IList<UIView>> Views;
		protected TableCellFactory<UITableViewCell> CellFactory;
		protected DialogViewController Controller;

		protected UITableViewCell Cell { get; set; }

		protected string NibName { get; set; }

		public UITableViewStyle TableViewStyle { get; set; }
			
		public bool IsSearchbarHidden { get; set; }
		public bool EnableSearch { get; set; }
		public bool IncrementalSearch { get; set; }
		public string SearchPlaceholder { get; set; }
		public SearchCommand SearchCommand { get; set; }
		
		public bool IsRoot { get; set; }
		public bool IsNavigateable { get; set; }
		public Type NavigationView { get; set; }

		public string Caption { get; set; }
		
		public BaseDialogViewSource(DialogViewController controller, IEnumerable<Type> viewTypes)
		{
			Controller = controller;
			ViewTypes = viewTypes;

			Views = new Dictionary<UITableViewCell, IList<UIView>>();
		}

		protected virtual UITableViewCell NewCell(NSString cellId, NSIndexPath indexPath)
		{
			var cellStyle = UITableViewCellStyle.Value1;

			var views = new List<UIView>();

			if (ViewTypes != null)
			{
				foreach (var viewType in ViewTypes)
				{
					var view = Activator.CreateInstance(viewType) as UIView;
			
					var initializeCell = view as IInitializeCell;
					if (initializeCell != null)
					{
						if (cellStyle == UITableViewCellStyle.Value1)
						{
							cellStyle = initializeCell.CellStyle;
						}
					}

					views.Add(view);
				}
			}
			
			var cell = new UITableViewCell(cellStyle, cellId) { };
			
			cell.TextLabel.BackgroundColor = UIColor.Clear;
			cell.DetailTextLabel.BackgroundColor = UIColor.Clear;
			cell.TextLabel.AdjustsFontSizeToFitWidth = true;
			cell.DetailTextLabel.AdjustsFontSizeToFitWidth = true;

			var selectable = this as ISelectable;
			cell.SelectionStyle = selectable != null ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.Blue;
			
			if (views.Count > 0)
			{
				cell.ContentView.AutosizesSubviews = true;
			}

			Views.Add(cell, views);

			foreach (var view in views)
			{
				var contentView = view as ICellContent;
				if (contentView != null)
				{
					contentView.Cell = cell;

					view.Frame = cell.ContentView.Bounds;
					view.Tag = 1;
					view.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
					cell.ContentView.Add(view);
				}

				var createCell = view as IInitializeCell;
				if (createCell != null)
				{
					createCell.InitializeCell(cell, indexPath);
				}
			}
			
			cell.TextLabel.Text = Caption;

			return cell;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			Cell = CellFactory.GetCell(tableView, indexPath, NibName, (cellId, idxPath) => NewCell(cellId, idxPath));

			UpdateCell(Cell, indexPath);

			return Cell;
		}

		protected virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.AccessoryView = null;

			cell.Accessory = IsRoot || IsNavigateable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
		}

		protected MemberInfo GetMemberFromView(string memberName)
		{
			var memberInfo = Controller.RootView.GetType().GetMember(memberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault();

			return memberInfo;
		}
	}
}


