namespace MVVMSample
{
	using System;
	using System.Drawing;
	using System.IO;
	using MonoMobile.MVVM;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	[Preserve(AllMembers = true)]
	public class SampleTheme : Theme
	{
		public SampleTheme()
		{
			Name = "SampleTheme";
			
			BarTintColor = UIColor.FromRGB(50, 100, 200);
			BarStyle = UIBarStyle.Default;
		
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Images/bluesky.jpg"));
			
			CellBackgroundColor = UIColor.Clear;
			
			SeparatorColor = UIColor.FromWhiteAlpha(0.4f, 1.0f);
			SeparatorStyle = UITableViewCellSeparatorStyle.None;
			PlaceholderColor = UIColor.FromWhiteAlpha(0.4f, 1.0f);
			DetailTextColor = UIColor.FromWhiteAlpha(0.1f, 0.8f);
			DetailTextAlignment = UITextAlignment.Right;
			PlaceholderAlignment = UITextAlignment.Right;
			
			HeaderTextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			HeaderTextShadowColor = UIColor.FromWhiteAlpha(0f, 0.2f);
			
			FooterTextColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			FooterTextShadowColor = UIColor.FromWhiteAlpha(0f, 0.2f);
			
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

