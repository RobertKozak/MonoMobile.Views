using System;
using MonoMobile.MVVM;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace MVVMSample
{
	public class TestViewModel: ViewModel
	{
		public string UserName 
		{
			get { return Get(()=>UserName); }
			set { Set(()=>UserName, value); }
		}
		
		public string TestString
		{
			get { return Get(() => TestString); }
			set { Set(() => TestString, value); }
		}

		public bool TestBool
		{
			get { return Get(()=>TestBool); }
			set { Set(()=>TestBool, value); }
		}

		public double TestDouble
		{
			get { return Get(() => TestDouble); }
			set { Set(() => TestDouble, value); }
		}
		
		public Single TestSingle
		{
			get { return Get(() => TestSingle); }
			set { Set(() => TestSingle, value); }
		}

		public float TestFloat
		{
			get { return Get(() => TestFloat); }
			set { Set(() => TestFloat, value); }
		}
		
		public int TestInt
		{
			get { return Get(() => TestInt); }
			set { Set(() => TestInt, value); }
		}
		
		public Int16 TestInt16
		{
			get { return Get(() => TestInt16); }
			set { Set(() => TestInt16, value); }
		}

		public Int32 TestInt32
		{
			get { return Get(() => TestInt32); }
			set { Set(() => TestInt32, value); }
		}

		public Int64 TestInt64
		{
			get { return Get(() => TestInt64); }
			set { Set(() => TestInt64, value); }
		}
		
		public UInt16 TestUInt16
		{
			get { return Get(() => TestUInt16); }
			set { Set(() => TestUInt16, value); }
		}

		public UInt32 TestUInt32
		{
			get { return Get(() => TestUInt32); }
			set { Set(() => TestUInt32, value); }
		}

		public UInt64 TestUInt64
		{
			get { return Get(() => TestUInt64); }
			set { Set(() => TestUInt64, value); }
		}

		public Byte TestByte
		{
			get { return Get(() => TestByte); }
			set { Set(() => TestByte, value); }
		}

		public SByte TestSByte
		{
			get { return Get(() => TestSByte); }
			set { Set(() => TestSByte, value); }
		}

		public Decimal TestDecimal
		{
			get { return Get(() => TestDecimal); }
			set { Set(() => TestDecimal, value); }
		}

		public DateTime TestDateTime
		{
			get { return Get(() => TestDateTime); }
			set { Set(() => TestDateTime, value); }
		}
		
		public DateTime TestTime
		{
			get { return Get(() => TestTime); }
			set { Set(() => TestTime, value); }
		}

		public DateTime TestDate
		{
			get { return Get(() => TestDate); }
			set { Set(() => TestDate, value); }
		}

		public string TestUri
		{
			get { return Get(() => TestUri); }
			set { Set(() => TestUri, value); }
		}
		
		public Uri TestUri2
		{
			get { return Get(() => TestUri2); }
			set { Set(() => TestUri2, value); }
		}

		public CLLocationCoordinate2D TestLocation
		{
			get { return Get(() => TestLocation); }
			set { Set(() => TestLocation, value); }
		}
		
		public UIImage TestImage
		{
			get { return Get(() => TestImage); }
			set { Set(() => TestImage, value); }
		}
		
		public TestEnum TestEnum
		{
			get { return Get(() => TestEnum); }
			set { Set(() => TestEnum, value); }
		}

		public InventoryItemViewModel Inventory
		{
			get { return Get(() => Inventory); }
			set { Set(() => Inventory, value); }
		}
		
		public ObservableCollection<InventoryItemViewModel> InventoryCollection
		{
			get { return Get(() => InventoryCollection); }
			private set { Set(() => InventoryCollection, value); }
		}
		
		public List<InventoryItemViewModel> InventoryList
		{
			get { return Get(() => InventoryList); }
			private set { Set(() => InventoryList, value); }
		}
		
		public EnumCollection<TestEnum> TestEnumCollection
		{
			get { return Get(() => TestEnumCollection); }
			private set { Set(() => TestEnumCollection, value); }
		}
		
		private ObservableCollection<EnumItem> TestSelected;
		private EnumItem TestSelectedItem;

		public TestViewModel()
		{
			UserName = "Robert";
			TestBool = true;
			TestDouble = 3.4f;
			TestSingle = 3.5f;
			TestFloat = 3.6f;

			TestInt = 1;
			TestInt16 = 16;
			TestInt32 = 32;
			TestInt64 = 64;

			TestUri = "Http://www.nowcom.com";
			TestUri2 = new Uri("Http://www.google.com");

			TestLocation = new CLLocationCoordinate2D(34.062110, -118.333009);

			TestImage = UIImage.FromBundle("Images/bluesky.jpg");

			Inventory = new InventoryItemViewModel()
			{
				Id = 1,
				Year = 2010,
				Make = "BMW",
				Model = "7 Series", 
				Trim = "750 Li", 
				StockNumber = "S2345", 
				Vin = "S4SDDSFSE344DF333"
			};

			InventoryList = new List<InventoryItemViewModel>();
			
			InventoryList.Add(new InventoryItemViewModel()
          	{
				Id = 1,
				Year = 2010,
				Make = "BMW",
				Model = "7 Series", 
				Trim = "750 Li", 
				StockNumber = "S2345", 
				Vin = "S4SDDSFSE344DF333"
			});

			InventoryList.Add(new InventoryItemViewModel()
          	{
				Id = 2,
				Year = 2009,
				Make = "Mercedes",
				Model = "E Class", 
				Trim = "E350", 
				StockNumber = "S2346", 
				Vin = "B4SXY345E344DF333"
			});
			InventoryList.Add(new InventoryItemViewModel()
          	{
				Id = 3,
				Year = 2010,
				Make = "Toyota",
				Model = "Camry", 
				Trim = "XE", 
				StockNumber = "S2348", 
				Vin = "W4UVRKQHI323AB821"
			});
			InventoryList.Add(new InventoryItemViewModel()
	      	{
				Id = 4,
				Year = 2008,
				Make = "Honda",
				Model = "Accord", 
				Trim = "VE", 
				StockNumber = "S2349", 
				Vin = "S024GPATN836DF834"
			});

			InventoryCollection = new ObservableCollection<InventoryItemViewModel>(InventoryList);
			
			TestEnumCollection = new EnumCollection<TestEnum>();

			TestSelected = new ObservableCollection<EnumItem>();
		}
	}

	public enum TestEnum
	{
		One,
		Two,
		Three,
		Apple,
		Orange,
		[Description("Slippers & Socks")]
		SlippersAndSocks
	}
}

