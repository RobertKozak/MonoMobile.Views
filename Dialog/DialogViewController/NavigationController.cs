// 
//  NavigationController.cs
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
	using System.Linq;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class NavigationController : UINavigationController
	{
		public NavigationController()
		{}
		public NavigationController(NSCoder coder) : base(coder)
		{}
		public NavigationController(NSObjectFlag t) : base(t)
		{}
		public NavigationController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{}
		public NavigationController(UIViewController rootViewController) : base(rootViewController)
		{}

		public override UIViewController[] PopToViewController(UIViewController viewController, bool animated)
		{
			var oldViewController = VisibleViewController; 
			var controllers = base.PopToViewController(viewController, animated);

			DisposeOldViewController(oldViewController);

			return controllers;
		}

		public override UIViewController[] PopToRootViewController(bool animated)
		{
			var oldViewController = VisibleViewController; 
			var controllers = base.PopToRootViewController(animated);
			
			DisposeOldViewController(oldViewController);

			return controllers;
		}

		public override UIViewController PopViewControllerAnimated(bool animated)
		{
			var oldViewController = VisibleViewController; 
			var controller = base.PopViewControllerAnimated(animated);

			DisposeOldViewController(oldViewController);

			return controller;
		}

		private void DisposeOldViewController(UIViewController controller)
		{
		//	DidReceiveMemoryWarning();
			controller.Dispose();
		}
	}
}

