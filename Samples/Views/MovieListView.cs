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
	using MonoMobile.MVVM;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;
	
	[Preserve(AllMembers=true)]
	[Theme(typeof(BackgroundImageTheme))]
	[Theme(typeof(NavbarTheme))]
	[Theme(typeof(FrostedTheme))]
	[EnableSearch(true, IncrementalSearch = false)]
	public class MovieListView: View, INotifyPropertyChanged
	{	
//		[Section(Order = 2)]
//		[EnableSearch(true, IncrementalSearch = false)]
//		public List<ISection> Search(ISection[] sections, string searchText)
//		{
//			var result = new List<ISection>() { sections[1] };
//			return result;
//		}

		private View1 _View1 = new View1();
		private View2 _View2 = new View2();
		
		[DefaultValue(true)]
		public bool TestBool { get; set; }

		[Date]
		[Theme(typeof(MarbledTheme))]
		public DateTime Date {get; set;}
		
		[Time]
	//	[Theme(typeof(GraniteStyle))]
		public DateTime Time {get; set;}

		[Entry]
	//	[Theme(typeof(CorkStyle))]
		public string TestEntry2 = "Test Entry";

		[Password("Enter passsword")]
		[DefaultValue("Test String")]
		public string MyString
		{
			get;
			set;
		}

	//	[Section]
//		[Root(ViewType = typeof(MovieView))]
		[List(ViewType = typeof(MovieView), ThemeType = typeof(PaperTheme))]
		public ObservableCollection<MovieViewModel> Movies 
		{
			get;
			set;
		}
		
		[LoadMore]
		public void Activity()
		{
			Thread.Sleep(1500);
		}

		[Section("Using a DataTemplate", "This is a footer")]
		[Root(ViewType = typeof(MovieView))]
		[NavbarButton(UIBarButtonSystemItem.Add)]
		public ObservableCollection<MovieViewModel> Movies2 
		{
		get; set;
		}
//				
		[NavbarButton("Add")]
		public void AddMovie()
		{
			CanChangeStyle = !CanChangeStyle;
			((MovieListViewModel)DataContext).TestEntry2 = "Test Robert";
		}

		[Map("A Map")]
		public CLLocationCoordinate2D Location {get; set;}

			
		[Entry]
		[Bind("TestEntry", "Value")]
		[DefaultValue("Rocket")]
		public string TestEntry 
		{
			get;// { return Get(()=>TestEntry, "Testing"); }
			set;// { Set(()=>TestEntry, value); }
		} 

		[Section]
		[Root(CellStyle = UITableViewCellStyle.Subtitle)]
		[Caption("Root Address")]
		public AddressView AddressView
		{
			get;// { return Get(()=>AddressView, new AddressView() {Number = "4751", Street ="Wilshire Blvd", City ="LA", State="CA", Zip ="90010" }); } 
			set;// { Set(()=>AddressView, value); } 
		}
		
		[Section(Order = 10)]
		[Root]
		public IView DynamicView
		{
			get;// { return Get(()=>DynamicView, new View1()); }
			set;// { Set(()=>DynamicView, value); }
		}
		
		[Section]
	//	[CellStyle(typeof(ClearStyle))]
	//[CellStyle(typeof(ButtonCellStyle))]
		public View WebSamples
		{
			get;// { return Get(()=>WebSamples, new WebSamples()); }
			set;// { Set(()=>WebSamples, value); }
		}
		
		private bool _CanChangeStyle;
		public bool CanChangeStyle 
		{ 
			get { return _CanChangeStyle; } 
			set 
			{
				_CanChangeStyle = value; 
				if (PropertyChanged != null) 
					PropertyChanged(this, new PropertyChangedEventArgs("CanChangeStyle")); 
			}
		}

		[Button]
		[Section(Order = 11)]
		public void ChangeDynamicView()
		{
			if(DynamicView.GetType() == typeof(View1))
				DynamicView = _View2;
			else
				DynamicView = _View1;
		}
		
		[Button("CanChangeStyle", CommandOption = CommandOption.Hide)]
		public void ChangeStyle()
		{
			
		}
//		[Inline]
//		[Section("", "This Address is the same as Root Address above")]
//		public AddressView InlineAddress 
//		{
//			get { return Get(()=>AddressView); } 
//			set { Set(()=>AddressView, value); } 
//		}
		
		[Section]
		[Root]
		public InterestingView InterestingStuff
		{
			get;// { return Get(()=>InterestingStuff, new InterestingView()); }
			set;// { Set(()=>InterestingStuff, value); }
		}
		
		[PullToRefresh]
		public void RefreshData()
		{
			for(var index = 0; index < 100; index++)
				System.Threading.Thread.Sleep(50);
		}

		[Root]
		public ElementAPIView ElementAPIView 
		{
			get; set;
		}
	
		public MovieListView()
		{
			DataContext = new MovieListViewModel();// { TestEntry = "TestEntry string" };
		

			Movies = new ObservableCollection<MovieViewModel>();
			var dataModel = new MovieDataModel();
			dataModel.Load();
			Movies =dataModel.Movies;
			MyString = "Test string";

InterestingStuff = new InterestingView();
			
			Location = new CLLocationCoordinate2D(-33.867139,151.207114);

DynamicView = new View1();
			ElementAPIView = new ElementAPIView();
			AddressView = new AddressView() {Number = "4751", Street ="Wilshire Blvd", City ="LA", State="CA", Zip ="90010" };
			WebSamples = new WebSamples();

////			foreach(var movie in dataModel.Movies)
////			{
////				Movies.Add(new MovieView() { DataContext = movie });
////			}
//
Movies2 = Movies;
		
			Time = DateTime.Now;
			Date = DateTime.Now;
			
			CanChangeStyle = true;
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
			BackgroundUri = new Uri("file://" + Path.GetFullPath("Images/bluesky.jpg"));
		}
	}
}

