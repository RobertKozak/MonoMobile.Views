namespace DealerCenter
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public class BlueButtonTheme : Theme
	{
		public BlueButtonTheme()
		{
			Name = "BlueButtonTheme";
			
			CellBackgroundColor = UIColor.FromRGBA(33, 114, 255, 128);

			TextColor = UIColor.White;
			TextShadowColor = UIColor.DarkGray;
			TextShadowOffset = new SizeF(0, 1);
		}
	}
}

