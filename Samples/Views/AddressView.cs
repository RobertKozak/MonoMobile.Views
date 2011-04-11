using System;
using MonoMobile.MVVM;
using MonoMobile.MVVM;
using MonoTouch.UIKit;

namespace Samples
{
	public class AddressView: View
	{
		[Entry]
		public string Number 
		{
			get;// { return Get(() => Number); }
			set;// { Set (() => Number, value); }
		}

		[Entry]
		public string Street 
		{
			get;// { return Get (() => Street); }
			set;// { Set (() => Street, value); }
		}

		[Entry]
		public string City 
		{
			get;// { return Get (() => City); }
			set;// { Set (() => City, value); }
		}

		[Entry]
		public string State 
		{
			get;// { return Get (() => State); }
			set;// { Set (() => State, value); }
		}

		[Entry]
		public string Zip 
		{
			get;// { return Get (() => Zip); }
			set;// { Set (() => Zip, value); }
		}

		public override string ToString()
		{
			return string.Format ("{0} {1}, {2}, {3}, {4}", Number, Street, City, State, Zip);
		}
	}
}

