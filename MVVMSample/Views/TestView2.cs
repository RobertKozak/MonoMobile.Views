using System;
using MonoMobile.MVVM;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace DealerCenter
{
	public class TestView : View
	{
		private new TestViewModel DataContext { get { return (TestViewModel)GetDataContext(); } set { SetDataContext(value); } }

//	[Section]
		[Entry]
		[Bind("TestBool", "IsPassword")]
		public string UserName = "Test";
//
//		public string TestString = "This is a test";
//		
		[Entry]
		public bool TestBool;
		[Entry]
		public double TestDouble;
		public Single TestSingle;
		[Range(0, 100)]
		public float TestFloat;
//		
//		[Entry]
//		public int TestInt;
//		public Int16 TestInt16;
//		public Int32 TestInt32;
//		public Int64 TestInt64;
//		
//		[Entry]
//		public UInt16 TestUInt16;
//		public UInt32 TestUInt32;
//		public UInt64 TestUInt64;
//		
//		[Entry]
//		public Byte TestByte;
//		public SByte TestSByte;
//		[Entry]
//		public Decimal TestDecimal;
//		
//	[Section]
//		[Date]
//		public DateTime TestDateTime;
//
//		[Time]
//		public DateTime TestTime;
//
//		[Date]
//		public DateTime TestDate;
//
//		[Html]
//		public string TestUri;
//
//		[Html]
//		public Uri TestUri2;
//		
//		[Map("Nowcom")]
//		public CLLocationCoordinate2D TestLocation;
//		
//		public UIImage TestImage;

//	[Section]
//		public List<string> TestStrings;

//		public TestEnum TestEnum;
//		//[List(ViewType = typeof(InventoryView))]
//		[List(ViewType = typeof(InventoryView))]
//		public InventoryItemViewModel Inventory;
//		//[List(ViewType = typeof(InventoryView))]
//		[Root(ViewType = typeof(InventoryView))]
//		public ObservableCollection<InventoryItemViewModel> InventoryCollection;
//		[List(ViewType = typeof(InventoryView))]
//		[Root(ViewType = typeof(InventoryView))]
//		[Selection]
//		[Bind("TestSelected", "SelectedItems")]
//		[Bind("TestSelectedItem", "SelectedItem")]
//		public List<InventoryItemViewModel> InventoryList;
//
//		[List]
//		[Bind("TestSelected", "SelectedItems")]
//		[Bind("TestSelectedItem", "SelectedItem")]
//		public EnumCollection<TestEnum> TestEnumCollection;
		
	[Section]
		[Button]
		public void Test()
		{
			DataContext.ACaption = "Testing...";
			//var x = UserName;
			DataContext.TestBool = !DataContext.TestBool;
			DataContext.TestDouble += 1.3f;
			DataContext.TestSingle += 1.2f;
			DataContext.TestFloat += 1.1f;

			DataContext.UserName = "Robert";
			
			DataContext.TestInt += 2;
			DataContext.TestInt16 += 2;
			DataContext.TestInt32 += 2;
			DataContext.TestInt64 += 2;
			
			DataContext.TestUInt16 += 2;
			DataContext.TestUInt32 += 2;
			DataContext.TestUInt64 += 2;

			DataContext.TestByte += 2;
			
			DataContext.TestSByte += 4;

			DataContext.TestString = "A new string";

			DataContext.TestDecimal = 5.55m;

			DataContext.TestDateTime = DateTime.Now; 
			DataContext.TestTime = DateTime.Now;
			DataContext.TestDate = DateTime.Now;

			DataContext.TestEnum = TestEnum.Two;

//			DataContext.Inventory = new InventoryItemViewModel()
//			{	
//				Id = 2,
//				Year = 2009,
//				Make = "Mercedes",
//				Model = "E Class", 
//				Trim = "E350", 
//				StockNumber = "S2346", 
//				Vin = "B4SXY345E344DF333"
//			};

		}
		
//		[NavbarButton]
//		public void AddInt16()
//		{
//			DataContext.TestInt16 += 4;
//		}

		public TestView()
		{
			DataContext = new TestViewModel();
			DataContext.UserName = "Rkozak";
		}
	}

	public class NoTheme : Theme {}
}

