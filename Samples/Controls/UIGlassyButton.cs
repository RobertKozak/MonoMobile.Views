//
// UIGlassyButton.cs: 
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
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
namespace Samples.Controls
{
	using System;
	using System.Drawing;
	using MonoTouch.CoreAnimation;
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public class UIGlassyButton : UIButton
	{
		private bool _Initialized;
		private NSTimer _Timer;

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
			Font = UIFont.BoldSystemFontOfSize(17);
			SetTitle(Title, UIControlState.Normal);
			SetTitleColor(UIColor.White, UIControlState.Normal);
			
			_Initialized = true;
		}

		public override void Draw(RectangleF rect)
		{
			base.Draw(rect);

			if (!_Initialized)
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

			base.BeginTracking(uitouch, uievent);
			return true;
		}

		public override void EndTracking(UITouch uitouch, UIEvent uievent)
		{
			if (uievent.Type == UIEventType.Touches)
			{
				_Timer = NSTimer.CreateScheduledTimer(TimeSpan.FromMilliseconds(400), ButtonTimer);
				SetNeedsDisplay();
			}

			base.EndTracking(uitouch, uievent);
		}
		
		private void ButtonTimer()
		{
			_Timer.Invalidate();
			_Timer = null;
			Highlighted = false;
			SetNeedsDisplay();
		}
	}
}
