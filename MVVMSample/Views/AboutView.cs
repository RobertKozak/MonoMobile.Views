namespace DealerCenter
{
	using System;
	using MonoMobile.MVVM;

	public class AboutView : View
	{		
	[Section("Support on the web", "Please visit us on the web if you have any questions.\n\n\n\n\n\n\n\n\n")]

		[Html]
		[Caption("DealerCenter Support")]
		public string DealerCenterSupport = "http://www.dealercenter.net/en/Pages/support.aspx";
		[Html]
		public string NowcomSupport = "http://www.dealercenter.net/en/Pages/contact-us.aspx";
		
	[Section("", "")]

		public string Version = "1.0";
	}
}

