namespace Samples
{
	using System;
	using System.IO;
	using MonoMobile.Views;
	using MonoTouch.UIKit;

	public class CorkTheme: Theme
	{
		public CorkTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/cork.png");
			SeparatorColor = UIColor.Black;

			Name= "Corked";
		}
	}
}
