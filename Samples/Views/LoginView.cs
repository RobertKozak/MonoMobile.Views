using System;
using MonoMobile.MVVM;
using MonoTouch.Dialog;
using System.ComponentModel;
using MonoTouch.UIKit;
namespace Samples
{
	//[Theme(typeof(FrostedTheme))]
	//[Theme(typeof(NavbarTheme))]
//	[BackgroundImage("Images/bluesky.jpg")]
	public class LoginView : View
	{
		[Section("Login to DealerCenter")]
		[Entry(AutoCorrectionType = UITextAutocorrectionType.No, EditMode = EditMode.NoCaption)]
		public string UserName { get; set;}

		[Password(EditMode = EditMode.NoCaption)]
		public string Password { get; set;}
		
		[Section("", "", Order=2)]
		[Button]
		public void Login()
		{

		}
		
		[Section(Order = 3)]
		[Caption("About")]
		public AboutView AboutView
		{
			get;
			set;
		}

		public LoginView()
		{
			AboutView = new AboutView();
		}
	}
	
	public class AboutView: View
	{
		public string Version = "1.0";
 
		[Section("Find us on the web")]
		[Html]
		public string DealerCenter = "http://www.dealercenter.net/en/Pages/contact-us.aspx";

		[Html]
		public string Nowcom = "http://www.nowcom.com/support.aspx";

		[Section]
		[Caption("")]
		[Theme(typeof(CopyrightTheme))]
		public string Copyright = "Copyright © 2011 Nowcom Corporation"; 
	}

	public class CopyrightTheme: Theme
	{
		public CopyrightTheme()
		{
			Name = "CopyrightTheme";
			DetailTextFont = UIFont.BoldSystemFontOfSize(13);
			DetailTextColor = UIColor.DarkTextColor;
			DetailTextShadowColor = UIColor.FromWhiteAlpha(1f, 0.8f);
			DetailTextShadowOffset = new System.Drawing.SizeF(0, 1);
			DetailTextAlignment = UITextAlignment.Center;

			CellBackgroundColor = UIColor.Red;
		}
	}
}

