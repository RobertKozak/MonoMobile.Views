//// 
////  AddressAttribute.cs - example of a CellViewTemplate
//// 
////  Author:
////    Robert Kozak (rkozak@gmail.com / Twitter:@robertkozak)
//// 
////  Copyright 2011, Nowcom Corporation.
//// 
////  Code licensed under the MIT X11 license
//// 
////  Permission is hereby granted, free of charge, to any person obtaining
////  a copy of this software and associated documentation files (the
////  "Software"), to deal in the Software without restriction, including
////  without limitation the rights to use, copy, modify, merge, publish,
////  distribute, sublicense, and/or sell copies of the Software, and to
////  permit persons to whom the Software is furnished to do so, subject to
////  the following conditions:
//// 
////  The above copyright notice and this permission notice shall be
////  included in all copies or substantial portions of the Software.
//// 
////  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
////  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
////  MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
////  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
////  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
////  OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
////  WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//// 
//namespace MonoMobile.Views
//{
//	using System;
//	using System.Drawing;
//	using MonoTouch.UIKit;
//	using MonoTouch.Foundation;
//
//	[Preserve(AllMembers = true)]
//	public class Address
//	{
//		public string Street { get; set; }
//
//		public string City { get; set; }
//
//		public string State { get; set; }
//
//		public string ZipCode { get; set; }
//		
//		public Address(string street, string city, string state, string zipCode)
//		{
//			Street = street;
//			City = city;
//			State = state;
//			ZipCode = zipCode;
//		}
//	}
//
//	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
//	public class AddressAttribute: CellViewTemplate
//	{
//		public override Type CellViewType { get { return typeof(AddressCellView); } }
//
//		public AddressAttribute(): base(typeof(AddressValueConverter))
//		{
//
//		}
//
//		class AddressCellView: CellView<Address>, ISelectable
//		{
//			public override UITableViewCellStyle CellStyle { get { return UITableViewCellStyle.Value1; } }
//			public AddressCellView(RectangleF frame): base(frame)
//			{
//	
//			}
//	
//			public void Selected(DialogViewController controller, UITableView tableView, object item, MonoTouch.Foundation.NSIndexPath indexPath)
//			{
//				var view = Value;
//	
//				var dc = view as IDataContext<object>;
//				if (dc != null)
//				{
//					dc.DataContext = Value;
//				}
//	
//				var dvc = new DialogViewController(Caption, view, controller.Theme, true) { Autorotate = true };
//				var nav = controller.ParentViewController as UINavigationController;
//	
//				nav.PushViewController(dvc, true);	
//			}	
//			
//			public override void UpdateCell(UITableViewCell cell, MonoTouch.Foundation.NSIndexPath indexPath)
//			{
//				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
//				cell.TextLabel.Text = Value.City;
//				cell.DetailTextLabel.Text = Value.State;
//			}
//		}
//
//		class AddressValueConverter : IValueConverter
//		{
//			public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//			{
//				var address = ((string)value).Split(',');
//				var street = address[0];
//				var city = address[1];
//				var state = address[2];
//				var zip = address[3];
//				return new Address(street, city, state, zip);
//			}
//	
//			public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//			{
//				return "";
//			}
//		}
//	}
//}
//
