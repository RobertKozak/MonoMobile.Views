//using System.Collections;
//using System.Linq;
//using System.Reflection;
//using System.Collections.Generic;
//// 
////  EnumView.cs
//// 
////  Author:
////    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
//// 
////  Copyright 2011, Nowcom Corporation.
//// 
////  Code licensed under the MIT X11 license
//// 
////  Permission is hereby granted, free of charge, to any person obtaining
////  a copy of this software and associated documentation files (the
////  "Software"), to deal in the Software without restriction, including
////  without limitation the rights to use, copy, modify, merge, publish,
////  distribute, sublicense, and/or sell copies of the Software, and to
////  permit persons to whom the Software is furnished to do so, subject to
////  the following conditions:
//// 
////  The above copyright notice and this permission notice shall be
////  included in all copies or substantial portions of the Software.
//// 
////  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
////  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
////  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
////  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
////  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
////  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
////  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//// 
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Drawing;
//	using MonoTouch.UIKit;
//	using MonoTouch.Foundation;
//	
//	[Preserve(AllMembers = true)]
//	public class EnumView : CellView, ISelectable
//	{
//		private MemberInfo _SelectedItemsMember;
//		private MemberInfo _SelectedItemMember;
//		
//		private ListSource _NavigationSource;
//
//		protected IDictionary<UITableViewCell, UIView> SelectedAccessoryViews;
//		protected IDictionary<UITableViewCell, UIView> UnselectedAccessoryViews;
//		
//		public Type SelectedAccessoryViewType { get; set; }
//		public Type UnselectedAccessoryViewType { get; set; }
//
//		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }
//		
//		public UIModalTransitionStyle TransitionStyle { get; set; }
//		public bool IsModel { get; set; }
//		public Type ViewType { get; set; }
//
//		public IList SelectedItems { get; set; }
//		public object SelectedItem { get; set; }
//		
//		public string SelectedItemMemberName { set { _SelectedItemMember = GetMemberFromView(value); } }
//		public string SelectedItemsMemberName { set { _SelectedItemsMember = GetMemberFromView(value); } }
//				
//		public UnselectionBehavior UnselectionBehavior { get; set; }
//		
//		public bool IsNavigateable { get; set; }
//		public bool IsSelectable { get; set; }
//		public bool IsMultiselect { get; set; }
//		public bool PopOnSelection { get; set; }
//		public bool HideCaptionOnSelection { get; set; }
//		
//		public EnumView() : base(RectangleF.Empty)
//		{
//		}
//
//		public EnumView(RectangleF frame) : base(frame)
//		{
//		}
//
//		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
//		{			
//			cell.TextLabel.Text = Caption;
//			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//
//			var navigateToView = DataContext.Member.GetCustomAttribute<NavigateToViewAttribute>();
//			if (navigateToView != null)
//			{
//				TransitionStyle = navigateToView.TransitionStyle;
//				IsModel = navigateToView.ShowModal;
//				ViewType = navigateToView.ViewType; 
//			}
//
//			var multiselectionAttribute = DataContext.Member.GetCustomAttribute<MultiselectionAttribute>();
//			IsMultiselect = multiselectionAttribute != null;
//			if (multiselectionAttribute != null && !string.IsNullOrEmpty(multiselectionAttribute.MemberName))
//			{
//				SelectedItemsMemberName = multiselectionAttribute.MemberName;
//				SelectedAccessoryViewType = multiselectionAttribute.SelectedAccessoryViewType;
//				UnselectedAccessoryViewType = multiselectionAttribute.UnselectedAccessoryViewType;
//				UnselectionBehavior = multiselectionAttribute.UnselectionBehavior;
//			}
//
//			var selectionAttribute = DataContext.Member.GetCustomAttribute<SelectionAttribute>();
//			if (selectionAttribute != null && !string.IsNullOrEmpty(selectionAttribute.MemberName))
//			{
//				IsSelectable = true;
//				SelectedItemMemberName = selectionAttribute.MemberName;
//				ViewType = selectionAttribute.NavigateToView;
//				IsNavigateable = selectionAttribute.NavigateToView != null || !DataContext.Type.IsEnum;
//
//				if (SelectedAccessoryViewType == null || selectionAttribute.SelectedAccessoryViewType != null)
//				{
//					SelectedAccessoryViewType = selectionAttribute.SelectedAccessoryViewType;
//				}
//				if (UnselectedAccessoryViewType == null || selectionAttribute.UnselectedAccessoryViewType != null)
//				{
//					UnselectedAccessoryViewType = selectionAttribute.UnselectedAccessoryViewType;
//				}
//			}						
//		}
//
//		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
//		{
//			SelectedItems.Add(SelectedItem);
//
//			SetItems();
//
//			controller.Selected(SelectedItem, indexPath);
//
//			if (PopOnSelection && !(IsNavigateable || IsMultiselect))
//			{
//				controller.NavigationController.PopViewControllerAnimated(true);
//			}
//			
//			if (IsSelectable || IsMultiselect)
//			{
//				controller.ReloadData();
//			}
//			else
//			{
//				new Wait(new TimeSpan(0, 0, 0, 0, 300), () => 
//				{
//					controller.ReloadData();
//				});
//			}
//			
//			if (IsNavigateable)
//			{
//				if (ViewType != null)
//				{
//					NavigateToView(ViewType);
//					//return;
//				}
//			
//				NavigateToList();
//			}
//		}
//
//		public void NavigateToView(Type viewType)
//		{
//			if (viewType != null)
//			{
//				var view = Activator.CreateInstance(viewType) as UIView;
//
//				var dc = view as IDataContext<object>;
//				if (dc != null)
//				{
//					dc.DataContext = SelectedItem;
//				}
//				
//				MonoMobileApplication.PushView(view);
//			}
//		}
//
//		public void NavigateToList()
//		{
//			//var section = Sections[0];
////			IEnumerable<Type> viewTypes = null; 
////			var dvc = new DialogViewController(Caption, Controller.RootView, true);
////			if (_NavigationSource == null)
////			{
////				_NavigationSource = new ListSource(dvc, GetSectionData(0), viewTypes);
////			}
////
////			_NavigationSource.Controller = dvc;
//
////			dvc.ToolbarButtons = null;
////			dvc.NavbarButtons = null;
//
//			_NavigationSource.SelectedItem = SelectedItem;
//			_NavigationSource.SelectedItems = SelectedItems;
//			_NavigationSource.UnselectionBehavior = UnselectionBehavior;
//
//			_NavigationSource.IsNavigateable = false;
//			_NavigationSource.IsRoot = false;
//			_NavigationSource.IsMultiselect = IsMultiselect;
//			_NavigationSource.IsSelectable = IsSelectable;
//		//	_NavigationSource.CellFactory = CellFactory;
//			_NavigationSource.NavigationToView = null;
//			_NavigationSource.PopOnSelection = PopOnSelection;
//	//		_NavigationSource.NibName = NibName;
//
//	//		_NavigationSource.TableViewStyle = TableViewStyle;
//	
////			_NavigationSource.IsSearchbarHidden = IsSearchbarHidden;
////			_NavigationSource.EnableSearch = EnableSearch;
////			_NavigationSource.IncrementalSearch = IncrementalSearch;
////			_NavigationSource.SearchPlaceholder = SearchPlaceholder;
////			_NavigationSource.SearchCommand = SearchCommand;
//
//			_NavigationSource.SelectedAccessoryViewType = SelectedAccessoryViewType;
//			_NavigationSource.UnselectedAccessoryViewType = UnselectedAccessoryViewType;
//			
//	//		dvc.TableView.Source = _NavigationSource;
//	//		Controller.NavigationController.PushViewController(dvc, true);
//		}
//
//		protected MemberInfo GetMemberFromView(string memberName)
//		{
//			var memberInfo = DataContext.Type.GetMember(memberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault();
//
//			return memberInfo;
//		}
//
//		private void SetItems()
//		{
////			if (_SelectedItemMember != null)
////			{
////				_SelectedItemMember.SetValue(Controller.RootView, SelectedItem);
////			}
////	
////			if (IsMultiselect)
////			{		
////				if (_SelectedItemsMember != null)
////				{
////					_SelectedItemsMember.SetValue(Controller.RootView, SelectedItems);
////				}
////			}
//		}
//	}
//}
//
