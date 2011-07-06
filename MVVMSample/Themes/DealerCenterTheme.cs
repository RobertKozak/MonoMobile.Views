namespace DealerCenter
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public class DealerCenterTheme : Theme
	{
		public DealerCenterTheme()
		{
			Name = "DealerCenterTheme";
			
			//BarTintColor = UIColor.FromRGB(50, 100, 200);
			BarTintColor = UIColor.FromRGB(18, 115, 188);
			BarStyle = UIBarStyle.Default;
		
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Default.png"));
			//BackgroundColor = UIColor.ScrollViewTexturedBackgroundColor;

			CellBackgroundColor = UIColor.Clear;
			
			TextColor = UIColor.Black;
			TextShadowColor = UIColor.FromWhiteAlpha(0.9f, 1f);
			TextShadowOffset = new SizeF(0, 1);

			SeparatorColor = UIColor.Gray;
			SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
			PlaceholderColor = UIColor.FromWhiteAlpha(0.4f, 1.0f);
			DetailTextColor = UIColor.FromWhiteAlpha(0.1f, 0.8f);
			DetailTextAlignment = UITextAlignment.Right;
			PlaceholderAlignment = UITextAlignment.Right;
			
			HeaderTextColor = UIColor.FromRGB(37, 93, 177);
			HeaderTextShadowColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			FooterTextShadowOffset = new SizeF(1, 0);
			
			FooterTextColor = UIColor.FromRGB(37, 93, 177);
			FooterTextShadowColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			FooterTextShadowOffset = new SizeF(1, 0);
			
			DrawContentViewAction = (rect, context, cell) => DrawContentView(rect, context, cell);
		}

		public void DrawContentView(RectangleF rect, CGContext context, UITableViewElementCell cell)
		{
			context.SaveState();
			
			var backgroundColor = CellBackgroundColor;
			if (backgroundColor != null)
			{
				if (backgroundColor == UIColor.Clear)
					backgroundColor = UIColor.White;
				
				context.SetFillColorWithColor(backgroundColor.ColorWithAlpha(0.4f).CGColor);
				context.SetBlendMode(CGBlendMode.Overlay);
				context.FillRect(rect);
			}
			
			context.RestoreState();
		}
	}
}

