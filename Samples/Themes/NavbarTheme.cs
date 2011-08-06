namespace Samples
{
	using System;
	using MonoMobile.Views;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class NavbarTheme: Theme
	{
		public NavbarTheme()
		{
			Name = "NavbarTheme";

			BarTintColor = UIColor.FromRGB(50, 100, 200);
			BarStyle = UIBarStyle.Default;
			//BarTranslucent = true;
		}
	}
}

