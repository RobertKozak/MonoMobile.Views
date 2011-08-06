namespace Samples
{
	using System;
	using System.Threading;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
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
			MonoMobileApplication.NavigationController = _Navigation;
			
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
					_Navigation.ViewControllers = new UIViewController[] { new DialogViewController("Sample", new TestView(), true) { Autorotate = true } };
					
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

