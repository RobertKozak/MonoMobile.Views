using System;
using MonoMobile.MVVM;

namespace Samples
{
	public class View1: View
	{
		public string Person 
		{ 
			get;// { return Get(()=>Person, "Robert"); } 
			set;// { Set(()=>Person, value); }
		} 
		public int Age 
		{
			get;// { return Get(()=>Age, 43); } 
			set;// { Set(()=>Age, value); }
		}
		
		[Button]
		public void ChangeAge()
		{
			Age = 44;
			Person = "Robert";
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
			AddressView = new AddressView() {Number = "4751", Street ="Wilshire Blvd", City ="LA", State="CA", Zip ="90010" };
		}
	}
}

