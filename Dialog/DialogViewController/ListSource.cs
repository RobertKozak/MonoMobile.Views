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
	public class ListSource : BaseDialogViewSource, IHandleDataContextChanged, ISearchBar, ITableViewStyle, IActivation
	{
		private readonly NSString _CellId = new NSString("listCell");
		private ListSource _NavigationSource;

		private MemberInfo _SelectedItemsMember;
		private MemberInfo _SelectedItemMember;

		public IList SelectedItems { get; set; }
		public object SelectedItem { get; set; }
		
		public string SelectedItemMemberName { set { _SelectedItemMember = GetMemberFromView(value); } }
		public string SelectedItemsMemberName { set { _SelectedItemsMember = GetMemberFromView(value); } }
				
		public UnselectionBehavior UnselectionBehavior { get; set; }

		public bool IsSelectable { get; set; }
		public bool IsMultiselect { get; set; }
		public bool PopOnSelection { get; set; }
		public bool HideCaptionOnSelection { get; set; }
		
		public ListSource(DialogViewController controller, IList list, IEnumerable<Type> viewTypes) : base(controller)
		{	
			Sections = new Dictionary<int, Section>();
			var section = new Section(controller) { DataContext = list };
			section.ViewTypes.Add(_CellId, viewTypes.ToList());
			
			Sections.Add(0, section);

			var genericTypeDefinition = typeof(List<>).GetGenericTypeDefinition();
			var genericType = list.GetType().GetGenericArguments().FirstOrDefault();
			Type[] generic = { genericType };		
			SelectedItems = Activator.CreateInstance(genericTypeDefinition.MakeGenericType(generic), new object[] { }) as IList;

			CellFactory = new TableCellFactory<UITableViewCell>(_CellId);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
			}

			base.Dispose(disposing);
		}
	
		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = CellFactory.GetCell(tableView, indexPath, _CellId, NibName, (cellId, idxPath) => NewCell(cellId, idxPath));

			UpdateCell(cell, indexPath);

			return cell;
		}

		protected override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{	
			base.UpdateCell(cell, indexPath);

			SetSelectionAccessory(cell, indexPath);

			var section = Sections[indexPath.Section];
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
							dc.DataContext = GetSectionData(indexPath.Section)[indexPath.Row];
						}
		
						var updateable = view as IUpdateable;
						if (updateable != null)
						{
							updateable.UpdateCell(cell, indexPath);
						}
					}
				}
				else if (IsRoot)
				{
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
					cell.TextLabel.Text = Caption;
					if (IsMultiselect)
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
								cell.DetailTextLabel.Text = SelectedItem.ToString();
						}
					}
				}
				else
				{
					cell.TextLabel.Text = GetSectionData(indexPath.Section)[indexPath.Row].ToString();
				}
			}
		}

		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{			
			if (!IsRoot)
			{
				SelectedItem = GetSectionData(indexPath.Section)[indexPath.Row]; 

				if (SelectedItems.Contains(SelectedItem))
				{
					SelectedItems.Remove(SelectedItem);

					switch (UnselectionBehavior)
					{
						case UnselectionBehavior.SetSelectedToCurrentValue :
							break;
						case UnselectionBehavior.SetSelectedToNull :
							SelectedItem = null;
							break;
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
				}
	
				if (PopOnSelection && !(IsNavigateable || IsRoot || IsMultiselect))
				{
					Controller.NavigationController.PopViewControllerAnimated(true);
				}
				
				if (IsSelectable || IsMultiselect)
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
			
			if (IsNavigateable || IsRoot)
			{
				if (NavigationView != null)
				{
					NavigateToView(NavigationView);
					//return;
				}
			
				if (IsRoot)
				{
					NavigateToList();
				}
			}
		}
		
		public void NavigateToView(Type viewType)
		{
			if (NavigationView != null)
			{
				var view = Activator.CreateInstance(NavigationView) as UIView;

				var dc = view as IDataContext<object>;
				if (dc != null)
				{
					dc.DataContext = SelectedItem;
				}
				
				MonoMobileApplication.PushView(view);
			}
		}

		public void NavigateToList()
		{
			var section = Sections[0];
			var dvc = new DialogViewController(Caption, Controller.RootView, true);
			if (_NavigationSource == null)
				_NavigationSource = new ListSource(dvc, GetSectionData(0), section.ViewTypes[_CellId]);

			_NavigationSource.Controller = dvc;

			dvc.ToolbarButtons = null;
			dvc.NavbarButtons = null;

			_NavigationSource.SelectedItem = SelectedItem;
			_NavigationSource.SelectedItems = SelectedItems;
			_NavigationSource.UnselectionBehavior = UnselectionBehavior;

			_NavigationSource.IsNavigateable = false;
			_NavigationSource.IsRoot = false;
			_NavigationSource.IsMultiselect = IsMultiselect;
			_NavigationSource.IsSelectable = IsSelectable;
			_NavigationSource.CellFactory = CellFactory;
			_NavigationSource.NavigationView = null;
			_NavigationSource.PopOnSelection = PopOnSelection;
			_NavigationSource.NibName = NibName;

			_NavigationSource.TableViewStyle= TableViewStyle;
	
			_NavigationSource.IsSearchbarHidden = IsSearchbarHidden;
			_NavigationSource.EnableSearch = EnableSearch;
			_NavigationSource.IncrementalSearch = IncrementalSearch;
			_NavigationSource.SearchPlaceholder = SearchPlaceholder;
			_NavigationSource.SearchCommand = SearchCommand;

			_NavigationSource.SelectedAccessoryViewType = SelectedAccessoryViewType;
			_NavigationSource.UnselectedAccessoryViewType = UnselectedAccessoryViewType;
			
			dvc.TableView.Source = _NavigationSource;
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
			if (_NavigationSource != null)
			{
				SelectedItem = _NavigationSource.SelectedItem;
				SelectedItems = _NavigationSource.SelectedItems;
				SetItems();
			}
			else
			{
				Controller.UpdateSource();
			}

			Controller.ReloadData();
		}

		public void Deactived()
		{
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
				var selectedIndex = GetSectionData(indexPath.Section).IndexOf(SelectedItem);
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
					foreach (var item in SelectedItems)
					{
						selectedIndex = GetSectionData(indexPath.Section).IndexOf(item);
						
						if (selectedAccessoryView != null)
						{
							cell.AccessoryView = selectedIndex == indexPath.Row ? selectedAccessoryView : cell.AccessoryView;
						}
						else
							cell.Accessory = selectedIndex == indexPath.Row ? UITableViewCellAccessory.Checkmark : cell.Accessory;
					}
				}
			}
		}
	}
}
