// 
//  ListCellView.cs
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
	public class ListCellView : CellView<IEnumerable>, ISelectable
	{
		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }

		public ListCellView() : base(RectangleF.Empty)
		{
		}

		public ListCellView(RectangleF frame) : base(frame)
		{
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var source = Controller.TableView.Source as BaseDialogViewSource;
			indexPath = source.ResetIndexPathRow(indexPath);

			var listSource = source.GetListSource(indexPath);

			// TODO: Why is this null?
			if (listSource == null)
				return;

			listSource.UpdateCell(cell, indexPath);

			cell.TextLabel.Text = Caption;
			cell.Accessory = listSource.Sections[0].DataContext.Count > 1 ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
			cell.SelectionStyle = listSource.Sections[0].DataContext.Count > 1 ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None; 
			cell.DetailTextLabel.Text = string.Empty;
			
			var text = Caption;
			if (listSource.SelectedItem != null)
			{
				text = listSource.SelectedItem.ToString();
				cell.DetailTextLabel.Text = text;
			}
			
			if (listSource.IsRootCell)
			{
				if (listSource.ReplaceCaptionWithSelection)
				{
					cell.TextLabel.Text = text;
					cell.DetailTextLabel.Text = string.Empty;
				}
				else
				{
					if (cell.TextLabel.Text != text)
						cell.DetailTextLabel.Text = text; 
				}
			}
		
			if (listSource.IsMultiselect) 
			{
				cell.DetailTextLabel.Text = listSource.SelectedItems.Count.ToString();
			}
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{	
			var source = controller.TableView.Source as ViewSource;
			indexPath = source.ResetIndexPathRow(indexPath);
			var listSource = source.GetListSource(indexPath);
							
			if (listSource.Sections[0].DataContext.Count > 0)
			{
//				var isMultiselect = listSource.SelectionAction == SelectionAction.Multiselection;
//				var listAttribute = DataContext.Member.GetCustomAttribute<ListAttribute>();
//	
//				var navigateToView = DataContext.Member.GetCustomAttribute<NavigateToViewAttribute>();
//				if (navigateToView != null)
//				{
//					listSource.ModalTransitionStyle = navigateToView.TransitionStyle;
//					listSource.IsModal = navigateToView.ShowModal;
//					listSource.NavigationViewType = navigateToView.ViewType;
//					listSource.IsSelectable = listAttribute != null && isMultiselect;
//				}
				
				listSource.RowSelected(tableView, indexPath);
			}
		}
	}
}


