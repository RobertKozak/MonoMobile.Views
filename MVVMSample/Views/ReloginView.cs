using System;
using MonoMobile.MVVM;
using MonoTouch.UIKit;
namespace DealerCenter
{
	[Theme(typeof(LoginTheme))]
	[Caption("DealerCenter Login")]
	public class ReloginView : View
	{
	[Section("", "Please type in your DealerCenter credentials", Order = 0)]
		[Entry]
		public string UserName;

		[Password]
		public string Password;

	[Section(Order = 1)]
		[Button]
		[Theme(typeof(BlueButtonTheme))]
		public void Login()
		{
			Console.WriteLine("Logged in");
			Application.NavigationController.DismissModalViewControllerAnimated(true);
		}
	}
}

