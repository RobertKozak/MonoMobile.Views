using System;
using MonoTouch.MVVM;
using MonoTouch.Dialog;
namespace Samples
{
	public class View1: View
	{
		public string Person 
		{ 
			get { return Get(()=>Person, "Robert"); } 
			set { Set(()=>Person, value); }
		} 
		public int Age 
		{
			get { return Get(()=>Age, 43); } 
			set { Set(()=>Age, value); }
		}
	}

	public class View2 : View
	{
		public string Company { get; set; }
		[Inline]
		public AddressView AddressView { get; set; }
		
		public View2()
		{
			Company = "Nowcom Corporation";
			AddressView = new AddressView() {Number = "4751", Street ="S Wilshire Blvd", City ="LA", State="CA", Zip ="90010" };
		}
	}
}

