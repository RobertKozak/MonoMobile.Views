// 
//  ListSource.cs
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
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
				
	[Preserve(AllMembers = true)]
	public class ListSource : BaseDialogViewSource, IHandleDataContextChanged, ISearchBar, ITableViewStyle, IActivation
	{
		private MemberInfo _SelectedItemsMember;
		private MemberInfo _SelectedItemMember;

		public readonly NSString CellId;
		public MemberData MemberData { get; set; }

		public IList SelectedItems { get; set; }
		public object SelectedItem { get; set; }
		
		public string SelectedItemMemberName { set { _SelectedItemMember = GetMemberFromView(value); } }
		public string SelectedItemsMemberName { set { _SelectedItemsMember = GetMemberFromView(value); } }
				
		public UnselectionBehavior UnselectionBehavior { get; set; }

		public bool IsSelectable { get; set; }
		public bool IsMultiselect { get; set; }
		public bool PopOnSelection { get; set; }
		public bool HideCaptionOnSelection { get; set; }

		public Type NavigationViewType { get; set; }
		public ListSource NavigationSource { get; set; }
		public bool IsModal {get; set; }
		public UIModalTransitionStyle ModalTransitionStyle { get; set; }

		public ListSource(DialogViewController controller, IList list, IEnumerable<Type> viewTypes) : base(controller)
		{	
			Sections = new Dictionary<int, Section>();
			var section = new Section(controller) { DataContext = list };

			IList<Type> viewTypesList = null;
			if (viewTypes != null)
				viewTypesList = viewTypes.ToList();

			var genericType = list.GetType().GetGenericArguments().FirstOrDefault();
			CellId = new NSString(genericType.ToString());

			section.ViewTypes.Add(CellId, viewTypesList);
			
			Sections.Add(0, section);

			SelectedItems = list.GetType().CreateGenericListFromEnumerable(null);

			CellFactory = new TableCellFactory<UITableViewCell>(CellId);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach(var section in Sections.Values)
				{
					var notifyCollectionChanged = section.DataContext as INotifyCollectionChanged;
					if (notifyCollectionChanged != null)
					{
						notifyCollectionChanged.CollectionChanged -= HandleCollectionChanged;
					}
					section.DataContext = null;
				}
			}

			base.Dispose(disposing);
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			if (IsRoot)
			{
				return 1;
			}

			return Sections != null ? Sections[section].NumberOfRows : 0;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Sections != null ? Sections.Count : 0;
		}
	
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			indexPath = NSIndexPath.FromRowSection(indexPath.Row, 0);

			var cell = CellFactory.GetCell(tableView, indexPath, CellId, NibName, (cellId, idxPath) => NewCell(cellId, idxPath));

			UpdateCell(cell, indexPath);

			return cell;
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{	
			base.UpdateCell(cell, indexPath);
			
			cell.SelectionStyle = IsRoot || IsNavigateable ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;  

			SetSelectionAccessory(cell, indexPath);

			var section = Sections[0];
			if (section.Views.ContainsKey(cell))
			{
				var views = section.Views[cell];
	
				if (views.Count > 0)
				{
					foreach (var view in views)
					{
						var dc = view as IDataContext<object>;
						if (dc != null)
						{
							dc.DataContext = GetSectionData(0)[indexPath.Row];
						}
		
						var updateable = view as IUpdateable;
						if (updateable != null)
						{
							updateable.UpdateCell(cell, indexPath);
						}
					}
				}
			}
			
			if (IsRoot)
			{
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				cell.TextLabel.Text = Caption;
				if (IsMultiselect && cell.DetailTextLabel != null)
				{
					cell.DetailTextLabel.Text = SelectedItems.Count.ToString();
				}
				else
				{
					if (SelectedItem != null)
					{
						if (HideCaptionOnSelection)
							cell.TextLabel.Text = SelectedItem.ToString();
						else
							if (cell.DetailTextLabel != null)
								cell.DetailTextLabel.Text = SelectedItem.ToString();
					}
				}
			}
			else
			{
				cell.TextLabel.Text = GetSectionData(0)[indexPath.Row].ToString();
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{			
			if (!IsRoot)
			{
				SelectedItem = GetSectionData(0)[indexPath.Row]; 

				if (SelectedItems.Contains(SelectedItem))
				{
					SelectedItems.Remove(SelectedItem);

					switch (UnselectionBehavior)
					{
						case UnselectionBehavior.SetSelectedToCurrentValue : break;
						case UnselectionBehavior.SetSelectedToNull : SelectedItem = null; break;
						case UnselectionBehavior.SetSelectedToPreviousValueOrNull :
						{
							if (SelectedItems.Count > 0)
							{
								SelectedItem = SelectedItems[SelectedItems.Count - 1];
							}
							else
							{
								SelectedItem = null;
							}

							break;
						}
					}

				}
				else
				{
					SelectedItems.Add(SelectedItem);
				}

				SetItems();

				if (Controller != null)
				{
					Controller.Selected(SelectedItem, indexPath);
	
					if (PopOnSelection && !(IsNavigateable || IsRoot || IsMultiselect))
					{
						Controller.NavigationController.PopViewControllerAnimated(true);
					}
					
					if (IsSelectable || IsMultiselect || !IsRoot)
					{
						Controller.ReloadData();
					}
					else
					{
						new Wait(new TimeSpan(0, 0, 0, 0, 300), () => 
						{
							Controller.ReloadData();
						});
					}
				}
			}

			var data = GetSectionData(0);

			if (IsRoot && (data is IEnumerable || data is Enum))
			{
				NavigateToList();
				return;
			}			
			
			if (IsNavigateable && (SelectedItem != null || NavigationViewType != null))
			{
				NavigateToView();
			}
		}
		
		public void NavigateToView()
		{
			object view = null;
			var viewType = NavigationViewType;
					
			if (viewType != null)
			{
				view = Activator.CreateInstance(viewType);
				
				var dc = view as IDataContext<object>;
				if (dc != null)
				{
					dc.DataContext = SelectedItem;
				}
				else
				{
					view = SelectedItem;
				}
			}
			
			if (Caption == null)
				Caption = SelectedItem.ToString();

			var dvc = new DialogViewController(Caption, view, true);
			Controller.NavigationController.PushViewController(dvc, true);
		}

		public void NavigateToList()
		{
			var section = Sections[0];
			var data = GetSectionData(0);
			var type = data.GetType();

			var dvc = new DialogViewController(Caption, null, true);
			dvc.ToolbarButtons = null;
			dvc.NavbarButtons = null;

			if (NavigationSource == null)
				NavigationSource = new ListSource(dvc, data, section.ViewTypes[CellId]);
			
			NavigationSource.IsNavigateable = false;
			NavigationSource.IsRoot = false;
			NavigationSource.NavigationViewType = null;
			
			var viewType = NavigationViewType;
			if (viewType == null)
			{
				var genericType = data.GetType().GetGenericArguments().FirstOrDefault();
				viewType = ViewContainer.GetView(genericType);
			}
		
			if (viewType != null)
			{
				NavigationSource.IsNavigateable = IsNavigateable;
				NavigationSource.NavigationViewType = viewType;
			}

			NavigationSource.CellFactory = CellFactory;

			NavigationSource.SelectedItem = SelectedItem;
			NavigationSource.SelectedItems = SelectedItems;
			NavigationSource.UnselectionBehavior = UnselectionBehavior;

			NavigationSource.IsMultiselect = IsMultiselect;
			NavigationSource.IsSelectable = IsSelectable;

			NavigationSource.PopOnSelection = PopOnSelection;
			NavigationSource.NibName = NibName;

			NavigationSource.TableViewStyle = TableViewStyle;
	
			NavigationSource.IsSearchbarHidden = IsSearchbarHidden;
			NavigationSource.EnableSearch = EnableSearch;
			NavigationSource.IncrementalSearch = IncrementalSearch;
			NavigationSource.SearchPlaceholder = SearchPlaceholder;
			NavigationSource.SearchCommand = SearchCommand;

			NavigationSource.SelectedAccessoryViewType = SelectedAccessoryViewType;
			NavigationSource.UnselectedAccessoryViewType = UnselectedAccessoryViewType;
			
			NavigationSource.Controller = dvc;
			dvc.TableView.Source = NavigationSource;
			Controller.NavigationController.PushViewController(dvc, true);
		}

		public void HandleNotifyDataContextChangedDataContextChanged(object sender, DataContextChangedEventArgs e)
		{
			SetSectionData(0, e.NewDataContext as IList);

			SelectedItems.Clear();

			Controller.UpdateSource();
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return false;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return  UITableViewCellEditingStyle.None;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
		}

		public void Activated()
		{
			if (NavigationSource != null)
			{
				SelectedItem = NavigationSource.SelectedItem;
				SelectedItems = NavigationSource.SelectedItems;
				SetItems();
			}
			else
			{
				Controller.UpdateSource();
			}

			Controller.ReloadData();
		}

		public void Deactivated()
		{
		}
		
		public void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{	
			var section = Sections[0];

			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					section.DataContext.Add(item);
					section.SetNumberOfRows();
					
					var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(section.DataContext.Count - 1, section.Index) };
					Controller.TableView.InsertRows(indexPaths, UITableViewRowAnimation.Fade);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var item in e.OldItems)
				{
					var row = section.DataContext.IndexOf(item);
					section.DataContext.Remove(item);
					section.SetNumberOfRows();
					
					var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
					Controller.TableView.DeleteRows(indexPaths, UITableViewRowAnimation.Fade);
				}
			}
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				section.DataContext.Clear();
				section.SetNumberOfRows();
				Controller.TableView.ReloadSections(NSIndexSet.FromIndex(section.Index), UITableViewRowAnimation.Automatic);
			}
			if (e.Action == NotifyCollectionChangedAction.Move)
			{
				var index = 0;
				foreach (var item in e.NewItems)
				{
					var row = e.OldStartingIndex;
					section.DataContext.RemoveAt(row);
					section.SetNumberOfRows();
					
					var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
					Controller.TableView.DeleteRows(indexPaths, UITableViewRowAnimation.Bottom);

					section.DataContext.Add(item);
					section.SetNumberOfRows();
					
					indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(section.DataContext.Count - 1, section.Index) };
					Controller.TableView.InsertRows(indexPaths, UITableViewRowAnimation.Top);
					
					index++;
				}				
			}
			if (e.Action == NotifyCollectionChangedAction.Replace)
			{
				var index = 0;
				foreach (var item in e.NewItems)
				{
					var row = e.NewStartingIndex + index;
					section.DataContext[e.NewStartingIndex + index] = item;
					section.SetNumberOfRows();
					
					var indexPaths = new NSIndexPath[] { NSIndexPath.FromRowSection(row, section.Index) };
					Controller.TableView.ReloadRows(indexPaths, UITableViewRowAnimation.Fade);
				}
			}
		}

		private void SetItems()
		{
			if (_SelectedItemMember != null)
			{
				_SelectedItemMember.SetValue(Controller.RootView, SelectedItem);
			}
	
			if (IsMultiselect)
			{		
				if (_SelectedItemsMember != null)
				{
					_SelectedItemsMember.SetValue(Controller.RootView, SelectedItems);
				}
			}
		}

		protected override void SetSelectionAccessory(UITableViewCell cell, NSIndexPath indexPath)
		{
			base.SetSelectionAccessory(cell, indexPath);
			
			if (!IsNavigateable)
			{	
				var selectedIndex = GetSectionData(0).IndexOf(SelectedItem);
				UIView selectedAccessoryView = SelectedAccessoryViews.Count > 0 ? SelectedAccessoryViews[cell] : null;
				UIView unselectedAccessoryView = UnselectedAccessoryViews.Count > 0 ? UnselectedAccessoryViews[cell] : null;

				if (selectedAccessoryView != null)
				{
					cell.AccessoryView = selectedIndex == indexPath.Row ? selectedAccessoryView : unselectedAccessoryView;
				}
				else
				{
					cell.Accessory = selectedIndex == indexPath.Row ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				}
				
				if (IsMultiselect)
				{
					if (!SelectedItems.Contains(SelectedItem))
					{
						cell.AccessoryView = null;
						cell.Accessory = UITableViewCellAccessory.None;
					}

					foreach (var item in SelectedItems)
					{
						selectedIndex = GetSectionData(0).IndexOf(item);
						
						if (selectedIndex != indexPath.Row) continue;

						if (selectedAccessoryView != null)
						{
							cell.AccessoryView = selectedAccessoryView;
						}
						else
							cell.Accessory = UITableViewCellAccessory.Checkmark;
					}
				}
			}
		}
	}
}
