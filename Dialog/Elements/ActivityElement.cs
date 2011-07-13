//
// ActivityElement.cs
//
// Author:
//  Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
//
// Code licensed under the MIT X11 license
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
	using System.Drawing;
	using MonoTouch.UIKit;
	using MonoMobile.MVVM;

	public class ActivityElement : UIViewElement
	{
		public bool Animating { get; set; }

		public ActivityElement() : base ("", new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray), false)
		{
			DataBinding = new ActivityElementDataBinding(this);

			var sbounds = UIScreen.MainScreen.Bounds;			
			var uia = ContentView as UIActivityIndicatorView;
			
			uia.StartAnimating();
			
			var vbounds = ContentView.Bounds;
			ContentView.Frame = new RectangleF((sbounds.Width-vbounds.Width)/2, 4, vbounds.Width, vbounds.Height + 0);
			ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
		}
		
//		public bool Animating {
//			get
//			{
//				return ((UIActivityIndicatorView)ContentView).IsAnimating;
//			}
//			set
//			{
//				var activity = ContentView as UIActivityIndicatorView;
//
//				if (value)
//					activity.StartAnimating();
//				else
//					activity.StopAnimating();
//			}
//		}
		
		public override float GetHeight(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return base.GetHeight(tableView, indexPath)+ 8;
		}		
	}
}

