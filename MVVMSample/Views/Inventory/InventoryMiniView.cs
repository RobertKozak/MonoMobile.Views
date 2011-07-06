using System.Drawing;
using System.Linq;
namespace DealerCenter
{
	using System;
	using MonoTouch.Foundation;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class InventoryMiniView : RootElement, ISelectable
	{
		private static UIImage _StarImage = UIImage.FromFile("Images/Star.png");

		private InventoryItemViewModel InventoryItem
		{
			get { return (InventoryItemViewModel)DataContext; }
		}

		public UILabel Description { get; set;}
		public UILabel Trim { get; set; }
		public UILabel DrivetrainEngine { get; set; }
		public UILabel Stock { get; set; }

		public UILabel VIN { get; set; }
		public UILabel Mileage { get; set; }
		public UILabel Status { get; set; }
		public UILabel DateInStock { get; set; }
		public UILabel VehicleCost { get; set; }
		public UILabel TotalAdds { get; set; }
		public UILabel TotalCost { get; set; }

		public UILabel StockLabel { get; set; }
		public UILabel VINLabel { get; set; }
		public UILabel MileageLabel { get; set; }
		public UILabel StatusLabel { get; set; }
		
		public UILabel DateInStockLabel { get; set; }
		public UILabel VehicleCostLabel { get; set; }
		public UILabel TotalAddsLabel { get; set; }
		public UILabel TotalCostLabel { get; set; }
		
		public UIImageView Favorite { get; set; }
		public UIImageView Photo { get; set; }
		
		public bool ShowFavorite 
		{ 
			get { return InventoryItem.Favorite; } 
			set 
			{
				Favorite.Hidden = !InventoryItem.Favorite;
			} 
		}

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
		}
		
