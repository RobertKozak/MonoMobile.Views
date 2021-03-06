// 
//  ObjectCellView.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
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
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class ObjectCellView<T> : CellView<T>, ISelectable, INavigable
	{
		public UIModalTransitionStyle TransitionStyle { get; set; }
		public bool IsModal { get; set; }
		public Type NavigateToViewType { get; set; }
		
		public ObjectCellView() : base(RectangleF.Empty)
		{
		}

		public ObjectCellView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.TextLabel.Text = Caption;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;

			var navigateToView = DataContext.Member.GetCustomAttribute<NavigateToViewAttribute>();
			if (navigateToView != null)
			{
				TransitionStyle = navigateToView.TransitionStyle;
				IsModal = navigateToView.IsModal;
				NavigateToViewType = navigateToView.NavigateToViewType; 
			}
		}

		public virtual void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			if (NavigateToViewType == null)
				return;

			var dataContext = DataContext.Value;

			if (dataContext == null)
			{
				dataContext = Activator.CreateInstance(DataContext.Member.GetMemberType());
			}

			if (dataContext != null) 
			{
				var view = dataContext;
				if (!view.GetType().Equals(NavigateToViewType))
				{
					view = ViewCreator.Create(NavigateToViewType, dataContext);
				}

				var dvc = new DialogViewController(Caption, view, controller.Theme, true) { Autorotate = true };
				var nav = controller.ParentViewController as UINavigationController;

				if (IsModal)
				{		
					dvc.ModalTransitionStyle = TransitionStyle;
 
					var navController = new NavigationController() { ViewControllers = new UIViewController[] { dvc } };

					nav.PresentModalViewController(navController, true);
				}
				else
				{
					nav.PushViewController(dvc, true);
				}
			}
		}
	}
}

