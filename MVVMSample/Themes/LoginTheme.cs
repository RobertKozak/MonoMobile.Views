namespace DealerCenter
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class LoginTheme: Theme
	{
		public LoginTheme()
		{
			Name = "LoginTheme";

			BarTintColor = UIColor.FromRGB(18, 115, 188);
			BarStyle = UIBarStyle.Default;

			BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;
			CellBackgroundColor = UIColor.White;

			HeaderTextColor = UIColor.White;
			HeaderTextShadowColor = UIColor.Black;
			HeaderTextShadowOffset = new SizeF(0, 1);
			
			FooterTextColor = UIColor.White;
			FooterTextShadowColor = UIColor.Black;
			FooterTextShadowOffset = new SizeF(0, 1);
		}
	}
}

