// 
//  DateTimeView.cs
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
namespace MonoMobile.Views
{
	using System;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	using System.Drawing;

	public class DateTimeView: FocusableCellView, IHandleNotifyPropertyChanged
	{
		protected NSDateFormatter fmt = new NSDateFormatter { DateStyle = NSDateFormatterStyle.Short };

		public UIDatePicker DatePicker;
		
		public DateTimeView(RectangleF frame) : base(frame)
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				fmt.Dispose();
				fmt = null;
				if (DatePicker != null)
				{
					DatePicker.Dispose();
					DatePicker = null;
				}
			}
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			base.UpdateCell(cell, indexPath);

			cell.Accessory = UITableViewCellAccessory.None;
			cell.TextLabel.Text = Caption;

			DatePicker = CreatePicker();
			var view = new UIView(DatePicker.Bounds) { BackgroundColor = UIColor.Black };
			view.AddSubview(DatePicker);
		
			Control = DatePicker;

			InputView.InputView = view;
			InputView.InputAccessoryView = new UIDatePickerToolbar(this) { };

			if (cell.DetailTextLabel != null)
				cell.DetailTextLabel.Text = FormatDate(DatePicker.Date.ToDateTime());
		}
		
		public override void HandleNotifyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			DatePicker.Date = (DateTime)DataContext.Value;
			base.HandleNotifyPropertyChanged(sender, e);
		}

		public virtual string FormatDate(DateTime dt)
		{
			return fmt.ToString(dt) + " " + dt.ToLocalTime().ToShortTimeString();
		}

		public virtual UIDatePicker CreatePicker()
		{
			var bounds = UIScreen.MainScreen.Bounds;

			var picker = new UIDatePicker(new RectangleF(0, 0, bounds.Width, UIDevice.CurrentDevice.GetKeyboardHeight())) 
			{ 
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight, 
				Mode = UIDatePickerMode.Date 
			};
			
			picker.ValueChanged += delegate { DataContext.Value = DatePicker.Date; };

			return picker;
		}
	}
}

