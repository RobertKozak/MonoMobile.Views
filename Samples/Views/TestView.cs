using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Samples
{
	using System;
	using MonoMobile.MVVM;

	[Theme(typeof(BackgroundImageTheme))]
	[Theme(typeof(NavbarTheme))]
	[Theme(typeof(FrostedTheme))]
	public class TestView : View
	{
	[Section("Enumerables")]
		//[Root]
		//[List]
		[Caption("Testing Enums")]
		public TestEnum TestEnum = TestEnum.Two;
		//[Root]
		//[List]
		[PopOnSelection]
		public TestEnum PopEnum;
		//[Root]
		//[List]
		public EnumCollection<TestEnum> EnumCollection = new EnumCollection<TestEnum>();

		//public string MyListSelection;
		//[Root]
		//[List]
		//[Bind("MyListSelection")]
		public IEnumerable MyList = new List<string>() { "Windows", "OS X", "Linux"};
		//[Root]
		//[List]
		public MultiselectCollection<string> MySelectableList = new MultiselectCollection<string>() { "Windows", "OS X", "Linux" }; 
		
		//[List]
		//[List(ViewType = typeof(MovieView))]
		[Root(ViewType=typeof(MovieView))]
		//[Root]
		public ObservableCollection<MovieViewModel> Movies;
		
		public MyObject MyObjectSelection = new MyObject("Selected");
		//[List]
		//[Root]
		[Bind("MyObjectSelection")]
		public List<MyObject> Tests = new List<MyObject>() { new MyObject("First Object"), new MyObject("Second Object") };

	[Section("Fields")]
		[Entry]
		public string String = "a string";
		public bool Bool = true;
		public int Int = 5;
		[Range(1, 30, ShowCaption = true)]
		public float Float = 8.3f;
		[Date]
		public DateTime DateTime1;
		[Time]
		public DateTime DateTime2 = DateTime.Now;
		public UIImage Image;
		public Uri Uri;
		[Map("A Map")]
		public CLLocationCoordinate2D Location;
		//[List]
		public MyObject MyObject2;

	[Section("Properties")]
		public string StringProperty { get; set; }
		public bool BoolProperty { get; set; }
		public int IntProperty { get; set; }
		public float FloatProperty { get; set; }
		public DateTime DateTime1Property { get; set; }
		public UIImage ImageProperty { get; set; }
		public Uri UriProperty { get; set; }
		[Map("A Map")]
		public CLLocationCoordinate2D LocationProperty { get; set; }

	[Section("Classes")]
		public MyObject MyObject { get; set; }
		
		[List]
		public ButtonTestView ButtonTestView { get; set; }
		
		[NavbarButton("Test")]
		public void Test()
		{
MyObject2 = new MyObject("Object 2");

		}
		public TestView()
		{
			ButtonTestView = new ButtonTestView();
			EnumCollection[1].IsChecked = true;
 
			Uri = new Uri("Http://www.google.com");
			DateTime1 = DateTime.Now;
			Image = UIImage.FromFile("Images/brick.jpg");
			Location = new CLLocationCoordinate2D(-33.867139, 151.207114);

			StringProperty = String;
			BoolProperty = Bool;
			IntProperty = Int;
			FloatProperty = Float;
			DateTime1Property = DateTime1;
			ImageProperty = Image;
			UriProperty = Uri;
			LocationProperty = Location;

			MyObject = new MyObject("Object 1");

			Movies = new ObservableCollection<MovieViewModel>();
			var dataModel = new MovieDataModel();
			dataModel.Load();
			Movies =dataModel.Movies;
		}
	}
	
	[Theme(typeof(HoneyDoTheme))]
	public class MyObject :View
	{
		public string Name;
		public string TestString = "A Test String"; 

	[Section("Lists")]
		public TestEnum TestEnum;
		[PopOnSelection]
		public TestEnum PopEnum;
		public EnumCollection<TestEnum> EnumCollection = new EnumCollection<TestEnum>();
		public IEnumerable MyList = new List<string>() { "Windows", "OS X", "Linux"};
		public MultiselectCollection<string> MySelectableList = new MultiselectCollection<string>() { "Windows", "OS X", "Linux" }; 

		[List(ViewType = typeof(MovieView))]
		public ObservableCollection<MovieViewModel> Movies;

		public MyObject(string name)
		{
			Name = name;

			Movies = new ObservableCollection<MovieViewModel>();
			var dataModel = new MovieDataModel();
			dataModel.Load();
			Movies =dataModel.Movies;
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

