////
//// DateTimeElement.cs
////
//// Author:
////  Miguel de Icaza (miguel@gnome.org)
////
//// Copyright 2010, Novell, Inc.
////
//// Code licensed under the MIT X11 license
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
////
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
////
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Collections.Generic;
//	using System.Drawing;
//	using System.Linq;
//	using MonoMobile.Views;
//	using MonoTouch.Foundation;
//	using MonoTouch.UIKit;
//
//	[Preserve(AllMembers = true)]
//	public class DateTimeElement : FocusableElement, ISelectable
//	{
//		protected NSDateFormatter fmt = new NSDateFormatter { DateStyle = NSDateFormatterStyle.Short };
//	
//		public UIDatePicker DatePicker;
//		
//		public DateTimeElement(string caption) : base(caption)
//		{
//			DataBinding = new DateTimeElementDataBinding(this);
//		}
//
//		public DateTimeElement(string caption, DateTime date) : this(caption)
//		{
//			DataContext = date;
//		}
//		
//		public override UITableViewElementCell NewCell(NSString cellId, NSIndexPath indexPath)
//		{
//			return new UITableViewElementCell(UITableViewCellStyle.Value1, cellId, this);
//		}
//
//		public override void InitializeCell(UITableView tableView)
//		{
//			base.InitializeCell(tableView);
//
//			Cell.Accessory = UITableViewCellAccessory.None;
//			Cell.TextLabel.Text = Caption;
//		}
//
//		protected override void Dispose(bool disposing)
//		{
//			base.Dispose(disposing);
//			if (disposing)
//			{
//				fmt.Dispose();
//				fmt = null;
//				if (DatePicker != null)
//				{
//					DatePicker.Dispose();
//					DatePicker = null;
//				}
//			}
//		}
//
//		public virtual string FormatDate(DateTime dt)
//		{
//			return fmt.ToString(dt) + " " + dt.ToLocalTime().ToShortTimeString();
//		}
//		
//		public override void InitializeContent()
//		{ 
//			base.InitializeContent();
//
//			DatePicker = CreatePicker();
//			var view = new UIView(DatePicker.Bounds) { BackgroundColor = UIColor.Black };
//			view.AddSubview(DatePicker);
//		
//			Control = DatePicker;
//
//			((UIPlaceholderTextField)ElementView).InputView = view;
//			((UIPlaceholderTextField)ElementView).InputAccessoryView = new UIDatePickerToolbar(this) { };
//		}
//
//		public virtual UIDatePicker CreatePicker()
//		{
//			var bounds = UIScreen.MainScreen.Bounds;
//
//			var picker = new UIDatePicker(new RectangleF(0, 0, bounds.Width, UIDevice.CurrentDevice.GetKeyboardHeight())) 
//			{ 
//				AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight, 
//				Mode = UIDatePickerMode.Date 
//			};
//			
//			picker.ValueChanged += delegate { DataContext = DatePicker.Date; };
//
//			return picker;
//		}
//		
//		public override void UpdateCell()
//		{
//			base.UpdateCell();
//
//			OnDataContextChanged();
//		}
//
//		protected override void OnDataContextChanged()
//		{			
//			if (DetailTextLabel != null)
//				DetailTextLabel.Text = FormatDate(DatePicker.Date.ToDateTime());
//		}
//	}
//}
//
