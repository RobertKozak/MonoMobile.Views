// 
//  MonoMobileAppDelegate.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Threading;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	

	[Register("MonoMobileAppDelegate")]
	public class MonoMobileAppDelegate : UIApplicationDelegate
	{
		private static UIImage _DefaultImage = UIImage.FromFile("Default.png");
		
		private UIWindow _Window;
		private UINavigationController _Navigation;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			_Navigation = new UINavigationController();

            _Window = new UIWindow(UIScreen.MainScreen.Bounds);
			
			if (_DefaultImage != null)
				_Window.BackgroundColor = UIColor.FromPatternImage(_DefaultImage);
		
			_Navigation.View.Alpha = 0.0f;

			_Window.AddSubview(_Navigation.View);
			_Window.MakeKeyAndVisible();
			
			MonoMobileApplication.Window = _Window;
			MonoMobileApplication.Navigation = _Navigation;
			MonoMobileApplication.MainView = Activator.CreateInstance(MonoMobileApplication.MainViewType) as UIView;

			// this method initializes the main NavigationController
			var startupThread = new Thread(Startup);
			startupThread.Start();
			
			return true;
		}

		private void Startup()
		{
			using (var pool = new NSAutoreleasePool()) 
			{
				InvokeOnMainThread(delegate 
				{
					var binding = new BindingContext(MonoMobileApplication.MainView, MonoMobileApplication.Title);
					_Navigation.ViewControllers = new UIViewController[] { new DialogViewController(UITableViewStyle.Grouped, binding, true) { Autorotate = true } };

					UIView.BeginAnimations("fadeIn");
					UIView.SetAnimationDuration(0.3f);
					_Navigation.View.Alpha = 1.0f;
					UIView.CommitAnimations();
				});
			}
		}

        // This method is allegedly required in iPhoneOS 3.0
        public override void OnActivated(UIApplication application)
        {
        }
	}
}

