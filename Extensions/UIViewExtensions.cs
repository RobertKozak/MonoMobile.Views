// 
//  {filename}.cs
// 
//  Author:
//    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
//  Copyright 2011, Nowcom Corporation.
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
using System;
using MonoTouch.UIKit;
namespace MonoMobile.MVVM
{
	public static class UIViewExtensions
	{
		public static void FlipView(this UIView fromView, UIView toView)
		{			
			UIView.BeginAnimations("Flipper");
			UIView.SetAnimationDuration(1.25);
			UIView.SetAnimationCurve(UIViewAnimationCurve.EaseInOut);
			if (fromView.Superview == null)
			{
				UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromRight, fromView, true);
				toView.ViewWillAppear(true);
				fromView.ViewWillDisappear(true);
				
				toView.View.RemoveFromSuperview();
				this.View.AddSubview(fromView.View);
				
				fromView.ViewDidDisappear(true);
				toView.ViewDidAppear(true);
			} 
			else
			{
				UIView.SetAnimationTransition(UIViewAnimationTransition.FlipFromLeft, fromView, true);
				fromView.ViewWillAppear(true);
				toView.ViewWillDisappear(true);
				
				fromView.View.RemoveFromSuperview();
				this.View.AddSubview(toView.View);
				
				toView.ViewDidDisappear(true);
				fromView.ViewDidAppear(true);
			}
			
			UIView.CommitAnimations();
		}

	}
}

