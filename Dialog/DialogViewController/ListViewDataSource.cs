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
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	
	using System.Collections.Generic;

	[Preserve(AllMembers = true)]
	public class ListViewDataSource : BaseDialogViewSource, IDataContext<IList>, IHandleDataContextChanged, ISearchBar, ITableViewStyle
	{
		private MemberInfo _SelectedItemsMember;
		private MemberInfo _SelectedItemMember;

		public IList SelectedItems { get; set; }
		public object SelectedItem { get; set; }
		
		public string SelectedItemMemberName { set { _SelectedItemMember = GetMemberFromView(value); } }
		public string SelectedItemsMemberName { set { _SelectedItemsMember = GetMemberFromView(value); } }

		public IList DataContext { get; set; }

		public bool IsSelectable { get; set; }
		public bool IsMultiselect { get; set; }
		public bool PopOnSelection { get; set; }

		public ListViewDataSource(DialogViewController controller, IList list, IEnumerable<Type> viewTypes) : base(controller, viewTypes)
		{	
			DataContext = list;

			var genericTypeDefinition = typeof(List<>).GetGenericTypeDefinition();
			var genericType = DataContext.GetType().GetGenericArguments().FirstOrDefault();
			Type[] generic = { genericType };		
			SelectedItems = Activator.CreateInstance(genericTypeDefinition.MakeGenericType(generic), new object[] { }) as IList;

			CellFactory = new TableCellFactory<UITableViewCell>("listCell");

			SelectedItemMemberName = "ItemSelected";
			SelectedItemsMemberName = "ItemsSelected";
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			return DataContext != null ? DataContext.Count : 0;
		}

		public override int NumberOfSections(UITableView tableView)
		{	
			return DataContext != null ? 1 : 0;
		}
		
		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			SelectedItem = DataContext[indexPath.Row]; 
			if (_SelectedItemMember != null)
			{
				_SelectedItemMember.SetValue(Controller.RootView, SelectedItem);
			}

			if (IsMultiselect)
			{	
				if (SelectedItems.Contains(SelectedItem))
				{
					SelectedItems.Remove(SelectedItem);
				}
				else
				{
					SelectedItems.Add(SelectedItem);
				}

				if (_SelectedItemsMember != null)
				{
					_SelectedItemsMember.SetValue(Controller.RootView, SelectedItems);
				}
			};

			if (Controller != null)
			{
				Controller.Selected(SelectedItem, indexPath);
			}

			if (PopOnSelection)
			{
				Controller.NavigationController.PopViewControllerAnimated(true);
			}
			

			Controller.ReloadData();
		}
	
		protected override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{	
			var selectedIndex = DataContext.IndexOf(SelectedItem);

			if (IsMultiselect && indexPath.Row == selectedIndex)
			{
				cell.Accessory = cell.Accessory == UITableViewCellAccessory.None ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			}

			if (IsSelectable && !IsMultiselect)
			{
				cell.Accessory = (indexPath.Row == selectedIndex) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			}

			if (Views.ContainsKey(cell))
			{
				var views = Views[cell];
	
				if (views.Count > 0)
				{
					foreach (var view in views)
					{
						var dc = view as IDataContext<object>;
						if (dc != null)
						{
							dc.DataContext = DataContext[indexPath.Row];
						}
		
						var updateable = view as IUpdateable;
						if (updateable != null)
						{
							updateable.UpdateCell(cell, indexPath);
						}
					}
				}
				else
				{
					cell.TextLabel.Text = DataContext[indexPath.Row].ToString();
				}
			}
		}

		public void HandleNotifyDataContextChangedDataContextChanged(object sender, DataContextChangedEventArgs e)
		{
			DataContext = e.NewDataContext as IList;

			SelectedItems.Clear();

			Controller.UpdateSource();
		}
	}
}
