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
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public partial class DateTimeElement : Element, ISelectable
	{
		public DateTime Value { get; set; }  

		public UIDatePicker DatePicker;
		protected NSDateFormatter fmt = new NSDateFormatter { DateStyle = NSDateFormatterStyle.Short };

		public DateTimeElement(string caption) : base(caption)
		{
		}
		public DateTimeElement(string caption, DateTime date) : base(caption)
		{
			Value = date;
		}

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(UITableViewCellStyle.Value1, Id, this);
		}

		public override void InitializeCell(UITableView tableView)
		{
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

		public virtual UIDatePicker CreatePicker()
		{
			var picker = new UIDatePicker(RectangleF.Empty) { AutoresizingMask = UIViewAutoresizing.FlexibleWidth, Mode = UIDatePickerMode.Date, Date = (DateTime)Value };
			return picker;
		}

		private static RectangleF PickerFrameWithSize(SizeF size)
		{
			var screenRect = UIScreen.MainScreen.ApplicationFrame;
			float fY = 0, fX = 0;
			
			switch (UIApplication.SharedApplication.StatusBarOrientation)
			{
				case UIInterfaceOrientation.LandscapeLeft:
				case UIInterfaceOrientation.LandscapeRight:
					fX = (screenRect.Height - size.Width) / 2;
					fY = (screenRect.Width - size.Height) / 2 - 17;
					break;
				
				case UIInterfaceOrientation.Portrait:
				case UIInterfaceOrientation.PortraitUpsideDown:
					fX = (screenRect.Width - size.Width) / 2;
					fY = (screenRect.Height - size.Height) / 2 - 25;
					break;
			}
			
			return new RectangleF(fX, fY, size.Width, size.Height);
		}

		private class MyViewController : UIViewController
		{
			DateTimeElement container;

			public MyViewController(DateTimeElement container)
			{
				this.container = container;
			}

			public override void ViewWillDisappear(bool animated)
			{
				base.ViewWillDisappear(animated);
				container.Value = container.DatePicker.Date;
			}

			public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
			{
				base.DidRotate(fromInterfaceOrientation);
				container.DatePicker.Frame = PickerFrameWithSize(container.DatePicker.SizeThatFits(SizeF.Empty));
			}

			public bool Autorotate
			{
				get;
				set;
			}

			public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}

		public void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			var vc = new MyViewController(this) { Autorotate = dvc.Autorotate };
			DatePicker = CreatePicker();
			DatePicker.Frame = PickerFrameWithSize(DatePicker.SizeThatFits(SizeF.Empty));
			vc.View.BackgroundColor = UIColor.Black;
			vc.View.AddSubview(DatePicker);
			dvc.ActivateController(vc, dvc);
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

