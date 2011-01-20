using System;
using MonoTouch.MVVM;
using MonoTouch.Dialog;

namespace Samples
{
	public class WebSamples : View
	{
		public Uri Google 
		{
			get { return Get (() => Google, new Uri ("http://www.google.com")); }
			set { Set (() => Google, value); }
		}

		[Html]
		public string Nowcom 
		{
			get { return Get (() => Nowcom, "http://www.nowcom.com"); }
			set { Set (() => Nowcom, value); }
		}

	}
}

