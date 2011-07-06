namespace DealerCenter
{
	using System;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class AutoCheckHistoryElement : Element
	{
		private AutoCheckHistoryItem HistoryItem { get { return (AutoCheckHistoryItem)DataContext; } }

		public AutoCheckHistoryElement(): base(string.Empty)
		{
		}

		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell(tableView);
			
			DetailTextLabel.TextAlignment = UITextAlignment.Right;
			
			Cell.Accessory = UITableViewCellAccessory.None;

			TextLabel.TextAlignment = UITextAlignment.Left;
			TextLabel.Font = UIFont.BoldSystemFontOfSize(14);
			DetailTextLabel.Font = UIFont.BoldSystemFontOfSize(14);

			TextLabel.Text = HistoryItem.ToString();
			DetailTextLabel.Text = HistoryItem.Result ? "YES" : "NO";
			DetailTextLabel.TextColor = HistoryItem.Result ? UIColor.Red : UIColor.Blue;
		}
		
		public override float GetHeight(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 30f;
		}
	}
}

