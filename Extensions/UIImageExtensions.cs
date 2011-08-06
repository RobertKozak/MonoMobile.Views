using MonoTouch.CoreGraphics;
using MonoTouch.CoreAnimation;
//
// UIImageExtensions.cs:
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
namespace MonoMobile.Views
{
	using System;
	using MonoTouch.UIKit;
	using System.Drawing;

	public static class UIImageExtensions
	{
		public static UIImage FromFile(this UIImage image, string filename, SizeF fitSize)
		{
			var imageFile = UIImage.FromFile(filename);
			
			return imageFile.ImageToFitSize(fitSize);
		}

		public static UIImage RemoveSharpEdges(this UIImage image, int radius)
		{
			var width = image.Size.Width;
			UIGraphics.BeginImageContextWithOptions(new SizeF(width, width), false, 0f);
			var context = UIGraphics.GetCurrentContext();
			
			context.BeginPath();
			context.MoveTo(width, width/2);
			context.AddArcToPoint(width, width, width / 2, width, radius);
			context.AddArcToPoint(0, width, 0, width / 2, radius);
			context.AddArcToPoint(0, 0, width / 2, 0, radius);
			context.AddArcToPoint(width, 0, width, width / 2, radius);
			context.ClosePath();
			context.Clip();

			image.Draw(new PointF(0, 0));
			var converted = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return converted;
		}

		public static UIImage ImageToFitSize(this UIImage image, SizeF fitSize)
		{
			double imageScaleFactor = 1.0;
			imageScaleFactor = image.CurrentScale;
			
			double sourceWidth = image.Size.Width * imageScaleFactor;
			double sourceHeight = image.Size.Height * imageScaleFactor;
			double targetWidth = fitSize.Width;
			double targetHeight = fitSize.Height;
			
			double sourceRatio = sourceWidth / sourceHeight;
			double targetRatio = targetWidth / targetHeight;
			
			bool scaleWidth = (sourceRatio <= targetRatio);
			scaleWidth = !scaleWidth;
			
			double scalingFactor, scaledWidth, scaledHeight;
			
			if (scaleWidth)
			{
				scalingFactor = 1.0 / sourceRatio;
				scaledWidth = targetWidth;
				scaledHeight = Math.Round(targetWidth * scalingFactor);
			} 
			else
			{
				scalingFactor = sourceRatio;
				scaledWidth = Math.Round(targetHeight * scalingFactor);
				scaledHeight = targetHeight;
			}
			
			RectangleF destRect = new RectangleF(0, 0, (float)scaledWidth, (float)scaledHeight);
			
			UIGraphics.BeginImageContextWithOptions(destRect.Size, false, 0.0f);
			image.Draw(destRect);
			UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			
			return newImage;
		}
	}
}

