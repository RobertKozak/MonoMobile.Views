namespace MVVMSample
{
	using System;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class InventoryElement : RootElement
	{
	//	public BindableProperty YearProperty = BindableProperty.Register("Year");

		
		public UILabel YearLabel { get; set; }
		public override void InitializeContent()
		{
			ContentView = new UIView() { Opaque = false, BackgroundColor = UIColor.Clear };
			YearLabel = new UILabel(Cell.Bounds) { Opaque = false, BackgroundColor = UIColor.Clear };

			ContentView.Add(YearLabel);
		}
		
		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell(tableView);

			DetailTextAlignment = UITextAlignment.Right;
			Cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;
			TextAlignment = UITextAlignment.Left;
		}
		
		public override void BindProperties()
		{
			base.BindProperties();
			DataContextProperty.BindTo(this, YearLabel, "Text");
		}

		public InventoryElement() : base()
		{
		}

		public InventoryElement(string caption) : base(caption)
		{
		}
	}
}

