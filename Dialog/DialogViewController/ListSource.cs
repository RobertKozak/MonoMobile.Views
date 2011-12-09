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
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	
	
	[Preserve(AllMembers = true)]
	public class ListSource : BaseDialogViewSource, ISearchBar, IActivation
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
		
		public bool IsMultiselect { get; set; }
		public bool PopOnSelection { get; set; }
		public bool ReplaceCaptionWithSelection { get; set; }
		public DisplayMode DisplayMode { get; set; }

		public SelectionAction SelectionAction { get; set; }
		public bool IsCollapsed { get; set; }
		public IList CollapsedList { get; set; }

		public Type NavigationViewType { get; set; }
		public object NavigationView { get; set; }
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

//SelectionDisplayMode = SelectionDisplayMode.Collapsed;
//CollapsedList = new List<object>();
//			foreach(var item in Sections[0].DataContext)
//			{
//				CollapsedList.Add(item);
//			}
//			Sections[0].DataContext.Clear();
//
//IsCollapsed = true;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach(var section in Sections.Values)
				{
					section.DataContext = null;
				}
			}

			base.Dispose(disposing);
		}

		public override int RowsInSection(UITableView tableview, int sectionIndex)
		{
			if (IsRootCell || IsCollapsed)
			{
				return 1;
			}
			
			var numberOfRows = IsCollapsed ? 0 : 0;
			if (Sections.ContainsKey(sectionIndex))
			{
				numberOfRows += Sections[sectionIndex].NumberOfRows;
			}

			return Sections != null ? numberOfRows : 0;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Sections != null ? Sections.Count : 0;
		}
	
		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (MemberData != null && MemberData.RowHeight != 0)
				return MemberData.RowHeight;

			return base.GetHeightForRow(tableView, indexPath);
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			indexPath = NSIndexPath.FromRowSection(indexPath.Row, 0);

			var cell = CellFactory.GetCell(tableView, indexPath, CellId, NibName, (cellId, idxPath) => NewListCell(cellId, idxPath));

			UpdateCell(cell, indexPath);

			return cell;
		}
		
		private UITableViewCell NewListCell(NSString cellId, NSIndexPath indexPath)
		{
			var cellStyle = UITableViewCellStyle.Subtitle;		

			var section = Sections[indexPath.Section];
		
			IList<Type> viewTypes = null;

			var key = cellId.ToString();

			if (section.ViewTypes != null && section.ViewTypes.ContainsKey(key))
			{
				viewTypes = section.ViewTypes[key];
			}
 
			var cell = new ComposableViewListCell(cellStyle, cellId, indexPath, viewTypes, this);

			return cell;
		}
		
		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{	
			var composableListCell = cell as ComposableViewListCell;
			if (composableListCell != null)
			{
				composableListCell.IndexPath = indexPath;
			}
			
			Type dataType = null;
			var sectionData = GetSectionData(0);
			if (sectionData.Count > 0)
			{
				dataType = sectionData[0].GetType();
			}

			if (DisplayMode != DisplayMode.RootCell)
			{
				if (dataType != null && (dataType.IsPrimitive || dataType.IsEnum) && (SelectionAction == SelectionAction.NavigateToView || SelectionAction == SelectionAction.Custom))
				{
					IsSelectable = false;
					SelectionAction = SelectionAction.Custom;
				}
			}
			else
			{
				if (dataType != null && ((dataType.IsPrimitive || dataType == typeof(string))) && (SelectionAction == SelectionAction.NavigateToView))
				{
					IsNavigable = sectionData.Count > 1;
					SelectionAction = SelectionAction.Selection;
				}
			}

			base.UpdateCell(cell, indexPath);

			cell.SelectionStyle = IsNavigable ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;  
			cell.SelectionStyle = IsSelectable ? UITableViewCellSelectionStyle.None : cell.SelectionStyle;  

			cell.SelectionStyle = SelectionAction == SelectionAction.Custom ? UITableViewCellSelectionStyle.Blue : cell.SelectionStyle;
			
			SetSelectionAccessory(cell, indexPath);

			cell.SetNeedsDisplay();
		}

		public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			var updated = false;

			var sectionData = GetSectionData(0);
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
							cell.SetNeedsDisplay();
							updated = true;
						}
					}
				}
			}
		
			// Do default since no views have done an update
			if (!updated)
			{
				if (IsRootCell)
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
							if (ReplaceCaptionWithSelection)
								cell.TextLabel.Text = SelectedItem.ToString();
							else
								if (cell.DetailTextLabel != null)
									cell.DetailTextLabel.Text = SelectedItem.ToString();
						}
					}
				}
				else
				{
					if (sectionData.Count > 0)
						cell.TextLabel.Text = sectionData[indexPath.Row].ToString();
				}
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{	
			object data = null;

			if (DisplayMode == DisplayMode.Collapsable)
			{
				var section = Sections[0];
				var list = section.DataContext;	
				var indexPaths = new List<NSIndexPath>();
				
				if (IsCollapsed)
				{
					foreach(var item in CollapsedList)
					{
						var row = list.Add(item);
						indexPaths.Add(NSIndexPath.FromRowSection(row, section.Index));
					}
					IsCollapsed  = false;
					CollapsedList.Clear();
					Controller.TableView.InsertRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
				}
				else
				{
					foreach(var item in list)
					{
						var row = CollapsedList.Add(item);
						indexPaths.Add(NSIndexPath.FromRowSection(row, section.Index));
					}
					IsCollapsed = true;
					list.Clear();
					Controller.TableView.DeleteRows(indexPaths.ToArray(), UITableViewRowAnimation.Top);
				}

			//	Controller.TableView.ReloadRows(new NSIndexPath[] { NSIndexPath.FromRowSection(0, section.Index) }, UITableViewRowAnimation.Fade);
				return;
			}
			if (!IsRootCell)
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
					if (PopOnSelection && !(!IsSelectable || IsNavigable || IsMultiselect))
					{
						Controller.NavigationController.PopViewControllerAnimated(true);
					}
					
					if (IsSelectable || IsMultiselect || !IsNavigable)
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

				data = SelectedItem;
			}
			else
				data = GetSectionData(0);
			
			if (SelectionAction == SelectionAction.None)
				return;

			if (SelectionAction == SelectionAction.Custom)
			{
				IsSelectable = false;
				IsMultiselect = false;
				IsNavigable = false;
				Controller.Selected(SelectedItem, indexPath);
				return;
			}

			var dataType = data.GetType();
			if ((dataType.IsPrimitive || dataType.IsEnum) && SelectionAction == SelectionAction.NavigateToView)
			{
				IsSelectable = true;
				SelectionAction = SelectionAction.Selection;
			}

			if (IsNavigable && (data is IEnumerable && (!(data is string))))
			{
				NavigateToList();
				return;
			}			
			
			if (SelectionAction == SelectionAction.NavigateToView && (SelectedItem != null))
			{
				NavigateToView();
			}
		}
		
		public void NavigateToView()
		{
			var viewType = NavigationViewType;
			if (viewType == null)
			{
				viewType = ViewContainer.GetView(SelectedItem.GetType());
			}

			if (viewType != null)
			{
				var disposable = NavigationView as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}

				NavigationView = Activator.CreateInstance(viewType);
				
				var dc = NavigationView as IDataContext<object>;
				if (dc != null)
				{
					dc.DataContext = SelectedItem;
				}
				else
				{
					NavigationView = SelectedItem;
				}
				
				var initializable = NavigationView as IInitializable;
				if (initializable != null)
				{
					initializable.Initialize();
				}

				Caption = SelectedItem.ToString();
	
				var dvc = new DialogViewController(Caption, NavigationView, Controller.Theme, true);
				Controller.NavigationController.PushViewController(dvc, true);
			}
		}

		public void NavigateToList()
		{
			var section = Sections[0];
			var data = GetSectionData(0);

			if (string.IsNullOrEmpty(Caption))
			{
				Caption = data.ToString();
			}
			
			var dvc = new DialogViewController(Caption, null, Controller.Theme, true);
			dvc.ToolbarButtons = null;
			dvc.NavbarButtons = null;

			if (NavigationSource == null)
				NavigationSource = new ListSource(dvc, data, section.ViewTypes[CellId]);
			
			NavigationSource.SelectionAction = SelectionAction;

			NavigationSource.IsSelectable = true;
			NavigationSource.NavigationViewType = null;
			
			var viewType = NavigationViewType;
			if (viewType == null)
			{
				var genericType = data.GetType().GetGenericArguments().FirstOrDefault();
				viewType = ViewContainer.GetView(genericType);
			}
		
			if (viewType != null)
			{
				NavigationSource.IsNavigable = viewType == typeof(ObjectCellView<object>);
				NavigationSource.NavigationViewType = viewType;
			}

			NavigationSource.IsNavigable = !PopOnSelection && NavigationSource.IsNavigable && SelectionAction != SelectionAction.Custom;

			NavigationSource.CellFactory = CellFactory;

			NavigationSource.SelectedItem = SelectedItem;
			NavigationSource.SelectedItems = SelectedItems;
			NavigationSource.UnselectionBehavior = UnselectionBehavior;

			NavigationSource.IsMultiselect = IsMultiselect;
			NavigationSource.IsSelectable = IsSelectable;
	
			if (data.Count > 0 && (data[0].GetType().IsPrimitive || data[0].GetType().IsEnum))
				NavigationSource.IsSelectable = true;

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
			GetItems();

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
		
		private void GetItems()
		{		
			if (_SelectedItemMember != null)
			{
				var item = _SelectedItemMember.GetValue(Controller.RootView);
				if (item != null)
					SelectedItem = item;
			}
	
			if (IsMultiselect)
			{		
				if (_SelectedItemsMember != null)
				{
					var items = _SelectedItemsMember.GetValue(Controller.RootView) as IList;
					if (items != null)
						SelectedItems = items; 
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
			var sectionData = GetSectionData(0);
			cell.Accessory = SelectionAction == SelectionAction.Custom ? UITableViewCellAccessory.None : cell.Accessory;
			
			if (SelectionAction != SelectionAction.NavigateToView)
			{
				cell.Accessory = sectionData != null && sectionData.Count > 1 ? cell.Accessory : UITableViewCellAccessory.None;
			}

			base.SetSelectionAccessory(cell, indexPath);
			
			if (IsSelectable)
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
