using MonoTouch.UIKit;
namespace DealerCenter
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using MonoMobile.MVVM;

	public class InventoryViewModel : ViewModel
	{
		private new InventoryModel DataModel { get { return (InventoryModel)base.GetModel(); } set { base.SetModel(value); } }

		public ObservableCollection<InventoryItemViewModel> DCInventory { get { return Get(()=> DataModel.DCInventory); } }

		public ObservableCollection<InventoryItemViewModel> LocalInventory { get { return Get(() => DataModel.LocalInventory); } }
		public ObservableCollection<InventoryItemViewModel> FavoriteInventory { get { return Get(() => DataModel.FavoriteInventory); } }


		public ObservableCollection<InventoryItemViewModel> CurrentInventoryList
		{
			get { return Get(() => CurrentInventoryList); }
			set { Set(() => CurrentInventoryList, value); }
		}

		public string LoginCaption
		{
			get { return Get(() => LoginCaption); }
			set { Set(() => LoginCaption, value); }
		}

		public InventoryViewModel(InventoryModel model)
		{
			DataModel = model;
			LoginCaption = "Login";
			CurrentInventoryList = DCInventory;
		}

		public override void Refresh()
		{
			CurrentInventoryList.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2007,
				Make = "BMW",
				Model = "325i", 
				Trim = "4D Sedan", 
				StockNumber = "S2344", 
				Vin = "S024GPATN836DF834",
				Transmission = "Automatic",
				Mileage = 102324,
				Drivetrain = "RWD",
				Engine = "V6",
				ExteriorColor = "Black",
				VehicleCost = 11345.00f,
				TotalAdds = 0f,
				DateInStock = DateTime.Now.AddDays(-15),
				Icon = UIImage.FromFile("Images/car1.png").RemoveSharpEdges(5)
			});

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

