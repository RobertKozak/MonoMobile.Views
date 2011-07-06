using System;
using MonoTouch.UIKit;
using MonoMobile.MVVM;
namespace DealerCenter
{
	public class InventoryItemEditView : View
	{
		[NavbarButton(UIBarButtonSystemItem.Done)]
		public void Done()
		{
			Application.DismissModalView(true);
		}

		public string InventoryEdit = "Edit Screen";

		public InventoryItemEditView()
		{
		}
	}
}

