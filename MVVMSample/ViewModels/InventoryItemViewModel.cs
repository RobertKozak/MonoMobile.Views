namespace MVVMSample
{
	using System;
	using MonoMobile.MVVM;
	using Nowcom.VersionedLocalDB.DMS.Inventory;

	public class InventoryItemViewModel : ViewModel
	{
		public int Id 
		{ 
			get { return Get(() => Id); } 
			set { Set(() => Id, value); }
		} 

		public string StockNumber
		{ 
			get { return Get(() => StockNumber); } 
			set { Set(() => StockNumber, value); }
		}

		public int Year	
		{ 
			get { return Get(() => Year); } 
			set { Set(() => Year, value); }
		}

		public string Make	
		{ 
			get { return Get(() => Make); } 
			set { Set(() => Make, value); }
		}

		public string Model	
		{ 
			get { return Get(() => Model); } 
			set { Set(() => Model, value); }
		}

		public string Trim	
		{ 
			get { return Get(() => Trim); } 
			set { Set(() => Trim, value); }
		}

		public string Vin	
		{ 
			get { return Get(() => Vin); } 
			set { Set(() => Vin, value); }
		}

		public InventoryItem.InventoryStatus Status	
		{ 
			get { return Get(() => Status); } 
			set { Set(() => Status, value); }
		}


		public void KelleyMethod()
		{
			Year = 1990;
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Year, Make, Model);
		}
	}
}

