// 
//  HtmlView.cs
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
	using System.Drawing;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class HtmlView : CellView, ISelectable
	{
		public UIWebView Web { get; set; }

		public Uri Uri { get { return DataContext.Value is Uri ? DataContext.Value as Uri : new Uri(DataContext.Value.ToString()); } }

		public HtmlView(RectangleF frame) : base(frame)
		{
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
			HtmlView container;

			public WebViewController(HtmlView container) : base()
			{
				this.container = container;
			}

			public override void ViewWillDisappear(bool animated)
			{
				base.ViewWillDisappear(animated);
				NetworkActivity = false;
				if (container.Web == null)
					return;
				
				container.Web.StopLoading();
				container.Web.Dispose();
				container.Web = null;
			}

			public bool Autorotate { get; set; }

			public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			var vc = new WebViewController(this) { Autorotate = controller.Autorotate };
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

			vc.Title = Caption;
			vc.View.AddSubview(Web);
			
			controller.ActivateController(vc, controller);

			var url = new NSUrl(Uri.AbsoluteUri);
			Web.LoadRequest(NSUrlRequest.FromUrl(url));
		}
	}
}

