namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.Views;
	using MonoTouch.UIKit;

	public class BrickedTheme: Theme
	{
		public BrickedTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/brick.jpg");
			SeparatorColor = UIColor.Clear;
			
			TextColor = UIColor.White;
			TextShadowColor = UIColor.DarkGray;
			TextShadowOffset = new SizeF(1, 1);
			
			DetailTextColor = UIColor.White;
			DetailTextShadowColor = UIColor.DarkGray;
			DetailTextShadowOffset = new SizeF(1, 1);

			Name = "Bricked";
		}
	}
}
