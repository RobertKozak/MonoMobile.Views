// 
//  ListView.cs
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
	using System.Linq;
	using System.Reflection;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class ListView : CellView, ISelectable
	{
		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }

		public ListView() : base(RectangleF.Empty)
		{
		}

		public ListView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var source = Controller.TableView.Source as ViewSource;
			var listSource = source.GetListSource(indexPath);
			
//TODO: fix this
			if (listSource == null)
				return;
			
			listSource.UpdateCell(cell, indexPath);

			cell.TextLabel.Text = Caption;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.DetailTextLabel.Text = string.Empty;
			
			var text = Caption;
			if (listSource.SelectedItem != null)
			{
				text = listSource.SelectedItem.ToString();
				cell.DetailTextLabel.Text = text;
			}

			var rootAttribute = DataContext.Member.GetCustomAttribute<RootAttribute>();
			if (rootAttribute != null)
			{
				if (rootAttribute.HideCaptionOnSelection)
				{
					cell.TextLabel.Text = text;
					cell.DetailTextLabel.Text = string.Empty;
				}
				else
				{
					cell.DetailTextLabel.Text = text; 
				}
			}
		
			var multiselectionAttribute = DataContext.Member.GetCustomAttribute<MultiselectionAttribute>();
			if (multiselectionAttribute != null)
			{
				cell.DetailTextLabel.Text = listSource.SelectedItems.Count.ToString();
			}
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{	
			var source = controller.TableView.Source as ViewSource;
			var listSource = source.GetListSource(indexPath);
							
			var multiselectionAttribute = DataContext.Member.GetCustomAttribute<MultiselectionAttribute>();
			var selectionAttribute = DataContext.Member.GetCustomAttribute<SelectionAttribute>();

			var navigateToView = DataContext.Member.GetCustomAttribute<NavigateToViewAttribute>();
			if (navigateToView != null)
			{
				listSource.ModalTransitionStyle = navigateToView.TransitionStyle;
				listSource.IsModal = navigateToView.ShowModal;
				listSource.NavigationViewType = navigateToView.ViewType;
				listSource.IsNavigateable = selectionAttribute == null && multiselectionAttribute == null;
			}
			
			listSource.RowSelected(tableView, indexPath);
		}
	}
}


