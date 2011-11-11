// 
// UIDatePickerToolbar.cs
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

	public class UIDatePickerToolbar : UIKeyboardToolbar
	{
		private UIBarButtonItem _TodayButton;
		private UIDatePicker _DatePicker { get; set; }

		public UIDatePickerToolbar(IFocusable focusableElement) : base(focusableElement)
		{
		}

		public UIDatePickerToolbar(NSCoder coder) : base(coder) { }
		public UIDatePickerToolbar(NSObjectFlag t) : base(t) { }
		public UIDatePickerToolbar(IntPtr handle) : base(handle) { }
		public UIDatePickerToolbar(RectangleF frame) : base(frame) { }

		public override UIBarButtonItem[] CreateToolbarItems()
		{
			var caption = "Today";

			_DatePicker = FocusableView.Control as UIDatePicker;
			if(_DatePicker != null)
			{
				if (_DatePicker.Mode == UIDatePickerMode.Time)
					caption = "Now";
			}

			var baseButtons = base.CreateToolbarItems();
			
			_TodayButton = new UIBarButtonItem(caption, UIBarButtonItemStyle.Bordered, Today);
			
			var buttonList = baseButtons.ToList();
			buttonList.Insert(2, _TodayButton);
			
			return buttonList.ToArray();
		}

		public void Today(object sender, EventArgs e)
		{
			if (_DatePicker != null)
				_DatePicker.Date = DateTime.Now;
		}
	}
}

