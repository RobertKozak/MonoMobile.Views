namespace DealerCenter
{
	using System;
	using System.Drawing;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers = true)]
	public class AutoCheckVehicleView : RootElement, ISelectable
	{
		private AutoCheckViewModel AutoCheckReport { get { return (AutoCheckViewModel)DataContext; } }


//		public BindableProperty DescriptionProperty = BindableProperty.Register("Description");
//		public BindableProperty PhotoProperty = BindableProperty.Register("Photo");

		public UILabel Description { get; set; }
		public UILabel Trim { get; set; }
		public UILabel DrivetrainEngine { get; set; }
		public UILabel Stock { get; set; }
		public UILabel VIN { get; set; }
		public UILabel CountryOfAssembly { get; set; }
		public UILabel StockLabel { get; set; }
		public UILabel VINLabel { get; set; }
		public UILabel CountryOfAssemblyLabel { get; set; }
		public UIImageView Photo { get; set; }

//		public InventoryItemViewModel InventoryItem
//		{
//			get { return _InventoryItem; }
//			set { _InventoryItem = value; }
//		}
//
//		private InventoryItemViewModel _InventoryItem { get; set; }

		public new void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
		}

		public override UITableViewElementCell NewCell()
		{
			return new UITableViewElementCell(UITableViewCellStyle.Default, Id, this);
		}

		public override void InitializeCell(UITableView tableView)
		{
			base.InitializeCell(tableView);
			
			Cell.TextLabel.Text = string.Empty;
			
			Cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			Cell.Accessory = UITableViewCellAccessory.None;
			TextLabel.TextAlignment = UITextAlignment.Left;
			//		Description = string.Format("{0}     {1}", InventoryItem.StockNumber, InventoryItem.Vin);
			
			//	Cell.TextLabel.Text = InventoryItem.ToString();
			//	Cell.DetailTextLabel.Text = string.Format("{0}     {1}", InventoryItem.StockNumber, InventoryItem.Vin);
			//		Cell.DetailTextLabel.TextColor = UIColor.Red;
			
			
			
			
		}

		public override void InitializeContent()
		{
			base.InitializeContent();
			
			Photo = new UIImageView(new RectangleF(8, 8, 64, 64));
			Photo.Image = AutoCheckReport.Inventory.Icon;
			
			Description = new UILabel(new RectangleF(82, 8, 200, 22));
			Trim = new UILabel(new RectangleF(82, 30, 200, 22));
			DrivetrainEngine = new UILabel(new RectangleF(82, 50, 200, 22));
			Stock = new UILabel(new RectangleF(82, 80, 200, 20));
			VIN = new UILabel(new RectangleF(82, 100, 200, 20));
			CountryOfAssembly = new UILabel(new RectangleF(82, 120, 200, 20));

			
			StockLabel = new UILabel(new RectangleF(10, 80, 56, 20)) { Text = "Stock" };
			VINLabel = new UILabel(new RectangleF(10, 100, 56, 20)) { Text = "VIN" };
			CountryOfAssemblyLabel = new UILabel(new RectangleF(10, 120, 62, 20)) { Text = "Assembly" };

			ContentView = new UIView(RectangleF.Empty) { Tag = 1 };
			ContentView.BackgroundColor = UIColor.FromRGB(255, 239, 163);
			ContentView.Add(Photo);
			
			ContentView.Add(Description);
			ContentView.Add(Trim);
			ContentView.Add(DrivetrainEngine);
			
			ContentView.Add(Stock);
			ContentView.Add(VIN);
			ContentView.Add(CountryOfAssembly);
			
			ContentView.Add(StockLabel);
			ContentView.Add(VINLabel);
			ContentView.Add(CountryOfAssemblyLabel);
			
			Description.Font = UIFont.BoldSystemFontOfSize(17);
			Trim.Font = UIFont.SystemFontOfSize(17);
			DrivetrainEngine.Font = UIFont.SystemFontOfSize(17);
			
			StockLabel.Font = UIFont.SystemFontOfSize(14);
			VINLabel.Font = UIFont.SystemFontOfSize(14);
			CountryOfAssemblyLabel.Font = UIFont.SystemFontOfSize(14);
			
			Description.BackgroundColor = UIColor.Clear;
			Trim.BackgroundColor = UIColor.Clear;
			DrivetrainEngine.BackgroundColor = UIColor.Clear;
			
			StockLabel.BackgroundColor = UIColor.Clear;
			VINLabel.BackgroundColor = UIColor.Clear;
			CountryOfAssemblyLabel.BackgroundColor = UIColor.Clear;
			
			StockLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			VINLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			CountryOfAssemblyLabel.TextColor = UIColor.FromRGB(102, 102, 102);
		
			Stock.BackgroundColor = UIColor.Clear;
			VIN.BackgroundColor = UIColor.Clear;
			CountryOfAssembly.BackgroundColor = UIColor.Clear;
		
			Stock.Font = UIFont.SystemFontOfSize(14);
			VIN.Font = UIFont.SystemFontOfSize(14);
			CountryOfAssembly.Font = UIFont.SystemFontOfSize(14);
			
			Stock.TextColor = UIColor.FromRGB(128, 64, 0);
			VIN.TextColor = UIColor.FromRGB(128, 64, 0);
			CountryOfAssembly.TextColor = UIColor.FromRGB(128, 64, 0);
	
			
			Description.Text = AutoCheckReport.Inventory.ToString();
			Trim.Text = AutoCheckReport.Inventory.Trim;
			DrivetrainEngine.Text = string.Format("{0} {1}", AutoCheckReport.Inventory.Drivetrain, AutoCheckReport.Inventory.Engine);
			
			Stock.Text = AutoCheckReport.Inventory.StockNumber;
			VIN.Text = AutoCheckReport.Inventory.Vin;
			CountryOfAssembly.Text = AutoCheckReport.CountryOfAssembly;
			
			ContentView.Layer.ShadowColor = UIColor.Black.CGColor;
			ContentView.Layer.ShadowOffset = new SizeF(5f, 5f);
			ContentView.Layer.ShadowOpacity = 0.4f;
			ContentView.Layer.ShadowRadius = 2f;
		}

//		public void BindProperties()
//		{
////			DataContextProperty.BindTo(this, this, "Inventory");
////			PhotoProperty.BindTo(this, Photo, "Image");
//		}

		public override float GetHeight(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 148;
		}

		public AutoCheckVehicleView() : base()
		{
		}

		public AutoCheckVehicleView(string caption) : base(caption)
		{

		}
	}
}

