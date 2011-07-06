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
namespace MonoMobile.MVVM
{
	using System;
	using MonoTouch.UIKit;
	using System.Drawing;
	using System.Linq;
	using MonoTouch.Foundation;

	public class DialogViewDataSource : UITableViewSource
	{
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
			
			return count;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Root.Sections.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var section = Root.Sections[indexPath.Section];
			var element = section.Elements[indexPath.Row];
			
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

		public override string TitleForHeader(UITableView tableView, int section)
		{
			return Root.Sections[section].Caption;
		}

		public override string TitleForFooter(UITableView tableView, int section)
		{
			return Root.Sections[section].FooterText;
		}

		public override UIView GetViewForHeader(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections[sectionIdx];
			if (section.HeaderView == null && !string.IsNullOrEmpty(section.Caption))
			{
				section.HeaderView = CreateHeaderView(tableView, section.Caption);
				var themeable = section as IThemeable;
				if (themeable != null)
					themeable.ThemeChanged();
			}
			return section.HeaderView;
		}

		public override float GetHeightForHeader(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections[sectionIdx];
			if (section.HeaderView == null)
				return -1;
			return section.HeaderView.Frame.Height;
		}

		public override UIView GetViewForFooter(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections[sectionIdx];
			if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
			{
				section.FooterView = CreateFooterView(tableView, section.FooterText);
				var themeable = section as IThemeable;
				if (themeable != null)
					themeable.ThemeChanged();
			}
			
			// Use an empty UIView to Eliminate Extra separators for blank items
			if (section.FooterView == null)
				return new UIView(RectangleF.Empty) { BackgroundColor = UIColor.Clear };
			
			return section.FooterView;
		}

		public override float GetHeightForFooter(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections[sectionIdx];
			if (section.FooterView == null)
				return -1;
			return section.FooterView.Frame.Height;
		}

		private UIView CreateHeaderView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			
			var headerLabel = new UILabel();
			
			headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
			var size = headerLabel.StringSize(caption, headerLabel.Font);
			
			var bounds = new RectangleF(tableView.Bounds.X, tableView.Bounds.Y, tableView.Bounds.Width, size.Height + 10);
			var frame = new RectangleF(bounds.X + indentation + 10, bounds.Y, bounds.Width - (indentation + 10), size.Height + 10);
			
			headerLabel.Bounds = bounds;
			headerLabel.Frame = frame;
			
			headerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			headerLabel.ShadowColor = UIColor.White;
			headerLabel.ShadowOffset = new SizeF(0, 1);
			headerLabel.Text = caption;
			
			var view = new UIView(bounds) { BackgroundColor = tableView.BackgroundColor };
			
			if (tableView.Style == UITableViewStyle.Grouped)
			{
				headerLabel.BackgroundColor = UIColor.Clear;
				view.Opaque = false;
				view.BackgroundColor = UIColor.Clear;
			}
			
			view.AddSubview(headerLabel);
			
			return view;
		}

		private UIView CreateFooterView(UITableView tableView, string caption)
		{
			var indentation = UIDevice.CurrentDevice.GetIndentation();
			
			var bounds = tableView.Bounds;
			var footerLabel = new UILabel();
			var width = bounds.Width - (indentation * 2);
			var linefeeds = caption.Count(ch => ch == '\n');
			
			footerLabel.Font = UIFont.SystemFontOfSize(15);
			var size = footerLabel.StringSize(caption, footerLabel.Font);
			
			footerLabel.Lines = 1 + ((int)(size.Width / width)) + linefeeds;
			if (size.Width % width == 0)
				footerLabel.Lines--;
			
			var height = size.Height * (footerLabel.Lines);
			
			var rect = new RectangleF(bounds.X + indentation, bounds.Y, width, height + 10);
			
			footerLabel.Bounds = rect;
			footerLabel.BackgroundColor = UIColor.Clear;
			footerLabel.TextAlignment = UITextAlignment.Center;
			footerLabel.LineBreakMode = UILineBreakMode.WordWrap;
			footerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			footerLabel.ShadowColor = UIColor.White;
			footerLabel.ShadowOffset = new SizeF(0, 1);
			footerLabel.Text = caption;
			
			return footerLabel;
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return Container.Root.Sections[indexPath.Section].Elements[indexPath.Row].EditingStyle != UITableViewCellEditingStyle.None;
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
	}
}

