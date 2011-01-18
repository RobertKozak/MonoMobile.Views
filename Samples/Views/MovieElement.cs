namespace Samples
{
	using System;
	using MonoTouch.UIKit;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Dialog;
	using System.Drawing;
	using MonoTouch.Foundation;

	/// <summary>
	/// This is an example of implementing the OwnerDrawnElement abstract class.
	/// It makes it very simple to create an element that you draw using CoreGraphics
	/// </summary>
	public class MovieElement : OwnerDrawnElement
	{
		CGGradient gradient;
		private UIFont _CaptionFont = UIFont.SystemFontOfSize(15.0f);

		public MovieElement(string caption) : base(UITableViewCellStyle.Default)
		{
			Caption = caption;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			gradient = new CGGradient(
			    colorSpace,
			    new float[] { 0.95f, 0.95f, 0.95f, 1, 
							  0.85f, 0.85f, 0.85f, 1},
				new float[] { 0, 1 } );
		}
		
		public override void Draw(RectangleF bounds, CGContext context, UIView view)
		{
			UIColor.White.SetFill();
			context.FillRect(bounds);
			
			context.DrawLinearGradient(gradient, new PointF(bounds.Left, bounds.Top), new PointF(bounds.Left, bounds.Bottom), CGGradientDrawingOptions.DrawsAfterEndLocation);
			
			UIColor.DarkGray.SetColor();
			view.DrawString(Caption, new RectangleF(10, 10, bounds.Width - 20, TextHeight(bounds)), _CaptionFont, UILineBreakMode.WordWrap);
		}
		
		public override float Height(RectangleF bounds)
		{
			var height = 40.0f + TextHeight(bounds);
			return height;
		}
		
		private float TextHeight(RectangleF bounds)
		{
			SizeF size;
			using(NSString str = new NSString(this.Caption))
			{
				size = str.StringSize(_CaptionFont, new SizeF(bounds.Width - 20, 1000), UILineBreakMode.WordWrap);
			}			
			return size.Height;
		}
		
		public override string ToString()
		{
			return string.Format(Caption);
		}
	}
}

