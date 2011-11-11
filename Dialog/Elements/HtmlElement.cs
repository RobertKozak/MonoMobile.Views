////
//// HtmlElement.cs
////
//// Author:
////  Miguel de Icaza (miguel@gnome.org)
////
//// Copyright 2010, Novell, Inc.
////
//// Code licensed under the MIT X11 license
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
////
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//namespace MonoMobile.Views
//{
//	using System;
//	using System.IO;
//	using MonoTouch.Foundation;
//	using MonoTouch.UIKit;
//	using MonoMobile.Views;
//
//	/// <summary>
//	/// Used to display a cell that will launch a web browser when selected.
//	/// </summary>
//	[Preserve(AllMembers = true)]
//	public class HtmlElement : Element, ISelectable
//	{
//		private UIWebView web;
//		
//		public Uri Uri { get; set; }
//
//		public HtmlElement(string caption) : base(caption)
//		{
//			DataBinding = new HtmlElementDataBinding(this);
//		}
//
//		public HtmlElement(string caption, Uri url) : this(caption)
//		{
//			Uri = url;
//		}
//		
//		public override void InitializeCell(UITableView tableView)
//		{
//			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//			Cell.TextLabel.Text = Caption;
//
//			base.InitializeCell(tableView);
//		}
//
//		static bool NetworkActivity
//		{
//			set { UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value; }
//		}
//
//		// We use this class to dispose the web control when it is not
//		// in use, as it could be a bit of a pig, and we do not want to
//		// wait for the GC to kick-in.
//		class WebViewController : UIViewController
//		{
//			HtmlElement container;
//
//			public WebViewController(HtmlElement container) : base()
//			{
//				this.container = container;
//			}
//
//			public override void ViewWillDisappear(bool animated)
//			{
//				base.ViewWillDisappear(animated);
//				NetworkActivity = false;
//				if (container.web == null)
//					return;
//				
//				container.web.StopLoading();
//				container.web.Dispose();
//				container.web = null;
//			}
//
//			public bool Autorotate { get; set; }
//
//			public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
//			{
//				return Autorotate;
//			}
//		}
//
//		public void Selected(DialogViewController dvc, UITableView tableView, object item, NSIndexPath path)
//		{
//			var vc = new WebViewController(this) { Autorotate = dvc.Autorotate };
//			var frame = UIScreen.MainScreen.Bounds;
//
//			web = new UIWebView(frame) { BackgroundColor = UIColor.White, ScalesPageToFit = true, AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight };
//			web.LoadStarted += delegate { NetworkActivity = true; };
//			web.LoadFinished += delegate { NetworkActivity = false; };
//			web.LoadError += (webview, args) =>
//			{
//				NetworkActivity = false;
//				if (web != null)
//					web.LoadHtmlString(String.Format("<html><center><font size=+5 color='red'>An error occurred:<br>{0}</font></center></html>", args.Error.LocalizedDescription), null);
//			};
//			vc.NavigationItem.Title = Caption;
//			vc.View.AddSubview(web);
//			
//			dvc.ActivateController(vc, dvc);
//
//			var url = new NSUrl(Uri.AbsoluteUri);
//			web.LoadRequest(NSUrlRequest.FromUrl(url));
//		}
//	}
//}
//
