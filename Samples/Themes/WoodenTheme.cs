namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers=true)]
	public class WoodenTheme: Theme
	{
		public WoodenTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/wood.jpg");
			SeparatorColor = UIColor.Black;
			
			TextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			TextShadowColor = UIColor.Brown.ColorWithAlpha(0.2f);
			TextShadowOffset = new SizeF(0, 1);
			
			DetailTextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			DetailTextShadowColor = UIColor.Brown.ColorWithAlpha(0.2f);
			DetailTextShadowOffset = new SizeF(0, 1);
		}
	}
}
