namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class SkywardTheme: Theme
	{
		public SkywardTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/bluesky.jpg");
			SeparatorColor = UIColor.Black;
			
//			TextColor = UIColor.White.ColorWithAlpha(0.75f);
//			TextShadowColor = UIColor.Blue.ColorWithAlpha(0.25f);
//			TextShadowOffset = new SizeF(0, 1);
//			
//			DetailTextColor = UIColor.White.ColorWithAlpha(0.75f);
//			DetailTextShadowColor = UIColor.Blue.ColorWithAlpha(0.25f);
//			DetailTextShadowOffset = new SizeF(0, 1);
		}
	}
}
