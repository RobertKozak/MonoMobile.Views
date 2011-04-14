using MonoMobile.MVVM;
//
// GlassButtonStyle.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com) Twitter:@robertkozak
//
// Copyright 2011, Nowcom Corporation
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
namespace MonoMobile.MVVM
{
	using MonoTouch.UIKit;
	using System.Drawing;
	using MonoTouch.CoreGraphics;

	public class GlassButtonTheme: Theme
	{
		public UIColor HighlightColor { get; set; }
		
		public GlassButtonTheme()
		{
			//CellBackgroundColor = UIColor.FromRGB(88, 170, 34);
			CellBackgroundColor = UIColor.FromWhiteAlpha(1f, 0.6f);
			HighlightColor = UIColor.FromRGB(5, 115, 245);
			DrawWhenHighlighted = true;

			TextColor = UIColor.White;
			TextShadowColor = UIColor.DarkGray;
			TextAlignment = UITextAlignment.Center;

			DrawContentViewAction = (rect, context, cell) => { DrawContentView(rect, context, cell); };
		}

		public void DrawContentView(RectangleF rect, CGContext context, UITableViewElementCell cell)
		{
			var gradientFrame = rect;
			
			var shineFrame = gradientFrame;
			shineFrame.Y += 1;
			shineFrame.X += 1;
			shineFrame.Width -= 2;
			shineFrame.Height = (shineFrame.Height / 2);

			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();

			var gradient = new CGGradient(
			    colorSpace,
			    new float[] { 0f, 0f,0f, 0.70f, 
							  0f, 0f, 0f, 0.40f},
				new float[] { 0, 1 } );

			var shineGradient = new CGGradient(
			    colorSpace,
			    new float[] { 1f, 1f, 1f, 0.80f, 
							  1f, 1f, 1f, 0.10f},
				new float[] { 0, 1 } );
			
			if (Cell != null && Cell.Highlighted)
			{
				context.SetFillColorWithColor(HighlightColor.CGColor);
			}
			else
			{
				context.SetFillColorWithColor(CellBackgroundColor.CGColor);
			}
			context.FillRect(rect);
	
			context.DrawLinearGradient(gradient, new PointF(gradientFrame.Left, gradientFrame.Top), new PointF(gradientFrame.Left, gradientFrame.Bottom), CGGradientDrawingOptions.DrawsAfterEndLocation);		
			context.DrawLinearGradient(shineGradient, new PointF(shineFrame.Left, shineFrame.Top), new PointF(shineFrame.Left, shineFrame.Bottom), CGGradientDrawingOptions.DrawsAfterEndLocation);
		}
	}
}
