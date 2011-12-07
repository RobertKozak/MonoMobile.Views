// 
//  BooleanCellView.cs
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
	using System.Drawing;
	using MonoMobile.Views;
	using MonoTouch.UIKit;
	using MonoTouch.Foundation;
	
	[Preserve(AllMembers = true)]
	public class BooleanCellView : CellView<bool>, ISelectable, IAccessoryView
	{
		private UISwitch Switch { get; set; }
		
		public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Default; } }
		public bool HasCheckmark;

		public BooleanCellView(RectangleF frame) : base(new RectangleF(0, 0, 85, frame.Height))
		{	
			Switch = new UISwitch(new RectangleF(0, 9, frame.Width, frame.Height)) 
			{
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.Clear, 
				Tag = 1 
			};

			Switch.ValueChanged += delegate 
			{
				DataContext.Value = Switch.On;
			};

			Add(Switch);
		}

		public override void UpdateCell(UITableViewCell cell, NSIndexPath indexPath)
		{
			var checkmarkAttribute = DataContext.Member.GetCustomAttribute<CheckmarkAttribute>();
			HasCheckmark = checkmarkAttribute != null;

			UpdateValue();

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.TextLabel.Text = Caption;
			Switch.On = (bool)DataContext.Value;
		}

		public void Selected(DialogViewController controller, UITableView tableView, object item, NSIndexPath indexPath)
		{
			DataContext.Value = !(bool)DataContext.Value;
			UpdateValue();

			controller.UpdateSource();
		}

		private void UpdateValue()
		{
			if (HasCheckmark)
			{
				Cell.AccessoryView = null;
				Cell.Accessory = (bool)DataContext.Value ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && Switch != null)
			{
				Switch.Dispose();
				Switch = null;
			}
			
			base.Dispose(disposing);
		}
	}
}