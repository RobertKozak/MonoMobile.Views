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

	public class ListView : UITableView
	{
		private IRoot _Root;
		
		public bool IsDirty { get; private set; }
		public bool IsReloading { get; private set; }
		public UITableViewStyle Style { get; private set; }
		public UIImage BackgroundImage { get; set; }
		public UIViewController ParentViewController { get; set; }
		
		
		/// <summary>
		/// The _Root element displayed by the DialogViewController, the value can be changed during runtime to update the contents.
		/// </summary>
		public IRoot Root
		{
			get { return _Root; }
			set
			{
				if (_Root == value)
					return;
				if (_Root != null)
					_Root.Dispose();

				_Root = value;
				_Root.TableView = this;
				ReloadData();
			}
		}
		
		//Controller Actions
		public Action<bool> ViewWillAppear { get; private set; }
		public Action<UIInterfaceOrientation> DidRotate { get; private set; }
		
		
		public ListView()
		{
			
		}
		
		public ListView(RectangleF bounds, UITableViewStyle style = UITableViewStyle.Grouped) : base(bounds, style)
		{
			
		}
		
		public ListView(UIViewController parentViewController, RectangleF bounds,UITableViewStyle style = UITableViewStyle.Grouped) : base(bounds, style)
		{
			ParentViewController = parentViewController;
		}
		
		public void LoadView()
		{
			this.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			this.AutosizesSubviews = true;
			
			if (Root != null)
			{
				var themeable = Root as IThemeable;
				
				if (themeable != null)
				{
					
					if (themeable.Theme.TableViewStyle.HasValue)
						Style = themeable.Theme.TableViewStyle.Value;
					
					var separatorColor = themeable.Theme.SeparatorColor;
					if (separatorColor != null)
						this.SeparatorColor = separatorColor;

					var separatorStyle = themeable.Theme.SeparatorStyle;
					if (separatorStyle.HasValue)
						this.SeparatorStyle = separatorStyle.Value;
				}				
				
				this.Source = CreateSizingSource(Root.UnevenRows);
				ConfigureBackgroundImage();
				
			}
			
		}
		
		
		public virtual ListViewSizingSource CreateSizingSource(bool unevenRows)
		{
			return unevenRows ? new ListViewSizingSource(this) : new ListViewSizingSource(this);
		}
		
		

		
		private void ConfigureBackgroundImage()
		{
			UIColor color = null;
			if (BackgroundImage == null)
			{
				if (Root.Theme.BackgroundUri != null)
				{
					var imageUri = Root.Theme.BackgroundUri;
					BackgroundImage = ImageLoader.DefaultRequestImage(imageUri, null);
				}

				if (Root.Theme.BackgroundColor != null)
				{
					color = Root.Theme.BackgroundColor;
				}

				if (Root.Theme.BackgroundImage != null)
				{
					BackgroundImage = Root.Theme.BackgroundImage;
				}
			}

			if (BackgroundImage != null)
			{
				color = UIColor.FromPatternImage(BackgroundImage);
			}

			if (color != null)
			{
				if (this.RespondsToSelector(new Selector("backgroundView")))
					BackgroundView = new UIView() { Opaque = false };

				if (ParentViewController != null)
				{
					this.BackgroundColor = UIColor.Clear;
					ParentViewController.View.BackgroundColor = color;
				} else
				{
					this.BackgroundColor = color;
				}
			}
		}

		
		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			if (Root != null)
			{
				foreach (var section in Root.Sections)
				{
					foreach (var element in section.Elements)
					{
						var selected = element as EntryElement;
						if (selected != null && selected.Entry != null && selected.Entry.IsFirstResponder)
						{
							selected.Entry.ResignFirstResponder();
							break;
						}
					}
				}	
			}

			base.TouchesEnded(touches, evt);
		}
		
		public void Selected(NSIndexPath indexPath)
		{
			var element = Root.Sections [indexPath.Section].Elements [indexPath.Row];
			var selectable = element as IListSelectable;

			if (selectable != null)
			{
				if (element.Cell.SelectionStyle != UITableViewCellSelectionStyle.None)
				{
					DeselectRow(indexPath, false);
					UIView.Animate(0.3f, delegate { element.Cell.Highlighted = true;  }, delegate { element.Cell.Highlighted = false; });
				}

				selectable.Selected(this, indexPath);
			}
		}
		
		private void ViewDidAppearHandler(bool animated)
		{
			if (Root != null)
			{
				Root.Prepare();
				
				if (IsDirty)
				{
					this.ReloadData();
					IsDirty = false;
				}
				
				var index = Root.ItemIndex;
				if (index > -1)
				{
					var path = Root.PathForRadio();
					this.ScrollToRow(path, UITableViewScrollPosition.Top, false);
				}

			}
		}
		
		}
	
}