		public override void InitializeContent()
		{
			base.InitializeContent();

			Photo = new UIImageView(new RectangleF(8, 8, 64, 64));
			Photo.Image = InventoryItem.Icon;

			Favorite = new UIImageView(new RectangleF(Cell.Frame.Right - 50, 2, 32, 32));
			Favorite.Image = _StarImage;
			Favorite.Hidden = !InventoryItem.Favorite;
		
			Description = new UILabel(new RectangleF(82, 8, 200, 22));
			Trim = new UILabel(new RectangleF(82, 30, 200, 22));
			DrivetrainEngine = new UILabel(new RectangleF(82, 50, 200, 22));
			Stock = new UILabel(new RectangleF(82, 80, 200, 20));
			VIN = new UILabel(new RectangleF(82, 100, 200, 20));
			Mileage = new UILabel(new RectangleF(82, 120, 200, 20));
			Status = new UILabel(new RectangleF(82, 140, 200, 20));
			DateInStock = new UILabel(new RectangleF(220, 80, 75, 20)) { TextAlignment = UITextAlignment.Right };
			VehicleCost = new UILabel(new RectangleF(220, 100, 75, 20)) { TextAlignment = UITextAlignment.Right };
			TotalAdds = new UILabel(new RectangleF(220, 120, 75, 20)) { TextAlignment = UITextAlignment.Right };
			TotalCost = new UILabel(new RectangleF(220, 140, 75, 20)) { TextAlignment = UITextAlignment.Right };
		
			StockLabel = new UILabel(new RectangleF(10, 80, 56, 20)) { Text = "Stock" };
			VINLabel = new UILabel(new RectangleF(10, 100, 56, 20)) { Text = "VIN" };
			MileageLabel = new UILabel(new RectangleF(10, 120, 56, 20)) { Text = "Mileage" };
			StatusLabel = new UILabel(new RectangleF(10, 140, 56, 20)) { Text = "Status" };
			
			DateInStockLabel = new UILabel(new RectangleF(170, 80, 50, 20)) { Text = "Date" };
			VehicleCostLabel = new UILabel(new RectangleF(170, 100, 50, 20)) { Text = "Cost" };
			TotalAddsLabel = new UILabel(new RectangleF(170, 120, 50, 20)) { Text = "Adds" };
			TotalCostLabel = new UILabel(new RectangleF(170, 140, 50, 20)) { Text = "Total" };

			ContentView = new UIView(RectangleF.Empty);
			ContentView.BackgroundColor = UIColor.FromRGB(128, 192, 255);
			
			ContentView.Add(Photo);

			ContentView.Add(Description);
			ContentView.Add(Trim);
			ContentView.Add(DrivetrainEngine);
			
			ContentView.Add(Stock);
			ContentView.Add(VIN);
			ContentView.Add(Mileage);
			ContentView.Add(Status);
			
			ContentView.Add(DateInStock);
			ContentView.Add(VehicleCost);
			ContentView.Add(TotalAdds);
			ContentView.Add(TotalCost);

			ContentView.Add(StockLabel);
			ContentView.Add(VINLabel);
			ContentView.Add(MileageLabel);
			ContentView.Add(StatusLabel);

			ContentView.Add(DateInStockLabel);
			ContentView.Add(VehicleCostLabel);
			ContentView.Add(TotalAddsLabel);
			ContentView.Add(TotalCostLabel);

			Description.Font = UIFont.BoldSystemFontOfSize(17);
			Trim.Font = UIFont.SystemFontOfSize(17);
			DrivetrainEngine.Font = UIFont.SystemFontOfSize(17);

			StockLabel.Font = UIFont.SystemFontOfSize(14);
			VINLabel.Font = UIFont.SystemFontOfSize(14);
			MileageLabel.Font = UIFont.SystemFontOfSize(14);
			StatusLabel.Font = UIFont.SystemFontOfSize(14);

			Description.BackgroundColor = UIColor.Clear;
			Trim.BackgroundColor = UIColor.Clear;
			DrivetrainEngine.BackgroundColor = UIColor.Clear;

			StockLabel.BackgroundColor = UIColor.Clear;
			VINLabel.BackgroundColor = UIColor.Clear;
			MileageLabel.BackgroundColor = UIColor.Clear;
			StatusLabel.BackgroundColor = UIColor.Clear;
			
			StockLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			VINLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			MileageLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			StatusLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			
			DateInStockLabel.Font = UIFont.SystemFontOfSize(14);
			VehicleCostLabel.Font = UIFont.SystemFontOfSize(14);
			TotalAddsLabel.Font = UIFont.SystemFontOfSize(14);
			TotalCostLabel.Font = UIFont.SystemFontOfSize(14);
			
			DateInStockLabel.BackgroundColor = UIColor.Clear;
			VehicleCostLabel.BackgroundColor = UIColor.Clear;
			TotalAddsLabel.BackgroundColor = UIColor.Clear;
			TotalCostLabel.BackgroundColor = UIColor.Clear;
			
			DateInStockLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			VehicleCostLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			TotalAddsLabel.TextColor = UIColor.FromRGB(102, 102, 102);
			TotalCostLabel.TextColor = UIColor.FromRGB(102, 102, 102);

			Stock.BackgroundColor = UIColor.Clear;
			VIN.BackgroundColor = UIColor.Clear;
			Mileage.BackgroundColor = UIColor.Clear;
			Status.BackgroundColor = UIColor.Clear;

			Stock.Font = UIFont.SystemFontOfSize(14);
			VIN.Font = UIFont.SystemFontOfSize(14);
			Mileage.Font = UIFont.SystemFontOfSize(14);
			Status.Font = UIFont.SystemFontOfSize(14);

			Stock.TextColor = UIColor.FromRGB(128, 64, 0);
			VIN.TextColor = UIColor.FromRGB(128, 64, 0);
			Mileage.TextColor = UIColor.FromRGB(128, 64, 0);
			Status.TextColor = UIColor.FromRGB(0, 128, 0);

			DateInStock.BackgroundColor = UIColor.Clear;
			VehicleCost.BackgroundColor = UIColor.Clear;
			TotalAdds.BackgroundColor = UIColor.Clear;
			TotalCost.BackgroundColor = UIColor.Clear;
			
			DateInStock.Font = UIFont.SystemFontOfSize(14);
			VehicleCost.Font = UIFont.SystemFontOfSize(14);
			TotalAdds.Font = UIFont.SystemFontOfSize(14);
			TotalCost.Font = UIFont.SystemFontOfSize(14);
			
			DateInStock.TextColor = UIColor.FromRGB(128, 64, 0);
			VehicleCost.TextColor = UIColor.FromRGB(128, 64, 0);
			TotalAdds.TextColor = UIColor.FromRGB(128, 64, 0);
			TotalCost.TextColor = UIColor.FromRGB(0, 128, 0);


			Description.Text = InventoryItem.ToString();
			Trim.Text  = InventoryItem.Trim;
			DrivetrainEngine.Text  = string.Format("{0} {1} {2}", InventoryItem.Transmission, InventoryItem.Drivetrain, InventoryItem.Engine);
			
			Stock.Text  = InventoryItem.StockNumber;
			VIN.Text  = InventoryItem.Vin.Substring(11,6);
			Mileage.Text  = InventoryItem.Mileage.ToString();
			Status.Text = InventoryItem.Status.GetDescription().Capitalize();
			
			DateInStock.Text = InventoryItem.DateInStock.ToShortDateString();
			VehicleCost.Text = string.Format("{0:c2}", InventoryItem.VehicleCost);
			TotalAdds.Text = string.Format("{0:c2}", InventoryItem.TotalAdds);
			TotalCost.Text = string.Format("{0:c2}", InventoryItem.TotalCost);

			ContentView.Layer.ShadowColor = UIColor.Black.CGColor;
			ContentView.Layer.ShadowOffset = new SizeF(5f, 5f);
			ContentView.Layer.ShadowOpacity = 0.4f;
			ContentView.Layer.ShadowRadius = 2f;

			ContentView.Add(Favorite);
		}

		public override float GetHeight(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 168;
		}
		
		public InventoryMiniView() : base()
		{
			DataTemplate = new InventoryMiniViewDataTemplate(this);
		}

		public InventoryMiniView(string caption) : base(caption)
		{
			DataTemplate = new InventoryMiniViewDataTemplate(this);
		}
	}
}

