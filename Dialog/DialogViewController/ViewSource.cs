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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class ViewSource : BaseDialogViewSource, IActivation
	{	
		public ViewSource(DialogViewController controller) : base(controller)
		{
			CellFactory = new TableCellFactory<UITableViewCell>("cell");
		}  	
		
		public ViewSource(DialogViewController controller, string title) : this(controller)
		{
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			if (IsRoot)
			{
				return 1;
			}
		
			var listCount = 0;
			if (Sections != null)
			{
				var listSource = Sections[section].ListSources[0];

				if (listSource != null && !listSource.IsRoot)
				{
					listCount = listSource.RowsInSection(tableview, 0) - 1;
				}
			}

			return Sections != null ? Sections[section].NumberOfRows + listCount: 0;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Sections != null ? Sections.Count : 0;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			MemberData memberData = null;
			UITableViewCell cell = null;

			if (PerformActionIfCellListElement(cell, indexPath, (listSource) => 
			{
				memberData = listSource.MemberData;
				cell = listSource.GetCell(tableView, NSIndexPath.FromRowSection(indexPath.Row -  memberData.Order, indexPath.Section));
			}))
			{
				return cell;
			}

			memberData = GetMemberData(indexPath);

			cell = CellFactory.GetCell(tableView, indexPath, memberData.Id, NibName, (cellId, idxPath) => NewCell(cellId, idxPath));

			UpdateCell(cell, indexPath);

			return cell;
		}

		protected override UITableViewCell NewCell(NSString cellId, NSIndexPath indexPath)
		{
			var id = cellId;
	
			var memberData = GetMemberData(indexPath);
			if (memberData != null)
			{
				id = memberData.Id;
			}

			var section = Sections[indexPath.Section];
			var listSource = GetListSource(indexPath);

			if ((typeof(IEnumerable).IsAssignableFrom(memberData.Type) || typeof(Enum).IsAssignableFrom(memberData.Type)) && listSource != null && !listSource.IsRoot) 
			{
				id = listSource.CellId;
			}

			if (memberData != null)
			{
				var viewType = ViewContainer.GetView(memberData);
				if (viewType != null)
				{
					var key = memberData.Id.ToString();

					if (section.ViewTypes.ContainsKey(key))
					{
						var viewTypeList = section.ViewTypes[key];

						if (viewTypeList == null)
						{
							viewTypeList = new List<Type>();
							section.ViewTypes[key] =viewTypeList;
						}

						if (!viewTypeList.Contains(viewType))
						{
							viewTypeList.Add(viewType);
						}
					}
					else
						section.ViewTypes.Add(key, new List<Type>() { viewType });
				}
			}

			return base.NewCell(id, indexPath);
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			MemberData memberData = null;

			if (PerformActionIfCellListElement(cell, indexPath, (listSource) => listSource.UpdateCell(cell, indexPath)))
				return;
			
			memberData = GetMemberData(indexPath);

			base.UpdateCell(cell, indexPath);
			
			if (memberData.Value != null && cell.DetailTextLabel != null)
				cell.DetailTextLabel.Text = memberData.Value.ToString();

			foreach(var section in Sections.Values)
			{
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
								viewCaption.Caption = ViewParser.GetCaption(memberData.Member);
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
						}
					}
				}
			}
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			MemberData memberData = null;

			var listIndexPath = NSIndexPath.FromRowSection(0, indexPath.Section);
			var listSource = GetListSource(listIndexPath);
			
			if (listSource != null && !listSource.IsRoot)
			{
				var listCount = listSource.Sections[0].DataContext.Count;
				
				if (indexPath.Row < listCount)
				{
					memberData = listSource.MemberData;
									
					if (typeof(IEnumerable).IsAssignableFrom(memberData.Type) || typeof(Enum).IsAssignableFrom(memberData.Type))
					{
						listSource.RowSelected(tableView, indexPath);
						
						// Special case: Since Enums don't have MemberInfo for individual items we only keep track of the base 
						// type and set the actual value based on the row since we display them in the section in value order
						if (typeof(Enum).IsAssignableFrom(memberData.Type))
						{
							memberData.Value = indexPath.Row;
						}

						return;
					}
				}
			}
 
			var cell = Controller.TableView.CellAt(indexPath);
			if (cell == null)
				cell = GetCell(tableView, indexPath);

			foreach(var section in Sections.Values)
			{
				if (section.Views.ContainsKey(cell))
				{
					var views = section.Views[cell];
		
					if (views.Count > 0)
					{
						memberData = GetMemberData(indexPath);
						
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
		}	

		public void Activated()
		{
			foreach(var section in Sections.Values)
			{
				foreach(var listSource in section.ListSources.Values)
				{
					var activation = listSource as IActivation;
					if (activation != null)
						activation.Activated();
				}
			}
		}

		public void Deactivated()
		{
			foreach (var section in Sections.Values)
			{
				foreach (var listSource in section.ListSources.Values)
				{
					var activation = listSource as IActivation;
					if (activation != null)
					{
						activation.Deactivated();
					}
				}
			}
		}
	
		private bool PerformActionIfCellListElement(UITableViewCell cell, NSIndexPath indexPath, Action<ListSource> action)
		{
			var listIndexPath = NSIndexPath.FromRowSection(0, indexPath.Section);
			var listSource = GetListSource(listIndexPath);

			if (listSource != null && !listSource.IsRoot)
			{
				var listCount = listSource.Sections[0].DataContext.Count;
				if (indexPath.Row < listCount)
				{
					var memberData = listSource.MemberData;
								
					if ((typeof(IEnumerable).IsAssignableFrom(memberData.Type) || typeof(Enum).IsAssignableFrom(memberData.Type)))
					{
						if (action != null)
						{
							action(listSource);
							return true;
						}
					}
				}
			}

			return false;
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

