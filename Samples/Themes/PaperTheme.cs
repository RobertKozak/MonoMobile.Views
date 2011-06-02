namespace Samples
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
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