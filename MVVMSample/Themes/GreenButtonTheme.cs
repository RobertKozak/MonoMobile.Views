namespace DealerCenter
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public class GreenButtonTheme : Theme
	{
		public GreenButtonTheme()
		{
			Name = "GreenButtonTheme";
			
			CellBackgroundColor = UIColor.FromRGBA(0, 99, 9, 128);

			TextColor = UIColor.White;
			TextShadowColor = UIColor.DarkGray;
			TextShadowOffset = new SizeF(0, 1);
		}
	}
}