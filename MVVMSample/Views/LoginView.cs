using System;
using MonoMobile.MVVM;
using MonoTouch.UIKit;
using System.ComponentModel;
namespace DealerCenter
{
	[Theme(typeof(LoginTheme))]
	[Caption("DealerCenter Login")]
	public class LoginView : View
	{
	[Section(" ","Please type in your DealerCenter credentials", Order = 0)]
		[Entry]
		public string UserName;
		
		[Password]
		public string Password;
		
	[Section("","\n\n\n\n\n",Order = 1)]
		[Button]
		[Theme(typeof(BlueButtonTheme))]
		public void Login()
		{
			Console.WriteLine("Logged in");
			Application.NavigationController.DismissModalViewControllerAnimated(true);
		} 
		
//	[Section("", "  Tap here to try DealerCenter with sample data.", Order = 2)]
//		[Button]
//		[Theme(typeof(BlueButtonTheme))]
//		[Bind("ButtonColor", "BackgroundColor")]
//		public void Demo()
//		{
//		}

		public override void Initialize()
		{
			DataContext = new LoginViewModel() { UserName = "Rkozak" };
		}

	[Section("\n\n", Order = 3)]
		public AboutView About = new AboutView();
	}
}

