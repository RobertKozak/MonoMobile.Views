using MonoMobile.MVVM;
namespace DealerCenter
{
	using System;
	using MonoTouch.UIKit;

	public class InventoryListSelector : UISegmentedControl, ITappable
	{
		private UIImage favorites = UIImage.FromFile("Images/favorites.png");
		
		public ICommand Command { get; set; }
		public object CommandParameter { get; set; }

		public InventoryListSelector()
		{
			Frame = new System.Drawing.RectangleF(20, 0, 215, 30);
			InsertSegment("DealerCenter", 0, true);
			InsertSegment(favorites, 1, true);
			InsertSegment("Phone", 2, true);

			SelectedSegment = 0;
			
			ControlStyle = UISegmentedControlStyle.Bar;
			
			SetWidth(90, 0);
			SetWidth(35, 1);
			SetWidth(90, 2);

			ValueChanged += delegate 
			{
				Command.Execute(SelectedSegment);
			};
		}
	}
}

