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
	using System.Drawing;
	using System.Linq;
	using System.Reflection;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public abstract class BaseDialogViewSource : UITableViewSource, ISearchBar, IEnumerable, ITableViewStyle
	{
		private const float _SnapBoundary = 65;
		private bool _CheckForRefresh;

		protected TableCellFactory<UITableViewCell> CellFactory;
		
		protected string NibName { get; set; }
	
		protected IDictionary<UITableViewCell, UIView> SelectedAccessoryViews;
		protected IDictionary<UITableViewCell, UIView> UnselectedAccessoryViews;

		public IDictionary<NSIndexPath, float> RowHeights;

		public Type SelectedAccessoryViewType { get; set; }
		public Type UnselectedAccessoryViewType { get; set; }
		
		public IDictionary<int, Section> Sections { get; set; }

		public UITableViewStyle TableViewStyle { get; set; }
			
		public bool IsSearchbarHidden { get; set; }
		public bool EnableSearch { get; set; }
		public bool IncrementalSearch { get; set; }
		public string SearchPlaceholder { get; set; }
		public SearchCommand SearchCommand { get; set; }
		
		public bool IsRootCell { get; set; }
		public bool IsSelectable { get; set; }
		public bool IsNavigable { get; set; }

		public string Caption { get; set; }

		public DialogViewController Controller { get; set; }
		
		public BaseDialogViewSource(DialogViewController controller)
		{
			Controller = controller;

			RowHeights = new Dictionary<NSIndexPath, float>();

			SelectedAccessoryViews = new Dictionary<UITableViewCell, UIView>();
			UnselectedAccessoryViews = new Dictionary<UITableViewCell, UIView>();

			Sections = new Dictionary<int, Section>();

			TableViewStyle = UITableViewStyle.Grouped;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Controller = null;
			
				if (CellFactory != null)
				{
					CellFactory.Dispose();
					CellFactory = null;
				}

				foreach(var section in Sections.Values)
				{
					var disposable = section as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
						disposable = null;
					}
				}
			}

			base.Dispose(disposing);
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
					var memberData = GetMemberData(indexPath);

					foreach (var viewType in viewTypes)
					{
						UIView view = null;
						var hasFrameCtor = viewType.GetConstructor(new Type[] { typeof(RectangleF) }) != null;
						if (hasFrameCtor)
							view = Activator.CreateInstance(viewType, new object[] { cell.ContentView.Bounds }) as UIView;
						else
							view = Activator.CreateInstance(viewType) as UIView;

						var memberAttributes = memberData.Member.GetCustomAttributes(false);
						
						foreach (var memberAttribute in memberAttributes)
						{
							var hasCellViewTemplate = view as ICellViewTemplate;
							var cellViewTemplate = memberAttribute as CellViewTemplate;
							if (hasCellViewTemplate != null && cellViewTemplate != null)
							{
								hasCellViewTemplate.CellViewTemplate = cellViewTemplate;
							}

							var viewTheme = view as IThemeable;
							if (viewTheme != null)
							{
								var memberTheme = memberAttribute as CellViewTemplate;
								if (memberTheme != null && memberTheme.Theme != null)
								{
									viewTheme.Theme = Theme.CreateTheme(memberTheme.Theme);
								}
							}

							var navigable = view as INavigable;
							if (navigable != null)
							{
								var memberNavigable = memberAttribute as INavigable;
								if (memberNavigable != null)
								{
									navigable.ViewType = memberNavigable.ViewType;
									navigable.IsModal = memberNavigable.IsModal;
									navigable.TransitionStyle = memberNavigable.TransitionStyle;
								}
							}

							var caption = view as ICaption;
							if (caption != null)
							{
								var memberCaption = memberAttribute as ICaption;
								if (memberCaption != null && !string.IsNullOrEmpty(memberCaption.Caption))
								{
									caption.Caption = memberCaption.Caption;
								}
							}
						}

						var dc = view as IDataContext<MemberData>;
						if (dc != null)
						{
							dc.DataContext = memberData;
						}

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
						
						var themeable = view as IThemeable;
						if (themeable != null)
						{
							var theme = Theme.CreateTheme(Controller.Theme);

							var themeAttribute = viewType.GetCustomAttribute<ThemeAttribute>();
							if (themeAttribute != null)
							{
								var viewTypeTheme = Theme.CreateTheme(themeAttribute.ThemeType);
								theme.MergeTheme(viewTypeTheme);
							}

							themeAttribute = memberData.Member.GetCustomAttribute<ThemeAttribute>();
							if (themeAttribute != null)
							{
								var memberTheme = Theme.CreateTheme(themeAttribute.ThemeType);
								theme.MergeTheme(memberTheme);
							}

							themeable.Theme = theme;
							themeable.Theme.Cell = cell;
						}

						var initalizable = view as IInitializable;
						if (initalizable != null)
						{
							initalizable.Initialize();
						}
						
						views.Add(view);
					}
				}
			}
			
			cell.TextLabel.Text = Caption;
			cell.TextLabel.BackgroundColor = UIColor.Clear;
			
			if (cell.DetailTextLabel != null)
			{
				cell.DetailTextLabel.BackgroundColor = UIColor.Clear;
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
				using (new Wait(new TimeSpan(0), () =>
				{
					Controller.TableView.BeginUpdates();
					Controller.TableView.EndUpdates();
				}));
			}

			return cell;
		}

		public virtual void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
