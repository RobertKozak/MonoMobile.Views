namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.Views;
	using MonoTouch.UIKit;

	public class AutumnTheme : Theme
	{
		public AutumnTheme()
		{
			CellBackgroundImage = UIImage.FromFile("Images/leaves.jpg");
			SeparatorColor = UIColor.Black;
			
			TextColor = UIColor.White;
			TextShadowColor = UIColor.Brown;
			TextShadowOffset = new SizeF(1, 0);

			DetailTextColor = UIColor.White;
			DetailTextShadowColor = UIColor.Brown;
			DetailTextShadowOffset = new SizeF(0, 1);

			Name = "Autumn";
		}
	}
}
