namespace DealerCenter
{
	using System;
	using MonoMobile.MVVM;
	using Nowcom.VersionedLocalDB.DMS.Inventory;
	using MonoTouch.UIKit;
	using System.Collections.Generic;

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
		
		public string Transmission
		{
			get { return Get(() => Transmission); }
			set { Set(() => Transmission, value); }
		}

		public string Drivetrain
		{
			get { return Get(() => Drivetrain); }
			set { Set(() => Drivetrain, value); }
		}

		public string Engine
		{
			get { return Get(() => Engine); }
			set { Set(() => Engine, value); }
		}
		
		public int Mileage
		{
			get { return Get(() => Mileage); }
			set { Set(() => Mileage, value); }
		}

		public string Vin	
		{ 
			get { return Get(() => Vin); } 
			set { Set(() => Vin, value); }
		}
		
		public string ExteriorColor
		{
			get { return Get(() => ExteriorColor); }
			set { Set(() => ExteriorColor, value); }
		}

		public DateTime DateInStock
		{
			get { return Get(() => DateInStock); }
			set { Set(() => DateInStock, value); }
		}
		
		public int DaysInHeat
		{
			get { return (DateTime.Now - Get(() => DateInStock)).Days; }
		}

		public float VehicleCost
		{
			get { return Get(() => VehicleCost); }
			set { Set(() => VehicleCost, value); }
		}

		public float TotalCost
		{
			get { return VehicleCost + TotalAdds; }
		}

		public float TotalAdds
		{
			get { return Get(() => TotalAdds); }
			set { Set(() => TotalAdds, value); }
		}

		public InventoryItem.InventoryStatus Status	
		{ 
			get { return Get(() => Status); } 
			set { Set(() => Status, value); }
		}
		
		public UIImage Icon 
		{
			get { return Get(() => Icon); }
			set { Set(() => Icon, value); }
		}
		
		public UIImage Photo
		{
			get { return Get(() => Photo); }
			set { Set(() => Photo, value); }
		}

		public bool Favorite
		{
			get { return Get(() => Favorite); }
			set { Set(() => Favorite, value); }
		}

		public List<string> Equipment = new List<string>() {"Alloy Wheels", "Working Brakes", "Sunroof", "DVD Player" }; 
		
		public NadaViewModel Nada 
		{
			get { return Get(() => Nada); }
			private	set { Set(() => Nada, value); }
		}

		public KelleyViewModel Kelley
		{
			get { return Get(() => Kelley); }
			private set { Set(() => Kelley, value); }
		}

		public BlackBookViewModel BlackBook
		{
			get { return Get(() => BlackBook); }
			private set { Set(() => BlackBook, value); }
		}
		
		public AutoCheckViewModel AutoCheck
		{
			get { return Get(() => AutoCheck); }
			set { Set(() => AutoCheck, value); }
		}
		
		public bool KelleyBooked
		{
			get { return Get(() => KelleyBooked); }
			set { Set(() => KelleyBooked, value); }
		}
		
		public bool NADABooked
		{
			get { return Get(() => NADABooked); }
			set { Set(() => NADABooked, value); }
		}

		public bool BlackBookBooked
		{
			get { return Get(() => BlackBookBooked); }
			set { Set(() => BlackBookBooked, value); }
		}

		public bool AutoCheckBooked
		{
			get { return Get(() => AutoCheckBooked); }
			set { Set(() => AutoCheckBooked, value); }
		}
		
		public UIFont AFont
		{
			get { return Get(() => AFont, UIFont.SystemFontOfSize(22)); }
			set { Set(() => AFont, value); }
		}
		
		public bool IsLocal
		{
			get { return Get(() => IsLocal); }
			set { Set(() => IsLocal, value); }
		}

		public void KelleyMethod()
		{
			Year = 1990;
		}
		
		public InventoryItemViewModel()
		{
			Nada = new NadaViewModel() { InventoryItem = this  };
			Kelley = new KelleyViewModel() { InventoryItem = this };
			BlackBook = new BlackBookViewModel() { InventoryItem = this };
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Year, Make, Model);
		}
	}
}

