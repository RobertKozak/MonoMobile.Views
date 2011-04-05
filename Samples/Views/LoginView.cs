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
		[Section("Login")]
		[Entry("User Name", AutoCorrectionType = UITextAutocorrectionType.No)]
		[Caption("")]
		public string UserName { get; set;}
		[Password("Password")]
		[Caption("")]
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

