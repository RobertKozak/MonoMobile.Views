// 
//  ViewSource.cs
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

	public class ViewSource : BaseDialogViewSource<List<MemberData>>
	{
		public ViewSource(DialogViewController controller, object view) : base(controller)
		{
			CellFactory = new TableCellFactory<UITableViewCell>("memberCell");
		}  
		
		protected override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			base.UpdateCell(cell, indexPath);

			var memberData = GetSectionData(indexPath.Section)[indexPath.Row];
			
			cell.TextLabel.Text = memberData.Member.Name.Capitalize();
			
			if (memberData.DataContext != null)
				cell.DetailTextLabel.Text = memberData.DataContext.ToString();

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
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var item = GetSectionData(indexPath.Section)[indexPath.Row];

			var methodInfo = item.Member as MethodInfo; 
			if (methodInfo != null)
			{
				methodInfo.Invoke(item.Source, null);
				return;
			}
			
			UIView view = item.DataContext as UIView;

			if (view != null)
			{
				var dvc = new DialogViewController("", view, true) { Autorotate = true };
				var nav = Controller.ParentViewController as UINavigationController;
				nav.PushViewController(dvc, true);
			}
		}		
	}

//	public class DialogViewDataSource : BaseDialogViewSource<object>
//	{
//		protected IRoot Root {get; set; }
//
//		public DialogViewDataSource(DialogViewController controller): base(controller, null)
//		{
//			Root = controller.Root;
//		}
//		
//		public override int RowsInSection(UITableView tableview, int section)
//		{
//			var s = Root.Sections[section];
//			var count = s.Elements.Count;
//			
//			return s.ExpandState == ExpandState.Opened ? count : 0;
//		}
//
//		public override int NumberOfSections(UITableView tableView)
//		{
//			return Root.Sections.Count;
//		}
//
//		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
//		{
////			var sw = System.Diagnostics.Stopwatch.StartNew();
//
//			var element = GetElement(indexPath);
//			
//			//if no element return an empty cell since GetCell must return a cell
//			if (element == null)
//			{
//				Console.WriteLine("Creating empty Cell");
//				return new UITableViewCell();
//			}
//				
////			sw.Stop();
////			Console.WriteLine("GetCell: "+sw.Elapsed.TotalMilliseconds);
//			return element.GetCell(tableView) as UITableViewElementCell;
//		}
//
//		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
//		{
//			var element = GetElement(indexPath);
//
//			if (element != null)
//			{
//				var selectable = element as ISelectable;
//	
//				if (selectable != null)
//				{
//					if (element.Cell.SelectionStyle != UITableViewCellSelectionStyle.None)
//					{
//						tableView.DeselectRow(indexPath, false);
//						
//						UIView.Animate(0.3f, delegate {
//							element.Cell.Highlighted = true;  }, delegate {
//							element.Cell.Highlighted = false; });
//					}
//					
//					selectable.Selected(Controller, tableView, element.DataContext, indexPath);
//				}
//			}
//			else
//			{
////				tableView.DeselectRow(indexPath, true);
////				var selectable = RootView as ISelectable;
////				if (selectable != null)
////				{
////					selectable.Selected(Container, tableView, indexPath);
////				}
//
//				throw new Exception("Find out why we are here!");
//			}
//		}
//
//		public ISection GetSection(int sectionIndex)
//		{
//			ISection section = null;
//			if (Root.Sections.Count > sectionIndex)
//				section = Root.Sections[sectionIndex];
//			
//			return section;
//		}
//
//		public IElement GetElement(NSIndexPath indexPath)
//		{					
//			IElement element = null;
//			var section = GetSection(indexPath.Section);
//			if (section != null && section.Elements != null && section.Elements.Count > indexPath.Row)
//				element = section.Elements[indexPath.Row];
//
//			return element;
//		}
//	}
}

