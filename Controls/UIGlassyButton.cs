//
// UIGlassyButton.cs: 
//
// Author:
//   Robert Kozak (rkozak@nowcom.com)
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
namespace MonoTouch.MVVM.Controls
{
	using System;
	using MonoTouch.UIKit;
	using MonoTouch.CoreAnimation;
	using System.Drawing;
	
	public class UIGlassyButton : UIButton
	{
		private bool _Initialized;

		public UIColor Color { get; set; }		
		public UIColor HighlightColor { get; set; }

		public string _Title = string.Empty;
		public new string Title 
		{ 
			get { return _Title; } 
			set 
			{ 
				_Title = value;

				SetNeedsDisplay();
			} 
		}
		
		public UIGlassyButton(RectangleF rect): base(rect)
		{
			Color = UIColor.FromRGB(88f, 170f, 34f);
			HighlightColor = UIColor.Black;
		}
		
		public void Init(RectangleF rect)
		{
			Layer.MasksToBounds = true;
			Layer.CornerRadius = 8;
			
			var gradientFrame = rect;
			
			var shineFrame = gradientFrame;
			shineFrame.Y += 1;
			shineFrame.X += 1;
			shineFrame.Width -= 2;
			shineFrame.Height = (shineFrame.Height / 2);

			var shineLayer = new CAGradientLayer();
			shineLayer.Frame = shineFrame;
			shineLayer.Colors = new MonoTouch.CoreGraphics.CGColor[] { UIColor.White.ColorWithAlpha (0.75f).CGColor, UIColor.White.ColorWithAlpha (0.10f).CGColor };
			shineLayer.CornerRadius = 8;
			
			var backgroundLayer = new CAGradientLayer();
			backgroundLayer.Frame = gradientFrame;
			backgroundLayer.Colors = new MonoTouch.CoreGraphics.CGColor[] { Color.ColorWithAlpha(0.99f).CGColor, Color.ColorWithAlpha(0.80f).CGColor };

			var highlightLayer = new CAGradientLayer();
			highlightLayer.Frame = gradientFrame;
			
			Layer.AddSublayer(backgroundLayer);
			Layer.AddSublayer(highlightLayer);
			Layer.AddSublayer(shineLayer);
		
			VerticalAlignment = UIControlContentVerticalAlignment.Center;
			Font = UIFont.BoldSystemFontOfSize (17);
			SetTitle (Title, UIControlState.Normal);
			SetTitleColor (UIColor.White, UIControlState.Normal);
			
			_Initialized = true;
		}

		public override void Draw(RectangleF rect)
		{
			base.Draw(rect);

			if(!_Initialized)
				Init(rect);

			var highlightLayer = Layer.Sublayers[1] as CAGradientLayer;
			
			if (Highlighted)
			{
				if (HighlightColor == UIColor.Blue) 
				{
					highlightLayer.Colors = new MonoTouch.CoreGraphics.CGColor[] { HighlightColor.ColorWithAlpha(0.60f).CGColor, HighlightColor.ColorWithAlpha(0.95f).CGColor };
				} 
				else 
				{
					highlightLayer.Colors = new MonoTouch.CoreGraphics.CGColor[] { HighlightColor.ColorWithAlpha(0.10f).CGColor, HighlightColor.ColorWithAlpha(0.40f).CGColor };
				}
				
			}
			
			highlightLayer.Hidden = !Highlighted;
		}
		
		public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
		{
			if (uievent.Type == UIEventType.Touches)
			{
				SetNeedsDisplay();
			}

			return base.BeginTracking(uitouch, uievent); 
		}
		
		public override void EndTracking(UITouch uitouch, UIEvent uievent)
		{
			if (uievent.Type == UIEventType.Touches)
			{
				SetNeedsDisplay();
			}

			base.EndTracking(uitouch, uievent);
		}
	}
}

//		private void CreateRoundedTopCorners (MonoTouch.CoreGraphics.CGContext context, float radius, RectangleF rect)
//		{
//			// Drawing with a white stroke color
//			context.SetRGBStrokeColor(1.0f, 1.0f, 1.0f, 0.75f);
//			
//			context.SetRGBFillColor(1.0f, 0f, 0f, 0.75f);
//			// Draw them with a 2.0 stroke width so they are a bit more visible.
//			context.SetLineWidth (2.0f);
//			
//			// In order to create the 4 arcs correctly, we need to know the min, mid and max positions
//			// on the x and y lengths of the given rectangle.
//			var minx = rect.GetMinX ();
//			var midx = rect.GetMidX ();
//			var maxx = rect.GetMaxX ();
//			
//			var miny = rect.GetMinY ();
//			var midy = rect.GetMidY ();
//			var maxy = rect.GetMaxY ();
//			
//			// Start at 1
//			
//			context.MoveTo (minx, midy);
//			// Add an arc through 2 to 3
//			context.AddArcToPoint (minx, miny, midx, miny, radius);
//			context.AddArcToPoint (maxx, miny, maxx, midy, radius);
//			context.AddLineToPoint (maxx, maxy);
//			context.AddLineToPoint (minx, maxy);
//			context.ClosePath ();
//			// Fill & stroke the path
//			context.DrawPath (CGPathDrawingMode.FillStroke);
//			
//		}
