namespace Samples
{
	using MonoTouch.Dialog;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using System.Threading;

	// The name AppDelegateIPhone is referenced in the MainWindowIPhone.xib file.
	public partial class AppDelegateIPhone : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			window.AddSubview(navigation.View);
			
			// this method initializes the main menu Dialog
			var startupThread = new Thread(Startup as ThreadStart);
			startupThread.Start();
			
			window.MakeKeyAndVisible();
			
			return true;
		}

		[Export("Startup")]
		private void Startup()
		{
			using (var pool = new NSAutoreleasePool()) 
			{
				InvokeOnMainThread(delegate 
				{
					var binding = new BindingContext(new MovieListView(), "DealerCenter");
					navigation.ViewControllers = new UIViewController[] { new DialogViewController(UITableViewStyle.Grouped, binding, true) };//{Autorotate = true } };
				});
			}
		}
	}
}

