// 
// UIKeyboardToolbar.cs
// 
// Author:
//   Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
// 
// Copyright 2011, Nowcom Corporation.
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
	using System.Drawing;
	using System.Linq;
	using MonoTouch.Foundation;
	using System.Collections.Generic;
	using MonoTouch.UIKit;

	public class UIKeyboardToolbar : UIInputViewToolbar
	{
		private bool _NextButtonVisible;
		private bool _PreviousButtonVisible;
		private UIBarButtonItem _NextButton;
		private UIBarButtonItem _PrevButton;
		
		public bool NextButtonVisible 
		{ 
			get { return _NextButtonVisible; } 
			set 
			{ 
				if (_NextButtonVisible != value) 
				{ 
					_NextButtonVisible = value; 
					SetNeedsLayout(); 
				} 
			}
		} 

		public bool PreviousButtonVisible
		{
			get { return _PreviousButtonVisible; }
			set
			{
				if (_PreviousButtonVisible != value)
				{
					_PreviousButtonVisible = value;
					SetNeedsLayout();
				}
			}
		}

		public UIKeyboardToolbar(IFocusable focusableView) : base(focusableView)
		{		
			PreviousButtonVisible = true;
			NextButtonVisible = true;
		}
		
		public UIKeyboardToolbar(NSCoder coder) : base(coder) { }
		public UIKeyboardToolbar(IntPtr handle) : base(handle) { }
		public UIKeyboardToolbar(RectangleF frame) : base(frame) { }
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_NextButton != null)
				{
					_NextButton.Dispose();
					_NextButton = null;
				}

				if (_PrevButton != null)
				{
					_PrevButton.Dispose();
					_PrevButton = null;
				}
			}
			
			base.Dispose(disposing);
		}

		public override UIBarButtonItem[] CreateToolbarItems()
		{
			var baseButtons = base.CreateToolbarItems();

			if (PreviousButtonVisible)
				_PrevButton = new UIBarButtonItem("Previous", UIBarButtonItemStyle.Bordered, PreviousField);

			if (NextButtonVisible)
				_NextButton = new UIBarButtonItem("Next", UIBarButtonItemStyle.Bordered, NextField);
	
			UIBarButtonItem[] buttons = { _PrevButton, _NextButton };

			return buttons.Union(baseButtons).Where((b)=>b != null).ToArray();
		}

		public void PreviousField(object sender, EventArgs e)
		{
			FocusableView.MovePrev();
		}

		public void NextField(object sender, EventArgs e)
		{
			FocusableView.MoveNext();
		}
	}
}

