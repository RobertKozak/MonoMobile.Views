namespace Samples
{
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	public class Application
	{
		public static UIWindow Window { get; set; }
		public static UINavigationController Navigation { get; set; }

		static void Main(string[] args)
		{
			UIApplication.Main(args, null, "AppDelegate");
		}	
	}
}

