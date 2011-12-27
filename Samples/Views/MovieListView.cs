using MonoTouch.CoreAnimation;
namespace Samples
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using MonoTouch.CoreGraphics;
	using MonoTouch.CoreLocation;
	using MonoMobile.Views;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	using System.Linq;
	
	[Preserve(AllMembers = true)]
	[Theme(typeof(NavbarTheme))]
	public class MovieListView: View, INotifyPropertyChanged
	{
		[Section("Test Section", "")]
		[Order(2)]
		[List(SelectionAction = SelectionAction.Multiselection)]
		[Theme(typeof(CorkTheme))]
		public string TestEntry2 { get; set; }

		public bool TestBool { get; set; }

		[Date]
		public DateTime Date {get; set;}
		
		[Time]
		[Theme(typeof(GraniteTheme))]
		public DateTime Time {get; set;}

		[Password("Enter passsword")]
		[DefaultValue("Test String")]
		public string MyString { get; set; }

		[Section]
		[CellView(typeof(MovieView))]
		public ObservableCollection<MovieViewModel> Movies 
		{
			get;
			set;
		}
		
//		[LoadMore]
//		public void Activity()
//		{
//			Thread.Sleep(1500);
//		}
		
//		[Section]
//		[Progress("Getting values...", "Testing", IndicatorStyle = IndicatorStyle.AccessoryIndicator)]
//		public void Test()
//		{
//			System.Threading.Thread.Sleep(3000);
//		}

		[Section("Using a DataBinding", "This is a footer")]
		[NavigateToView(typeof(MovieView))]
		public ObservableCollection<MovieViewModel> Movies2 
		{
		get; set;
		}
		
		private float _CellSize = 44f; 
		private float CellSize 
		{ 
			get { return _CellSize; } 
			set { _CellSize = value; if (PropertyChanged != null) 
					PropertyChanged(this, new PropertyChangedEventArgs("CellSize")); }
		}

		[NavbarButton("Add")]
		public void AddMovie()
		{
			CellSize += 100;
//			CanChangeStyle = !CanChangeStyle;
//			((MovieListViewModel)DataContext).TestEntry2 = "Test Robert";
		}

	//	[Map("A Map")]
	//	public CLLocationCoordinate2D Location {get; set;}

			
		[Entry]
		[DefaultValue("Rocket")]
		public string TestEntry5 { get; set; } 

		[Section]
		[Caption("Root Address")]
		public AddressView AddressView { get; set; }
		
		
		[List(SelectionAction = SelectionAction.Multiselection)]
		public List<string> MovieClasses { get; set; }
		
		[Section]
	//	[CellStyle(typeof(ClearStyle))]
	//[CellStyle(typeof(ButtonCellStyle))]
		public View WebSamples { get; set; }
		
		private bool _CanChangeStyle;
		[Checkmark]
		public bool CanChangeStyle 
		{ 
			get { return _CanChangeStyle; } 
			set 
			{
				if (_CanChangeStyle != value)
				{
					_CanChangeStyle = value; 
					if (PropertyChanged != null) 
						PropertyChanged(this, new PropertyChangedEventArgs("CanChangeStyle")); 
				}
			}
		}
		
		[Button("CanChangeStyle", CommandOption = CommandOption.Hide)]
		public void ChangeStyle()
		{
			
		}
		
		[Section]
		public InterestingView InterestingStuff { get; set; }
		
		public void RefreshData()
		{
			for(var index = 0; index < 100; index++)
				System.Threading.Thread.Sleep(50);
		}

		public HoneyDoListView HoneyDoList { get; set; }
		[List]
		public Genre Genre { get; set; }

		public MovieListView()
		{
			DataContext = new MovieListViewModel();
			

			HoneyDoList = new HoneyDoListView();

			var enumType = typeof(Genre);
            var enumItems = from field in enumType.GetFields()
                            where field.IsLiteral
                            select EnumExtensions.GetDescriptionValue(field.Name, enumType);

			MovieClasses = new List<string>(enumItems.ToList());

			Movies = new ObservableCollection<MovieViewModel>();
			var dataModel = new MovieDataModel();
			dataModel.Load(5);
			Movies =dataModel.Movies;
			MyString = "Test string";

			InterestingStuff = new InterestingView();
			

			AddressView = new AddressView() {Number = "4751", Street ="Wilshire Blvd", City ="LA", State="CA", Zip ="90010" };

			Movies2 = Movies;
		
			Time = DateTime.Now;
			Date = DateTime.Now;
			
			CanChangeStyle = true;

			TestEntry2 = "Test Entry. This is a sample of some very long text.";
		}
	

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
}
	
	

	public class BackgroundImageTheme: Theme
	{
		[Preserve]
		public BackgroundImageTheme()
		{
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Images/bluewallpaper.jpg"));
		}
	}
}

