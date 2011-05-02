using System.Drawing;
namespace Samples
{
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using System.Threading;

	// The name AppDelegateIPhone is referenced in the MainWindowIPhone.xib file.
	public partial class AppDelegateIPhone : UIApplicationDelegate
	{
		ListView FooListView { get; set; }
		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			//			window.AddSubview(navigation.View);
			FooListView = new ListView();
			FooListView.Root = new RootElement()
			{
				new Section()
				{
					new StringElement("foo"),
				}
			};
			
			FooListView.Frame = window.Bounds;
			FooListView.LoadView();
			
			window.AddSubview(FooListView);
			
			
			// this method initializes the main menu Dialog
			//			var startupThread = new Thread(Startup as ThreadStart);
			//			startupThread.Start();
			
			window.MakeKeyAndVisible();
			
			return true;
		}

		private void Startup()
		{
			using (var pool = new NSAutoreleasePool()) 
			{
				InvokeOnMainThread(delegate 
				{
					var binding = new BindingContext(new MovieListView(), "MVVM Sample");
					navigation.ViewControllers = new UIViewController[] { new DialogViewController(UITableViewStyle.Grouped, binding, true) };//{Autorotate = true } };
				});
			}
		}
	}
}

