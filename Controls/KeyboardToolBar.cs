// 
//  UIKeyboardToolbar.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using System.Collections.Generic;
	using MonoTouch.UIKit;

	public class UIKeyboardToolbar : UIToolbar
	{
		private UIBarButtonItem _NextButton;
		private UIBarButtonItem _PrevButton;
		private UIBarButtonItem _Spacer;
		private UIBarButtonItem _DoneButton;
		
		private IFocusable _FocusableElement;

		public UIKeyboardToolbar(IFocusable focusableElement) : base(new RectangleF(0, 0, 320, 44))
		{		
			_FocusableElement = focusableElement;

			_PrevButton = new UIBarButtonItem("Previous", UIBarButtonItemStyle.Bordered, PreviousField);
			_NextButton = new UIBarButtonItem("Next", UIBarButtonItemStyle.Bordered, NextField);
			_Spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			_DoneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, DismissKeyboard);
			
			UIBarButtonItem[] buttons = { _PrevButton, _NextButton, _Spacer, _DoneButton };
			
			Items = buttons;
			
			if (_FocusableElement != null && _FocusableElement.Entry != null && _FocusableElement.Entry.InputAccessoryView == null)
			{
				TintColor = ((IElement)_FocusableElement).Theme.BarTintColor;
				Translucent = true;
			}
		}

		public UIKeyboardToolbar(NSCoder coder) : base(coder)
		{
		}

		public UIKeyboardToolbar(NSObjectFlag t) : base(t)
		{
		}

		public UIKeyboardToolbar(IntPtr handle) : base(handle)
		{
		}

		public UIKeyboardToolbar(RectangleF frame) : base(frame)
		{
		}

		public void PreviousField(object sender, EventArgs e)
		{
			_FocusableElement.MovePrev();
		}

		public void NextField(object sender, EventArgs e)
		{
			_FocusableElement.MoveNext();
		}

		public void DismissKeyboard(object sender, EventArgs e)
		{
			_FocusableElement.Entry.ResignFirstResponder();
		}
	}
}

