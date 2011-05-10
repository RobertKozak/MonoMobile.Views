//
// DateTimeElement.cs
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Copyright 2010, Novell, Inc.
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
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	public partial class DateTimeElement : FocusableElement, ISelectable
	{
		public NSDate Value { get; set; }  

		public UIDatePicker DatePicker;
		protected NSDateFormatter fmt = new NSDateFormatter { DateStyle = NSDateFormatterStyle.Short };

		public DateTimeElement(string caption) : base(caption)
		{
		}
		public DateTimeElement(string caption, DateTime date) : base(caption)
		{
			Value = date;
		}

		public override void InitializeCell(UITableView tableView)
		{
			RemoveTag(2);

			base.InitializeCell(tableView);

			Cell.Accessory = UITableViewCellAccessory.None;
			Cell.TextLabel.Text = Caption;
			
			DetailTextLabel.Text = FormatDate(Value);
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

		public virtual string FormatDate(DateTime dt)
		{
			return fmt.ToString(dt) + " " + dt.ToLocalTime().ToShortTimeString();
		}
		
		public override void InitializeContent()
		{ 
			base.InitializeContent();

			DatePicker = CreatePicker();
			var view = new UIView(DatePicker.Bounds);
			view.AddSubview(DatePicker);
		
			Control = DatePicker;

			InputControl.InputView = view;
			InputControl.InputAccessoryView = new UIDatePickerToolbar(this) { };

			InputControl.Started += (s, e) =>
			{
				ValueProperty.ConvertBack<NSDate>();				
			};

			InputControl.Ended += (s, e) => 
			{
				Value = DatePicker.Date;
				OnValueChanged();
			};
		}

		public virtual UIDatePicker CreatePicker()
		{
			var picker = new UIDatePicker(RectangleF.Empty) { AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight, Mode = UIDatePickerMode.Date, Date = (DateTime)Value };
			return picker;
		}

		protected virtual void OnValueChanged()
		{
			if (DatePicker != null)
				DatePicker.Date = Value;
			
			if (DetailTextLabel != null)
				DetailTextLabel.Text = FormatDate(Value);
		}
	}
}

