using MonoTouch.CoreGraphics;
namespace Samples
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.Views;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;

	[Preserve(AllMembers = true)]
	public class WoodenTheme: Theme
	{
		public WoodenTheme()
		{
			CellBackgroundColor = UIColor.FromWhiteAlpha(1f, 0.2f);

			TextColor = UIColor.DarkTextColor;
			TextShadowColor = UIColor.FromWhiteAlpha(0.8f, 1.0f);
			TextShadowOffset = new SizeF(0, 1);

			BackgroundUri = new Uri ("file://" + Path.GetFullPath("Images/wood.jpg"));
			SeparatorColor = UIColor.Black;
			
			TextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			TextShadowColor = UIColor.Brown.ColorWithAlpha(0.8f);
			TextShadowOffset = new SizeF(0, 1);
			
			DetailTextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			DetailTextShadowColor = UIColor.Brown.ColorWithAlpha(0.8f);
			DetailTextShadowOffset = new SizeF(0, 1);

			BarTintColor = UIColor.Brown;
			BarStyle = UIBarStyle.Default;

			HeaderTextColor = UIColor.FromRGB(92, 69, 0);
			HeaderTextShadowColor = UIColor.FromWhiteAlpha(1f, 0.6f);

			FooterTextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			FooterTextShadowColor = UIColor.FromWhiteAlpha(0f, 0.2f);
		}
	}
}
