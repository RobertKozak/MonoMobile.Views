using System;
using MonoMobile.Views;
using MonoTouch.UIKit;

namespace Samples
{
	public class AddressView: View
	{
		[Entry]
		public string Number { get; set; }

		[Entry]
		public string Street  { get; set; }

		[Entry]
		public string City  { get; set; }

		[Entry]
		public string State  { get; set; }

		[Entry]
		public string Zip  { get; set; }

		public override string ToString()
		{
			return string.Format ("{0} {1}, {2}, {3}, {4}", Number, Street, City, State, Zip);
		}
	}
}

