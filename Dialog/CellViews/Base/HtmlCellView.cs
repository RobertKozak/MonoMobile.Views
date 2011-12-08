// 
//  HtmlCellView.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
//  
//  based on code by
//    Miguel de Icaza (miguel@gnome.org)
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
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class HtmlCellView : CellView<Uri>, ISelectable
	{
		private WebViewController _WebViewController;
		public UIWebView Web { get; set; }

		public HtmlCellView(RectangleF frame) : base(frame)
		{
		}
		
		protected override void Dispose(bool disposing)
		{
			_WebViewController.Dispose();
			base.Dispose(disposing);
		}

		public override void UpdateCell(UITableViewCell cell, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			cell.TextLabel.Text = Caption;
		}
		
		static bool NetworkActivity
		{
			set { UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value; }
		}

		// We use this class to dispose the web control when it is not
		// in use, as it could be a bit of a pig, and we do not want to
		// wait for the GC to kick-in.
		class WebViewController : UIViewController
		{
			private HtmlCellView _Container;

			public WebViewController(HtmlCellView container) : base()
			{
				_Container = container;
			}

			protected override void Dispose(bool disposing)
			{
				_Container.Dispose();

				base.Dispose(disposing);
			}

			public override void ViewWillDisappear(bool animated)
			{
				NetworkActivity = false;
				if (_Container.Web != null)
				{
					_Container.Web.StopLoading();
					_Container.Web.RemoveFromSuperview();
					_Container.Web.Dispose();
					_Container.Web = null;
				}
	
				base.ViewWillDisappear(animated);
			}

			public bool Autorotate { get; set; }

			public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			var frame = UIScreen.MainScreen.Bounds;

			Web = new UIWebView(frame) { BackgroundColor = UIColor.White, ScalesPageToFit = true, AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight };
			Web.LoadStarted += delegate { NetworkActivity = true; };
			Web.LoadFinished += delegate { NetworkActivity = false; };
			Web.LoadError += (webview, args) =>
			{
				NetworkActivity = false;
				if (Web != null)
					Web.LoadHtmlString(String.Format("<html><center><font size=+5 color='red'>An error occurred:<br>{0}</font></center></html>", args.Error.LocalizedDescription), null);
			};

			_WebViewController = new WebViewController(this) { Autorotate = controller.Autorotate };
			_WebViewController.Title = Caption;
			_WebViewController.View.AddSubview(Web);
			
			controller.ActivateController(_WebViewController, controller);

			var url = new NSUrl(Value.AbsoluteUri);
			Web.LoadRequest(NSUrlRequest.FromUrl(url));
		}
	}
}

