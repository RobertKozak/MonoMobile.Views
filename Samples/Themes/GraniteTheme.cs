namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class GraniteTheme: Theme
	{
		public GraniteTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/granite.jpg");
			SeparatorColor = UIColor.Black;
			TextShadowColor = UIColor.LightGray;
			TextShadowOffset = new SizeF(-1, -1);
		}
	}
}
