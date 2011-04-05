using System.Drawing;
namespace Samples
{
	using System;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class MetallicTheme: Theme
	{
		public MetallicTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/metal.jpg");
			SeparatorColor = UIColor.DarkGray;
			TextColor = UIColor.FromWhiteAlpha(0.8f, 1f);
			TextShadowColor = UIColor.DarkGray;
			
			DetailTextColor = UIColor.DarkTextColor;
		}
	}
}

