using MonoTouch.CoreGraphics;
// 
//  DialogViewDataSource.cs
// 
// Author:
//   Miguel de Icaza
//   With changes by Robert Kozak, Copyright 2011, Nowcom Corporation
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
	using System.Drawing;
	using System.Linq;
	using MonoTouch.CoreAnimation;
	using MonoTouch.Foundation;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;

	public class DialogViewDataSource : UITableViewSource
	{
		private HeaderTapGestureRecognizer tapRecognizer;
		private Selector _HeaderSelector = new Selector("headerTap");

		private const float _SnapBoundary = 65;
		protected DialogViewController Container;
		protected IRoot Root;
		private bool _CheckForRefresh;

		public DialogViewDataSource(DialogViewController container)
		{
			Container = container;
			Root = container.Root;
		}
		
		public override int RowsInSection(UITableView tableview, int section)
		{
			var s = Root.Sections[section];
			var count = s.Elements.Count;
			
			return s.ExpandState == ExpandState.Opened ? count : 0;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Root.Sections.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var element = GetElement(indexPath);

			return element.GetCell(tableView) as UITableViewElementCell;
		}

		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			Container.Selected(indexPath);
		}

		#region Pull to Refresh support
		public override void Scrolled(UIScrollView scrollView)
		{
			if (!_CheckForRefresh)
				return;
			if (Container.Reloading)
				return;
			var view = Container.RefreshView;
			if (view == null)
				return;
			
			var point = Container.TableView.ContentOffset;
			
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
			if (Container.RefreshView == null)
				return;
			
			_CheckForRefresh = false;
			if (Container.TableView.ContentOffset.Y > -_SnapBoundary)
				return;
			
			Container.TriggerRefresh(true);
		}
		#endregion

		public override string TitleForHeader(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);
			if (section != null)
				return Root.Sections[sectionIndex].Caption;

			return string.Empty;
		}

		public override string TitleForFooter(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);
			if (section != null)
				return Root.Sections[sectionIndex].FooterText;
		
			return string.Empty;
		}

		public override UIView GetViewForHeader(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);
			if (section != null)
			{
				if (section.HeaderView == null && !string.IsNullOrEmpty(section.Caption))
				{
					section.HeaderView = CreateHeaderView(tableView, section.Caption);
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
					section.ExpandState = ExpandState.Closed;
				else
					section.ExpandState = ExpandState.Opened;
			
				Flip(section);
			}
		}

		public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
		{
			var element = GetElement(indexPath);

			if (element.AccessoryCommand != null && element.AccessoryCommand.CanExecute(null))
			{
				element.AccessoryCommand.Execute(null);
			}
		}
		
		private void Flip(ISection section)
		{
			UIView.BeginAnimations(null);
			UIView.SetAnimationDuration(0.18f);
			section.ArrowView.Layer.Transform = section.ExpandState == ExpandState.Closed ? CATransform3D.MakeRotation((float)Math.PI * 1.5f, 0, 0, 1) : CATransform3D.MakeRotation((float)Math.PI * 2f, 0, 0, 1);
			
			UIView.CommitAnimations();
		}

		public override float GetHeightForHeader(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);
			
			if (section != null && !string.IsNullOrEmpty(section.HeaderText) && section.HeaderView != null)
			{
				var indentation = UIDevice.CurrentDevice.GetIndentation();
				var width = tableView.Bounds.Width - (indentation * 2);

				var headerLabel = section.HeaderView.Subviews[0] as UILabel;
				if (headerLabel != null)
				{
					headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
					var size = headerLabel.StringSize(section.HeaderText, headerLabel.Font);
			
					var height = (float)(size.Height * (headerLabel.Font.NumberOfLines(section.HeaderText, width) + 0.5));
			
					return height;
				}
			}
			
			return 0;
		}

		public override UIView GetViewForFooter(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);
			if (section != null)
			{
				if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
				{
					section.FooterView = CreateFooterView(tableView, section.FooterText);
				}
				
				// Use an empty UIView to Eliminate Extra separators for blank items
				if (section.FooterView == null || section.ExpandState == ExpandState.Closed)
					return new UIView(RectangleF.Empty) { BackgroundColor = UIColor.Clear };
				
				return section.FooterView;
			}
			
			return null;
		}

		public override float GetHeightForFooter(UITableView tableView, int sectionIndex)
		{
			var section = GetSection(sectionIndex);

			if (section != null && !string.IsNullOrEmpty(section.FooterText))
			{
				var indentation = UIDevice.CurrentDevice.GetIndentation();

				var width = tableView.Bounds.Width - (indentation * 2);
				var linefeeds = section.FooterText.Count(ch => ch == '\n');
				var footerLabel = new UILabel();
			
				footerLabel.Font = UIFont.SystemFontOfSize(15);
				var size = footerLabel.StringSize(section.FooterText, footerLabel.Font);

				var height = size.Height * (footerLabel.Font.NumberOfLines(section.FooterText, width));

				return height;
			}
			
			return 0;			
		}

		private UIView CreateHeaderView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			var width = tableView.Bounds.Width - (indentation * 2);

			var headerLabel = new UILabel();
			headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);

			var size = headerLabel.StringSize(caption, headerLabel.Font);
			var height = size.Height * (headerLabel.Font.NumberOfLines(caption, width));

			var frame = new RectangleF(tableView.Bounds.X + indentation + 10, 4, tableView.Bounds.Width - (indentation + 10), height);
			
			headerLabel.Frame = frame;
			
			headerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			headerLabel.ShadowColor = UIColor.White;
			headerLabel.LineBreakMode = UILineBreakMode.WordWrap;
			headerLabel.ShadowOffset = new SizeF(0, 1);
			headerLabel.Text = caption;
			headerLabel.Lines = headerLabel.Font.NumberOfLines(caption, frame.Width);
			
			var view = new UIView(new RectangleF(0,0, frame.Width, height));

			if (tableView.Style == UITableViewStyle.Grouped)
			{
				headerLabel.BackgroundColor = UIColor.Clear;
				view.Opaque = false;
			}
			else
			{
				var theme = Container.Root.Theme;
				var background = theme.HeaderBackgroundColor;
				if (background != null)
					headerLabel.BackgroundColor = background;
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

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			var element = GetElement(indexPath);

			if (element != null)
			{	
				return element.EditingStyle != UITableViewCellEditingStyle.None;
			}

			return false;
		}

		public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var element = Container.Root.Sections[indexPath.Section].Elements[indexPath.Row];
			return element.EditingStyle;
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete)
			{
				Container.Root.Sections[indexPath.Section].Elements.RemoveAt(indexPath.Row);
				tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
			}
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var element = GetElement(indexPath);
			
			var sizable = element as ISizeable;
			if (sizable != null)
				return sizable.GetHeight(tableView, indexPath);
			
			return tableView.RowHeight;
		}

		public ISection GetSection(int sectionIndex)
		{
			ISection section = null;
			if (Root.Sections.Count > sectionIndex)
				section = Root.Sections[sectionIndex];
			
			return section;
		}

		public IElement GetElement(NSIndexPath indexPath)
		{
			IElement element = null;
			var section = GetSection(indexPath.Section);
			if (section != null && section.Elements != null && section.Elements.Count > indexPath.Row)
				element = section.Elements[indexPath.Row];

			return element;
		}
	}

	public class HeaderTapGestureRecognizer : UITapGestureRecognizer
	{
		public ISection Section { get; set; }
 
		public HeaderTapGestureRecognizer(ISection section, NSObject target, Selector selector): base(target, selector) 
		{
			Section = section;
		}
	}
}

