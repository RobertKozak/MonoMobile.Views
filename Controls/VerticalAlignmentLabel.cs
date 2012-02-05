// 
//  VerticalAlignmnentLabel.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011 - 2012, Nowcom Corporation.
// 
//  Code licensed under the MIT X11 license
// 
//  Permission is hereby granted, free of charge, to any person obtaining
//  a copy of this software and associated documentation files (the
//  "Software"), to deal in the Software without restriction, including
//  without limitation the rights to use, copy, modify, merge, publish,
//  distribute, sublicense, and/or sell copies of the Software, and to
//  permit persons to whom the Software is furnished to do so, subject to
//  the following conditions:
// 
//  The above copyright notice and this permission notice shall be
//  included in all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
namespace MonoMobile.Views
{
	using System.Drawing;
	using MonoTouch.UIKit;

	public enum UIVerticalAlignment
	{ 
		Middle = 0,    //the default (what standard UILabels do) 
		Top,
		Bottom
	} 

	public class VerticalAlignmnentLabel : UILabel
	{ 
		private UIVerticalAlignment _VerticalAlignment;

		public UIVerticalAlignment VerticalAlignment
		{ 
			get { return _VerticalAlignment; } 
			set
			{ 
				if (_VerticalAlignment != value)
				{ 
					_VerticalAlignment = value; 
					SetNeedsDisplay();   
				} 
			} 
		} 

		public VerticalAlignmnentLabel()
		{
		}

		public VerticalAlignmnentLabel(RectangleF rect) : base(rect)
		{
		} 

		public override void DrawText(RectangleF rect)
		{ 
			RectangleF bounds = TextRectForBounds(rect, Lines); 

			base.DrawText(bounds); 
		} 

		public override RectangleF TextRectForBounds(RectangleF bounds, int numberOfLines)
		{
			RectangleF calculatedRect = base.TextRectForBounds(bounds, numberOfLines);
 
			if (_VerticalAlignment != UIVerticalAlignment.Top)
			{   
				if (_VerticalAlignment == UIVerticalAlignment.Bottom)
				{ 
					bounds.Y += bounds.Height - calculatedRect.Height;    //move down by difference
				}
				else
				{    
					bounds.Y += (bounds.Height - calculatedRect.Height) / 2; 
				} 
			} 

			bounds.Height = calculatedRect.Height;    //always the calculated height 

			return (bounds); 
		} 
	}
}

