//
// DialogViewController.cs: drives MonoMobile.MVVM
//
// Author:
//   Miguel de Icaza
//   With changes by Robert Kozak, Copyright 2011, Nowcom Corporation
//
// Code to support pull-to-refresh based on Martin Bowling's TweetTableView
// which is based in turn in EGOTableViewPullRefresh code which was created
// by Devin Doty and is Copyrighted 2009 enormego and released under the
// MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoMobile.MVVM
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using MonoMobile.MVVM.Utilities;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.ObjCRuntime;
	using MonoTouch.UIKit;

	public class ListViewSource : UITableViewSource
	{
		protected IRoot Root;
		protected ListView Container;

		public ListViewSource(ListView listViewContainer)		{
			Container = listViewContainer;
			Root = listViewContainer.Root;
		}

		public override int RowsInSection(UITableView tableview, int section)
		{
			return Root.Sections [section].Elements.Count;
		}

		public override int NumberOfSections(UITableView tableView)
		{
			return Root.Sections.Count;
		}

		public override string TitleForHeader(UITableView tableView, int section)
		{
			return Root.Sections [section].Caption;
		}

		public override string TitleForFooter(UITableView tableView, int section)
		{
			return Root.Sections [section].FooterText;
		}

		public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var element = Root.Sections [indexPath.Section].Elements [indexPath.Row];
			return element.GetCell(tableView) as UITableViewElementCell;
		}

		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			Container.Selected(indexPath);
		}
		
		public override UIView GetViewForHeader(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections [sectionIdx];
			if (section.HeaderView == null && !string.IsNullOrWhiteSpace(section.Caption))
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
			var section = Root.Sections [sectionIdx];
			if (section.HeaderView == null)
				return -1;
			return section.HeaderView.Frame.Height;
		}
		
		private UIView CreateHeaderView(UITableView tableView, string caption)
		{
			var bounds = tableView.Bounds;
			var headerLabel = new UILabel();

			headerLabel.Font = UIFont.BoldSystemFontOfSize(UIFont.LabelFontSize);
			var size = headerLabel.StringSize(caption, headerLabel.Font);
			var rect = new RectangleF(bounds.X + 20, bounds.Y, bounds.Width - 20, size.Height + 10);

			headerLabel.Bounds = rect;
			headerLabel.Frame = headerLabel.Bounds;
			headerLabel.BackgroundColor = UIColor.Clear;
			headerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			headerLabel.ShadowColor = UIColor.White;
			headerLabel.ShadowOffset = new SizeF(0, 1);
			headerLabel.Text = caption;

			var view = new UIView(rect);
			view.AddSubview(headerLabel);

			return view;
		}
		
		public override UIView GetViewForFooter(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections [sectionIdx];
			if (section.FooterView == null && !string.IsNullOrEmpty(section.FooterText))
			{
				section.FooterView = CreateFooterView(tableView, section.FooterText);
				var themeable = section as IThemeable;
				if (themeable != null)
					themeable.ThemeChanged();
			}
			return section.FooterView;
		}
		
		public override float GetHeightForFooter(UITableView tableView, int sectionIdx)
		{
			var section = Root.Sections [sectionIdx];
			if (section.FooterView == null)
				return -1;
			return section.FooterView.Frame.Height;
		}
		
		private UIView CreateFooterView(UITableView tableView, string caption)
		{
			var bounds = tableView.Bounds;
			var footerLabel = new UILabel();

			footerLabel.Font = UIFont.SystemFontOfSize(15);
			var size = footerLabel.StringSize(caption, footerLabel.Font);
			var height = size.Height * (caption.Count((ch) => ch == '\n') + 1); 
			caption = caption.Replace("\n", "");
			var rect = new RectangleF(bounds.X + 10, bounds.Y, bounds.Width - 20, height + 10);

			footerLabel.Bounds = rect;
			footerLabel.BackgroundColor = UIColor.Clear;
			footerLabel.TextAlignment = UITextAlignment.Center;
			footerLabel.TextColor = UIColor.FromRGB(76, 86, 108);
			footerLabel.ShadowColor = UIColor.White;
			footerLabel.ShadowOffset = new SizeF(0, 1);
			footerLabel.Text = caption;

			return footerLabel;
		}


	}
	
	//
	// Performance trick, if we expose GetHeightForRow, the UITableView will
	// probe *every* row for its size;   Avoid this by creating a separate
	// model that is used only when we have items that require resizing
	//
	public class ListViewSizingSource : ListViewSource
	{
		public ListViewSizingSource(ListView listView) : base(listView)		
		{
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var element = Root.Sections [indexPath.Section].Elements [indexPath.Row];

			var sizable = element as ISizeable;
			if (sizable != null)
				return sizable.GetHeight(tableView, indexPath);

			return tableView.RowHeight;
		}
	}
		
}

