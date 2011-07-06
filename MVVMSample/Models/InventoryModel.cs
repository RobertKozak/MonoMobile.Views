namespace DealerCenter
{
	using System;
	using System.Collections.ObjectModel;
	using System.Linq;
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;

	public class InventoryModel : Model
	{
		public ObservableCollection<InventoryItemViewModel> DCInventory 
		{ 
			get { return Get(() => DCInventory, new ObservableCollection<InventoryItemViewModel>()); } 
		}

		public ObservableCollection<InventoryItemViewModel> LocalInventory
		{
			get { return Get(() => LocalInventory, new ObservableCollection<InventoryItemViewModel>()); }
		}

		public ObservableCollection<InventoryItemViewModel> FavoriteInventory
		{
			get { return DCInventory.Union(LocalInventory).Where((inventory) => inventory.Favorite).ToObservableCollection(); }
		}

		public override void Load()
		{			
			DCInventory.Add(new InventoryItemViewModel()
          	{
				Id = 1,
				Year = 2010,
				Make = "BMW",
				Model = "7 Series", 
				Trim = "750 Li", 
				StockNumber = "S2345", 
				Vin = "S4SDDSFSE344DF333",
				Transmission = "Automatic",
				Mileage = 102324,
				Drivetrain = "RWD",
				Engine = "V6",
				ExteriorColor = "Black",
				VehicleCost = 11345.00f,
				TotalAdds = 0f,
				DateInStock = DateTime.Now.AddDays(-15),
				AutoCheckBooked = true,
				Icon = UIImage.FromFile("Images/car1.png").RemoveSharpEdges(5)
			});

			DCInventory.Add(new InventoryItemViewModel()
          	{
				Id = 2,
				Year = 2009,
				Make = "Mercedes",
				Model = "E Class", 
				Trim = "E350", 
				StockNumber = "S2346", 
				Vin = "B4SXY345E344DF333",
				Transmission = "Manual",
				Mileage = 125445,
				Drivetrain = "RWD",
				Engine = "V6",
				VehicleCost = 17595.00f,
				TotalAdds = 550f,
				ExteriorColor = "Anthracite Metallic",
				DateInStock = DateTime.Now.AddDays(-10),
				AutoCheckBooked = false,
				Icon = UIImage.FromFile("Images/car2.jpg").RemoveSharpEdges(5)
			});
			DCInventory.Add(new InventoryItemViewModel()
          	{
				Id = 3,
				Year = 2010,
				Make = "Toyota",
				Model = "Camry", 
				Trim = "XE", 
				StockNumber = "S2348", 
				Vin = "W4UVRKQHI323AB821",
				Transmission = "Automatic",
				Mileage = 78655,
				Drivetrain = "FWD",
				Engine = "V4",
				VehicleCost = 14100.00f,
				TotalAdds = 250f,
				ExteriorColor = "Metalic Blue",
				DateInStock = DateTime.Now.AddDays(-3),
				Icon = UIImage.FromFile("Images/car3.png").RemoveSharpEdges(5)
			});


			LocalInventory.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2008,
				Make = "Honda",
				Model = "Accord", 
				Trim = "VE", 
				Mileage = 13645,
				Transmission = "Automatic",
				Drivetrain = "RWD",
				Engine = "V4",
				StockNumber = "S2349", 
				Vin = "S024GPATN836DF834",
				IsLocal = true,
				Icon = UIImage.FromFile("Images/NoPhoto.png").RemoveSharpEdges(5)
			});
		}

		public override void Save()
		{
		}

		public override void Refresh()
		{
//			DCInventory.Add(new InventoryItemViewModel()
//	      	{
//				Id = 4,
//				Year = 2007,
//				Make = "BMW",
//				Model = "325i", 
//				Trim = "4D Sedan", 
//				StockNumber = "S2344", 
//				Vin = "S024GPATN836DF834"
//			});
		}
	}
}

