using System.Drawing;
namespace Samples
{
	using System;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class PaperTheme: Theme
	{
		public PaperTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/paper.jpg");
			SeparatorColor = UIColor.DarkGray;
			TextColor = UIColor.FromWhiteAlpha(0.1f, 0.8f);
			TextShadowColor = UIColor.White;
			TextShadowOffset = new SizeF(0, 1);
			
			DetailTextColor = UIColor.DarkTextColor;
		}
	}
}