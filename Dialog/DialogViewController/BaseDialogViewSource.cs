// 
//  BaseDialogViewSource.cs
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
	using System.Collections.Specialized;
	using System.Drawing;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.CoreAnimation;
	using MonoTouch.Foundation;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public abstract class BaseDialogViewSource : UITableViewSource, ISearchBar, IEnumerable
	{
		private const float _SnapBoundary = 65;
		private bool _CheckForRefresh;

		private HeaderTapGestureRecognizer tapRecognizer;
		private Selector _HeaderSelector = new Selector("headerTap");

		protected TableCellFactory<UITableViewCell> CellFactory;
		protected DialogViewController Controller;
		
		protected string NibName { get; set; }
		
		protected IDictionary<NSIndexPath, float> RowHeights;

		protected IDictionary<UITableViewCell, UIView> SelectedAccessoryViews;
		protected IDictionary<UITableViewCell, UIView> UnselectedAccessoryViews;

		public Type SelectedAccessoryViewType { get; set; }
		public Type UnselectedAccessoryViewType { get; set; }
		
		private IDictionary<int, Section> _Sections;
		public IDictionary<int, Section> Sections {
			get 
			{ return _Sections;}  
			set
			{
				_Sections = value;
			} 
		}

		public UITableViewStyle TableViewStyle { get; set; }
			
		public bool IsSearchbarHidden { get; set; }
		public bool EnableSearch { get; set; }
		public bool IncrementalSearch { get; set; }
		public string SearchPlaceholder { get; set; }
		public SearchCommand SearchCommand { get; set; }
		
		public bool IsRoot { get; set; }
		public bool IsNavigateable { get; set; }

		public string Caption { get; set; }
		
		public BaseDialogViewSource(DialogViewController controller)
		{
			Controller = controller;

			RowHeights = new Dictionary<NSIndexPath, float>();

			SelectedAccessoryViews = new Dictionary<UITableViewCell, UIView>();
			UnselectedAccessoryViews = new Dictionary<UITableViewCell, UIView>();

			Sections = new Dictionary<int, Section>();
		}

		public virtual IEnumerator GetEnumerator()
		{
			foreach (var section in Sections)
				yield return section;
		}
		
		public int Add(Section section)
		{
			if (Sections.ContainsKey(section.Index))
			{
				Sections[section.Index] = section;
			}
			else
			{
				Sections.Add(section.Index, section);
			}
			
			section.Controller = Controller;
			return section.Index;
		}

		#region Cells
		protected virtual UITableViewCell NewCell(NSString cellId, NSIndexPath indexPath)
		{
			var cellStyle = UITableViewCellStyle.Default;
			var cell = new UITableViewCell(cellStyle, cellId) { };			

			var views = new List<UIView>();
			var section = Sections[indexPath.Section];
		
			var key = cellId.ToString();

			if (section.ViewTypes != null && section.ViewTypes.ContainsKey(key))
			{
				var viewTypes = section.ViewTypes[key];
				if (viewTypes != null)
				{
					foreach (var viewType in viewTypes)
					{
						UIView view = null;
						var hasFrameCtor = viewType.GetConstructor(new Type[] { typeof(RectangleF) }) != null;
						if (hasFrameCtor)
							view = Activator.CreateInstance(viewType, new object[] { cell.ContentView.Bounds }) as UIView;
						else
							view = Activator.CreateInstance(viewType) as UIView;
	
						var initializeCell = view as IInitializeCell;
						if (initializeCell != null)
						{
							var newCellStyle = initializeCell.CellStyle;
							if (newCellStyle != cellStyle)
							{
								// recreate cell with new style
								cell = new UITableViewCell(newCellStyle, cellId) { };
							}
	
							initializeCell.Cell = cell;
							initializeCell.Controller = Controller;
						}
						
						views.Add(view);
					}
				}
			}
			
			cell.TextLabel.Text = Caption;
			cell.TextLabel.BackgroundColor = UIColor.Clear;
			cell.TextLabel.AdjustsFontSizeToFitWidth = true;
			
			if (cell.DetailTextLabel != null)
			{
				cell.DetailTextLabel.BackgroundColor = UIColor.Clear;
				cell.DetailTextLabel.AdjustsFontSizeToFitWidth = true;
			}
			
			var selectable = this as ISelectable;
			cell.SelectionStyle = selectable != null ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.Blue;
			
			if (views.Count > 0)
			{
				cell.ContentView.AutosizesSubviews = true;
			}

			section.Views.Add(cell, views);
			var resizedRows = false;
			
			foreach (var view in views)
			{
				var accessoryView = view as IAccessoryView;
				if (accessoryView != null)
				{
					view.Tag = 1;
					view.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleLeftMargin;
					cell.AccessoryView = view;
				}
				else
				{
					var contentView = view as ICellContent;
					if (contentView != null)
					{
						view.Tag = 1;
						view.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleLeftMargin;
						cell.ContentView.Add(view);
					}
				}
				
				var sizeable = view as ISizeable;
				if (sizeable != null)
				{
					var rowHeight = sizeable.GetRowHeight();

					if (RowHeights.ContainsKey(indexPath))
					{
						if (RowHeights[indexPath] != rowHeight)
						{
							RowHeights[indexPath] = rowHeight;
							resizedRows = true;
						}
					}
					else
					{
						RowHeights.Add(indexPath, rowHeight);
						resizedRows = true;
					}
				}
			}

			if (resizedRows)
			{
//				new Wait(new TimeSpan(0), () =>
//				{
//					Controller.TableView.BeginUpdates();
//					Controller.TableView.EndUpdates();
//				});
			}

			return cell;
		}

		public virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var viewToRemove = cell.ContentView.ViewWithTag(1);
			if (viewToRemove != null)
			{
			//	viewToRemove.RemoveFromSuperview();
			}

//			cell.AccessoryView = null;
			cell.Accessory = IsRoot || IsNavigateable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
	
			cell.TextLabel.TextAlignment = UITextAlignment.Left;
			cell.TextLabel.Text = string.Empty;

			if (cell.DetailTextLabel != null)
			{
				cell.DetailTextLabel.TextAlignment = UITextAlignment.Right;
				cell.DetailTextLabel.Text = string.Empty;
			}
		}
		#endregion

		#region Row support
		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var memberData = GetMemberData(indexPath);
			if (memberData != null && memberData.RowHeight != 0)
				return memberData.RowHeight;

			if (!RowHeights.ContainsKey(indexPath))
			{
				return tableView.RowHeight;
			}

			return RowHeights[indexPath];
		}
		#endregion

		#region Pull to Refresh support
		public override void Scrolled(UIScrollView scrollView)
		{
			if (!_CheckForRefresh)
				return;
			if (Controller.Reloading)
				return;
			var view = Controller.RefreshView;
			if (view == null)
				return;
			
			var point = Controller.TableView.ContentOffset;
			
			if (view.IsFlipped && point.Y > -_SnapBoundary && point.Y < 0)
			{
				view.Flip(true);
				view.SetStatus(RefreshStatus.PullToReload);
			}
			else if (!view.IsFlipped && point.Y < -_SnapBoundary)
			{
				view.Flip(true);
				view.SetStatus(RefreshStatus.ReleaseToReload);
			}
		}

		public override void DraggingStarted(UIScrollView scrollView)
		{
			_CheckForRefresh = true;
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			if (Controller.RefreshView == null)
				return;
			
			_CheckForRefresh = false;
			if (Controller.TableView.ContentOffset.Y > -_SnapBoundary)
				return;
			
			Controller.TriggerRefresh(true);
		}
		#endregion
		
		#region Headers and Footers
		public override string TitleForHeader(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					return section.HeaderText;
				}
			}

			return string.Empty;
		}

		public override string TitleForFooter(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					return section.FooterText;
				}
			}
		
			return string.Empty;
		}

		public override float GetHeightForHeader(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				
				if (section != null && !string.IsNullOrEmpty(section.HeaderText))
				{
					var indentation = UIDevice.CurrentDevice.GetIndentation();
					var width = tableView.Bounds.Width - (indentation * 2);
					
					var headerLabel = new UILabel();
	
					headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
					var size = headerLabel.StringSize(section.HeaderText, headerLabel.Font);
				
					var height = (float)(size.Height * (headerLabel.Font.NumberOfLines(section.HeaderText, width) + 0.5));
						
					return height;
				}
			}
			return 0;
		}

		public override UIView GetViewForHeader(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					if (section.HeaderView == null && !string.IsNullOrEmpty(section.HeaderText))
					{
						section.HeaderView = CreateHeaderView(tableView, section.HeaderText);
					}
		
					if (section.HeaderView != null)
					{
						if (section.IsExpandable)
						{
							section.ArrowView.Bounds = new RectangleF(0, 0, 16, 16);
							section.ArrowView.Frame = new RectangleF(5, 8, 16, 16);
							section.HeaderView.Add(section.ArrowView);
						
							tapRecognizer = new HeaderTapGestureRecognizer(section, this, _HeaderSelector);
					
							section.HeaderView.AddGestureRecognizer(tapRecognizer);
							Flip(section);
						}
					}
					return section.HeaderView;
				}
			}
			return null;
		}

		public override float GetHeightForFooter(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				if (section != null && !string.IsNullOrEmpty(section.FooterText))
				{
					var indentation = UIDevice.CurrentDevice.GetIndentation();
	
					var width = tableView.Bounds.Width - (indentation * 2);
	
					var footerLabel = new UILabel();
				
					footerLabel.Font = UIFont.SystemFontOfSize(15);
					var size = footerLabel.StringSize(section.FooterText, footerLabel.Font);
	
					var height = size.Height * (footerLabel.Font.NumberOfLines(section.FooterText, width));
					
					return height;
				}
			}
			
			//if greater than 0 then the empty footer will display and prevent empty rows from displaying
			return 1;			
		}
		
		public override UIView GetViewForFooter(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex)
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
					{
						section.FooterView = CreateFooterView(tableView, section.FooterText);
					}
					
					// Use an empty UIView to Eliminate Extra separators for blank items
					if (section.FooterView == null || section.ExpandState == ExpandState.Closed)
					{
						return new UIView(RectangleF.Empty) { BackgroundColor = UIColor.Clear };
					}
					
					return section.FooterView;
				}
			}
			return null;
		}

		[Export("headerTap")]
		public void HeaderTap(HeaderTapGestureRecognizer recognizer)
		{
			var section = recognizer.Section;
			var state = section.ExpandState;
			
			if (section.IsExpandable)
			{
				if (state == ExpandState.Opened)
				{
					section.ExpandState = ExpandState.Closed;
				}
				else
				{
					section.ExpandState = ExpandState.Opened;
				}
			
				Flip(section);
			}
		}

		private UIView CreateHeaderView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			var width = tableView.Bounds.Width - (indentation - 10);

			var headerLabel = new UILabel();
			headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);

			var size = headerLabel.StringSize(caption, headerLabel.Font);
			var height = (float)(size.Height * (headerLabel.Font.NumberOfLines(caption, width) + 0.5));

			var frame = new RectangleF(tableView.Bounds.X + (indentation + 10), 0, width, height);
			
			headerLabel.Frame = frame;
			
			headerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			headerLabel.ShadowColor = UIColor.White;
			headerLabel.LineBreakMode = UILineBreakMode.WordWrap;
			headerLabel.ShadowOffset = new SizeF(0, 1);
			headerLabel.Text = caption;
			headerLabel.Lines = headerLabel.Font.NumberOfLines(caption, frame.Width);
			
			var view = new UIView(new RectangleF(0, 0, frame.Width, height));

			if (tableView.Style == UITableViewStyle.Grouped)
			{
				headerLabel.BackgroundColor = UIColor.Clear;
				view.Opaque = false;
			}
			else
			{
//				var theme = Container.Theme;
//				var background = theme.HeaderBackgroundColor;
//				if (background != null)
//				{
//					headerLabel.BackgroundColor = background;
//				}
			}

			view.BackgroundColor = headerLabel.BackgroundColor;
			
			view.AddSubview(headerLabel);
			
			return view;
		}

		private UIView CreateFooterView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			
			var bounds = tableView.Bounds;
			var width = bounds.Width - (indentation * 2);

			var footerLabel = new UILabel();		
			
			footerLabel.Font = UIFont.SystemFontOfSize(15);
			
			var rect = new RectangleF(bounds.X + indentation, bounds.Y, width, 0);
					
			footerLabel.Frame = rect;
			footerLabel.BackgroundColor = UIColor.Clear;
			footerLabel.TextAlignment = UITextAlignment.Center;
			footerLabel.LineBreakMode = UILineBreakMode.WordWrap;
			footerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			footerLabel.ShadowColor = UIColor.White;
			footerLabel.ShadowOffset = new SizeF(0, 1);
			footerLabel.Text = caption;
			footerLabel.Lines = footerLabel.Font.NumberOfLines(caption, width);

			return footerLabel;
		}

		private void Flip(Section section)
		{
			UIView.BeginAnimations(null);
			UIView.SetAnimationDuration(0.18f);
			section.ArrowView.Layer.Transform = section.ExpandState == ExpandState.Closed ? CATransform3D.MakeRotation((float)Math.PI * 1.5f, 0, 0, 1) : CATransform3D.MakeRotation((float)Math.PI * 2f, 0, 0, 1);
			
			UIView.CommitAnimations();
		}
		#endregion

		public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
		{
//			var element = GetElement(indexPath);
//
//			if (element.AccessoryCommand != null && element.AccessoryCommand.CanExecute(null))
//			{
//				element.AccessoryCommand.Execute(null);
//			}
		}

		public IList GetSectionData(int sectionIndex)
		{
			if (Sections != null)
			{
				return Sections[sectionIndex].DataContext;
			}

			return default(IList);
		}

		public void SetSectionData(int sectionIndex, IList data)
		{
			if (Sections != null)
			{
				Sections[sectionIndex].DataContext = data;
			}
		}

		protected MemberInfo GetMemberFromView(string memberName)
		{
			var memberInfo = Controller.RootView.GetType().GetMember(memberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault();

			return memberInfo;
		}
		
		public NSIndexPath ResetIndexPathRow(NSIndexPath indexPath)
		{
			int newRow = indexPath.Row;
			var listCount = 0;

			var listSource = GetListSource(NSIndexPath.FromRowSection(0, indexPath.Section));
			if (listSource != null && !listSource.IsRoot)
			{
				listCount = listSource.Sections[0].DataContext.Count;
			}

			
			if (indexPath.Row >= listCount && listCount != 0)
				indexPath = NSIndexPath.FromRowSection(indexPath.Row - listCount + 1, indexPath.Section);

			return indexPath;
		}

		protected MemberData GetMemberData(NSIndexPath indexPath)
		{
			var newIndexPath = ResetIndexPathRow(indexPath);
			
			var sectionData = GetSectionData(indexPath.Section);

			if (sectionData != null && sectionData.Count > newIndexPath.Row)
			{
				return sectionData[newIndexPath.Row] as MemberData;
			}

			return null;
		}

		public ListSource GetListSource(NSIndexPath indexPath)
		{
			var section = Sections[indexPath.Section];

			if (indexPath.Row < section.ListSources.Count)
				return section.ListSources[indexPath.Row];
		
			return null;
		}

		protected virtual void SetSelectionAccessory(UITableViewCell cell, NSIndexPath indexPath)
		{
			cell.AccessoryView = null;

			if (!IsNavigateable)
			{
				UIView selectedAccessoryView = null;
				UIView unselectedAccessoryView = null;

				if (SelectedAccessoryViews.ContainsKey(cell))
				{
					selectedAccessoryView = SelectedAccessoryViews[cell];
				}
				else
				{
					if (SelectedAccessoryViewType != null)
					{
						selectedAccessoryView = Activator.CreateInstance(SelectedAccessoryViewType) as UIView;
						SelectedAccessoryViews.Add(cell, selectedAccessoryView);
					}
				}

				if (UnselectedAccessoryViews.ContainsKey(cell))
				{
					unselectedAccessoryView = UnselectedAccessoryViews[cell];
				}
				else
				{
					if (UnselectedAccessoryViewType != null)
					{
						unselectedAccessoryView = Activator.CreateInstance(UnselectedAccessoryViewType) as UIView;
						UnselectedAccessoryViews.Add(cell, unselectedAccessoryView);
					}
				}
			}
		}
	}

	public class HeaderTapGestureRecognizer : UITapGestureRecognizer
	{
		public Section Section { get; set; }
 
		public HeaderTapGestureRecognizer(Section section, NSObject target, Selector selector): base(target, selector)
		{
			Section = section;
		}
	}
}


