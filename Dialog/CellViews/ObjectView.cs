// 
//  ObjectView.cs
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
	using System.Drawing;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class ObjectView : CellView, ISelectable
	{
		public UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Default; } }

		public UIModalTransitionStyle TransitionStyle { get; set; }
		public bool IsModel { get; set; }
		public Type ViewType { get; set; }
		
		public ObjectView() : base(RectangleF.Empty)
		{
		}

		public ObjectView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.TextLabel.Text = Caption;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;

			var navigateToView = DataContext.Member.GetCustomAttribute<NavigateToViewAttribute>();
			if (navigateToView != null)
			{
				TransitionStyle = navigateToView.TransitionStyle;
				IsModel = navigateToView.ShowModal;
				ViewType = navigateToView.ViewType; 
			}
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			if (DataContext.Value == null)
			{
				DataContext.Value = Activator.CreateInstance(DataContext.Member.GetMemberType());
			}

//			if (typeof(Enum).IsAssignableFrom(DataContext.Type))
//			{
//				var parser = new ViewParser();
//				
//				var source = parser.ParseList(null, DataContext.Source, DataContext.Member, null);
//			}

			if (DataContext.Value != null) 
			{
				var view = DataContext.Value;
				if (ViewType != null && !view.GetType().Equals(ViewType))
				{
					view = Activator.CreateInstance(ViewType); 
				}

				var dc = view as IDataContext<object>;
				if (dc != null)
				{
					dc.DataContext = DataContext.Value;
				}

				var dvc = new DialogViewController(Caption, view, true) { Autorotate = true };
				var nav = controller.ParentViewController as UINavigationController;

				if (IsModel)
				{		
					dvc.ModalTransitionStyle = TransitionStyle;
 
					var navController = new UINavigationController();
					navController.ViewControllers = new UIViewController[] { dvc };

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