//			cell.AccessoryView = null;
			cell.Accessory = IsSelectable ? UITableViewCellAccessory.None : UITableViewCellAccessory.DisclosureIndicator;	
			cell.Accessory = IsNavigable ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
		}
		#endregion

		#region Row support
		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var memberData = GetMemberData(indexPath);
			if (memberData != null && memberData.RowHeight != 0)
			{
				return memberData.RowHeight;
			}

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
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
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
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
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
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
			{
				var section = Sections[sectionIndex];
				
				if (section != null && !string.IsNullOrEmpty(section.HeaderText))
				{
					var indentation = UIDevice.CurrentDevice.GetIndentation();
					var width = tableView.Bounds.Width - (indentation * 2);
					
					using (var headerLabel = new UILabel())
					{
						headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
						var size = headerLabel.StringSize(section.HeaderText, headerLabel.Font);
					
						var height = (float)(size.Height * (headerLabel.Font.NumberOfLines(section.HeaderText, width) + 0.5));
						
						return height;
					}
				}
			}
			return 0;
		}

		public override UIView GetViewForHeader(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					if (section.HeaderView == null && !string.IsNullOrEmpty(section.HeaderText))
					{
						section.HeaderView = CreateHeaderView(tableView, section.HeaderText);
					}
		
					return section.HeaderView;
				}
			}
			return null;
		}

		public override float GetHeightForFooter(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
			{
				var section = Sections[sectionIndex];
				if (section != null && !string.IsNullOrEmpty(section.FooterText))
				{
					var indentation = UIDevice.CurrentDevice.GetIndentation();
	
					var width = tableView.Bounds.Width - (indentation * 2);
	
					using (var footerLabel = new UILabel())
					{
						footerLabel.Font = UIFont.SystemFontOfSize(15);
						var size = footerLabel.StringSize(section.FooterText, footerLabel.Font);
		
						var height = size.Height * (footerLabel.Font.NumberOfLines(section.FooterText, width));
						
						return height;
					}
				}
			}
			
			//if greater than 0 then the empty footer will display and prevent empty rows from displaying
			return 1;			
		}
		
		public override UIView GetViewForFooter(UITableView tableView, int sectionIndex)
		{
			if (Sections.Count > sectionIndex && Sections.ContainsKey(sectionIndex))
			{
				var section = Sections[sectionIndex];
				if (section != null)
				{
					if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
					{
						section.FooterView = CreateFooterView(tableView, section.FooterText);
					}
					
					// Use an empty UIView to Eliminate Extra separators for blank items
					if (section.FooterView == null)
					{
						return new UIView(RectangleF.Empty) { BackgroundColor = UIColor.Clear };
					}
					
					return section.FooterView;
				}
			}
			return null;
		}

		private UIView CreateHeaderView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			var width = tableView.Bounds.Width - (indentation - 10);

			var headerLabel = new UILabel() { Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize) };

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
				var background = Controller.Theme.HeaderBackgroundColor;
				if (background != null)
				{
					headerLabel.BackgroundColor = background;
				}
			}

			view.BackgroundColor = headerLabel.BackgroundColor;
			
			if (Controller.Theme != null)
			{
				headerLabel.TextAlignment = Controller.Theme.HeaderTextAlignment;

				if (Controller.Theme.HeaderTextColor != null)
					headerLabel.TextColor = Controller.Theme.HeaderTextColor;

				if (Controller.Theme.HeaderTextShadowColor != null)
					headerLabel.ShadowColor = Controller.Theme.HeaderTextShadowColor;

				headerLabel.ShadowOffset = Controller.Theme.HeaderTextShadowOffset;
			}

			view.AddSubview(headerLabel);
			
			return view;
		}

		private UIView CreateFooterView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			
			var bounds = tableView.Bounds;
			var width = bounds.Width - (indentation * 2);

			var footerLabel = new UILabel() { Font = UIFont.SystemFontOfSize(15) };
			
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
			
			if (tableView.Style == UITableViewStyle.Grouped)
			{
				footerLabel.BackgroundColor = UIColor.Clear;
			}
			else
			{
				var background = Controller.Theme.FooterBackgroundColor;
				if (background != null)
				{
					footerLabel.BackgroundColor = background;
				}
			}

			if (Controller.Theme != null)
			{
				footerLabel.TextAlignment = Controller.Theme.FooterTextAlignment;

				if (Controller.Theme.FooterTextColor != null)
					footerLabel.TextColor = Controller.Theme.FooterTextColor;

				if (Controller.Theme.FooterTextShadowColor != null)
					footerLabel.ShadowColor = Controller.Theme.FooterTextShadowColor;

				footerLabel.ShadowOffset = Controller.Theme.FooterTextShadowOffset;
			}
			return footerLabel;
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
		
		protected MemberInfo GetMemberFromViewModel(string memberName)
		{
			MemberInfo memberInfo = null;
			var dc = Controller.RootView as IDataContext<object>;
			if (dc != null)
			{
				memberInfo = dc.DataContext.GetType().GetMember(memberName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SingleOrDefault();
			}

			return memberInfo;
		}

		public NSIndexPath ResetIndexPathRow(NSIndexPath indexPath)
		{
			var listCount = 0;

			var listSource = GetListSource(NSIndexPath.FromRowSection(0, indexPath.Section));
			if (listSource != null && !listSource.IsRootCell)
			{
				listCount = listSource.Sections[0].DataContext.Count;
			
				if (indexPath.Row >= listCount)
					indexPath = NSIndexPath.FromRowSection(indexPath.Row - listCount + 1, indexPath.Section);
			}

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

			if (IsSelectable)
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
}


