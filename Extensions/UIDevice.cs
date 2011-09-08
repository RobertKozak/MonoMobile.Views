//
// UIDeviceExtensions.cs:
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
	using System.Collections.Generic;
	using MonoTouch.UIKit;
	
	public static class UIDeviceExtensions
	{
		public static bool IsPhone(this UIDevice device)
		{ 
			return device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; 
		} 
		
		public static bool IsPad(this UIDevice device)
		{ 
			return device.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad; 
		}
	
		public static float GetIndentation(this UIDevice device)
		{
			if (device.IsPhone())
				return 10;
			else
				return 40;
		}

		public static float GetDeviceMargin(this UIDevice device)
		{
			if (device.IsPhone())
				return 3;
			else
				return 7;
		}
		
		public static float GetFixedGap(this UIDevice device)
		{
			return 5;
		}
		
		public static float GetKeyboardHeight(this UIDevice device)
		{
			var orientation = device.Orientation;

			var landscape = orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;

			if (device.IsPad())
			{
				if (landscape)
					return 352;
				
				return 264;
			}
			
			if (landscape)
				return 140;
			
			return 216;
		}

		public static float GetActualWidth(this UIDevice device)
		{
			var orientation = device.Orientation;
			
			var landscape = orientation == UIDeviceOrientation.LandscapeLeft || orientation == UIDeviceOrientation.LandscapeRight;

			if (landscape)
				return UIScreen.MainScreen.Bounds.Height;
			else
				return UIScreen.MainScreen.Bounds.Width;
		}
	}
}

