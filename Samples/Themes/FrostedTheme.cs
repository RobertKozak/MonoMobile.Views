//
// FrostedTheme.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak
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
	using System;
	using MonoTouch.UIKit;
	using System.Drawing; 
	using MonoTouch.CoreGraphics;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class FrostedTheme: Theme
	{		
		public FrostedTheme()
		{
			Name = "FrostedTheme";

			CellBackgroundColor = UIColor.Clear;

			TextColor = UIColor.DarkTextColor;
			TextShadowColor = UIColor.FromWhiteAlpha(0.8f, 1.0f);
			TextShadowOffset = new SizeF(0,1);

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

			DrawElementViewAction = (rect, context, cell) => DrawContentView(rect, context, cell);
		}

		public void DrawContentView(RectangleF rect, CGContext context, UITableViewElementCell cell)
		{
			context.SaveState();
					
			var backgroundColor = CellBackgroundColor;
			if(backgroundColor != null)
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

