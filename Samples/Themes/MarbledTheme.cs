namespace Samples
{
	using System;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class MarbledTheme: Theme
	{
		public MarbledTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/marble.png");
			SeparatorColor = UIColor.Black;
		}
	}
}

