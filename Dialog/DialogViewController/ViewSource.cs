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

	public class ViewSource : BaseDialogViewSource
	{	
		public ViewSource(DialogViewController controller) : base(controller)
		{
			CellFactory = new TableCellFactory<UITableViewCell>("cell");
		}  	
		
		public ViewSource(DialogViewController controller, string title) : this(controller)
		{
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var memberData = GetMemberData(indexPath);
			
			var cell = CellFactory.GetCell(tableView, indexPath, memberData.Id, NibName, (cellId, idxPath) => NewCell(cellId, idxPath));

			UpdateCell(cell, indexPath);

			return cell;
		}

		protected override UITableViewCell NewCell(NSString cellId, NSIndexPath indexPath)
		{
			var memberData = GetMemberData(indexPath);
			
			var section = Sections[indexPath.Section];
		
			if (section.ViewTypes != null)
			{
				var viewType = ViewContainer.GetView(memberData);
				if (viewType != null)
				{
					if (section.ViewTypes.ContainsKey(memberData.Id))
						section.ViewTypes[memberData.Id] = new List<Type>() { viewType}; 
					else
						section.ViewTypes.Add(memberData.Id, new List<Type>() { viewType });
				}
			}

			return base.NewCell(memberData.Id, indexPath);
		}

		protected override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			base.UpdateCell(cell, indexPath);

			var resizedRows = false;

			var memberData = GetMemberData(indexPath);
			
			var caption = memberData.Member.Name.Capitalize();
			cell.TextLabel.Text = caption;
			
			if (memberData.Value != null && cell.DetailTextLabel != null)
				cell.DetailTextLabel.Text = memberData.Value.ToString();

			var section = Sections[indexPath.Section];
			if (section.Views.ContainsKey(cell))
			{
				var views = section.Views[cell];
	
				if (views.Count > 0)
				{
					foreach (var view in views)
					{
						var viewCaption = view as ICaption;
						if (viewCaption != null)
						{
							viewCaption.Caption = caption;
						}
		
						var dc = view as IDataContext<MemberData>;
						if (dc != null)
						{
							var item = GetMemberData(indexPath);
							dc.DataContext = item;
						}
		
						var updateable = view as IUpdateable;
						if (updateable != null)
						{
							updateable.UpdateCell(cell, indexPath);
						}

						var sizeable = view as ISizeable;
						if (sizeable != null)
						{
							var rowHeight = sizeable.GetRowHeight();

							if (RowHeights.ContainsKey(indexPath))
							{
								RowHeights[indexPath] = rowHeight;
							}
							else
								RowHeights.Add(indexPath, rowHeight);
							
							resizedRows = true;
						}
					}
				}

				if (resizedRows)
				{
					new Wait(new TimeSpan(0), ()=>
					{
						Controller.TableView.BeginUpdates();
						Controller.TableView.EndUpdates();
					});
				}
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var memberData = GetMemberData(indexPath);
			var cell = GetCell(tableView, indexPath);

			var section = Sections[indexPath.Section];
			if (section.Views.ContainsKey(cell))
			{
				var views = section.Views[cell];
	
				if (views.Count > 0)
				{
					foreach (var view in views)
					{
						var selectable = view as ISelectable;
						if (selectable != null)
						{
							selectable.Selected(Controller, tableView, memberData, indexPath);
						}
					}
				}
			}
		}	

		private MemberData GetMemberData(NSIndexPath indexPath)
		{
			return GetSectionData(indexPath.Section)[indexPath.Row] as MemberData;
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

