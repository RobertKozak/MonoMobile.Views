using System;
using MonoMobile.MVVM;
using MonoTouch.UIKit;
namespace DealerCenter
{
	public class PhotosView : View
	{
		[NavbarButton(UIBarButtonSystemItem.Done)]
		public void Done()
		{
			Application.DismissModalView(true);
		}

		public string Photos = "Photos will go here";

		public PhotosView()
		{
		}
	}
}

