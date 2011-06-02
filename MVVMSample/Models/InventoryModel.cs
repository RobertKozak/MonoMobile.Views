namespace MVVMSample
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using MonoMobile.MVVM;

	public class InventoryModel : Model
	{
		public ObservableCollection<InventoryItemViewModel> Inventory 
		{ 
			get { return Get(() => Inventory); } 
			set { Set(() => Inventory, value); }
		}

		public override void Load()
		{
			Inventory = new ObservableCollection<InventoryItemViewModel>();
			
			Inventory.Add(new InventoryItemViewModel()
          	{
				Id = 1,
				Year = 2010,
				Make = "BMW",
				Model = "7 Series", 
				Trim = "750 Li", 
				StockNumber = "S2345", 
				Vin = "S4SDDSFSE344DF333"
			});

			Inventory.Add(new InventoryItemViewModel()
          	{
				Id = 2,
				Year = 2009,
				Make = "Mercedes",
				Model = "E Class", 
				Trim = "E350", 
				StockNumber = "S2346", 
				Vin = "B4SXY345E344DF333"
			});
			Inventory.Add(new InventoryItemViewModel()
          	{
				Id = 3,
				Year = 2010,
				Make = "Toyota",
				Model = "Camry", 
				Trim = "XE", 
				StockNumber = "S2348", 
				Vin = "W4UVRKQHI323AB821"
			});
			Inventory.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2008,
				Make = "Honda",
				Model = "Accord", 
				Trim = "VE", 
				StockNumber = "S2349", 
				Vin = "S024GPATN836DF834"
			});
		}

		public override void Save()
		{
		}

		public override void Refresh()
		{
			Inventory.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2007,
				Make = "BMW",
				Model = "325i", 
				Trim = "4D Sedan", 
				StockNumber = "S2344", 
				Vin = "S024GPATN836DF834"
			});
		}
	}
}

