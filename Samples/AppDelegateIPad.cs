
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Samples
{

	// The name AppDelegateIPad is referenced in the MainWindowIPad.xib file.
	public partial class AppDelegateIPad : UIApplicationDelegate
	{
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window.AddSubview (navigation.View);
			
			// this method initializes the main menu Dialog
			var startupThread = new Thread (Startup as ThreadStart);
			startupThread.Start ();
			
			window.MakeKeyAndVisible ();
			return true;
		}

		[Export("Startup")]
		private void Startup ()
		{
			using (var pool = new NSAutoreleasePool ()) {
				InvokeOnMainThread (delegate {
					var binding = new BindingContext (new MovieListView (), "Movie List View");
					navigation.ViewControllers = new UIViewController[] { new DialogViewController (binding.Root, true) };
				});
			}
		}
	}
}

