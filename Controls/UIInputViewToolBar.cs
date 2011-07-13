// 
// UIInputViewToolbar.cs
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
namespace MonoMobile.MVVM
{
	using System;
	using System.Drawing;
	using System.Linq;
	using MonoTouch.Foundation;
	using System.Collections.Generic;
	using MonoTouch.UIKit;

	public class UIInputViewToolbar : UIToolbar
	{
		private UIBarButtonItem _Spacer;
		private UIBarButtonItem _DoneButton;

		protected IFocusable FocusableElement;

		public UIInputViewToolbar(IFocusable focusableElement) : base(new RectangleF(0, 0, 320, 44))
		{
			FocusableElement = focusableElement;
						
			var themeable = FocusableElement as IThemeable;
			if (themeable != null)
				TintColor = themeable.Theme.BarTintColor;
				
			Translucent = true;
		}

		public UIInputViewToolbar(NSCoder coder) : base(coder) { }
		public UIInputViewToolbar(NSObjectFlag t) : base(t) { }
		public UIInputViewToolbar(IntPtr handle) : base(handle) { }
		public UIInputViewToolbar(RectangleF frame) : base(frame) { }

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			Items = CreateToolbarItems();
		}

		public virtual UIBarButtonItem[] CreateToolbarItems()
		{
			if (_Spacer == null)
				_Spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			if (_DoneButton == null)
				_DoneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, DismissKeyboard);

			UIBarButtonItem[] buttons = { _Spacer, _DoneButton };
			
			return buttons.Where(b => b != null).ToArray();
		}

		public void DismissKeyboard(object sender, EventArgs e)
		{
			FocusableElement.ElementView.ResignFirstResponder();
		}
	}
}

