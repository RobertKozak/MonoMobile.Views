namespace Samples
{
	using System;
	using System.IO;
	using MonoMobile.Views;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class MarbledTheme: Theme
	{
		public MarbledTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/marble.png");
			SeparatorColor = UIColor.Black;
		}
	}
}

