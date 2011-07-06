namespace DealerCenter
{
	using System.Drawing;
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;	

	public class InventoryElement : RootElement
	{		
		private InventoryItemViewModel InventoryItem { get { return (InventoryItemViewModel)DataContext; } }

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(UITableViewCellStyle.Subtitle, Id, this);
		}

		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell(tableView);
			
			DetailTextLabel.TextAlignment = UITextAlignment.Right;

			Cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			TextLabel.TextAlignment = UITextAlignment.Left;

			if (InventoryItem.Icon != null)
				Cell.ImageView.Image = InventoryItem.Icon.ImageToFitSize(new SizeF(48, 48));

			Cell.TextLabel.Text = InventoryItem.ToString();
			Cell.DetailTextLabel.Text =  string.Format("{0:c2} [{1} days]", InventoryItem.TotalCost, InventoryItem.DaysInHeat);
			Cell.DetailTextLabel.TextColor = UIColor.Red;
		}

		public override float GetHeight(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 52;
		}

		public InventoryElement() : this(string.Empty)
		{
		}

		public InventoryElement(string caption) : base(caption)
		{
		}
	}
}

