using System;
using MonoMobile.MVVM;
using MonoMobile.MVVM;

namespace Samples
{
	public class WebSamples : View
	{
		public Uri Google 
		{
//			get { return Get (() => Google, new Uri ("http://www.google.com")); }
//			set { Set (() => Google, value); }
			get; set;
		}

		[Html]
		public string Nowcom 
		{
//			get { return Get (() => Nowcom, "http://www.nowcom.com"); }
//			set { Set (() => Nowcom, value); }
			get; set;
		}

		public WebSamples()
		{
			Google = new Uri("Http://www.google.com");
			Nowcom = "Http://www.nowcom.com";
		}

	}
}

