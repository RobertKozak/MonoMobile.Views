namespace Samples
{
	using MonoMobile.MVVM;
	using MonoTouch.UIKit;
	
	public class MovieView: View
	{
		[Section()]
		[Entry]
		public string Name 
		{ 
			get;// { return Get(()=>Name); } 
			set;// { Set(()=>Name, value); }
		}

		//[PopOnSelection]
		public Genre Genre 
		{
			get;// { return Get(()=>DataContext.Genre); } 
			set;// { Set(()=>DataContext.Genre, value); }
		}
		
		[PopOnSelection]
		public Rating Rating
		{
			get;// { return Get(()=>DataContext.Rating); } 
			set;// { Set(()=>DataContext.Rating, value); }
		}
		
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public float TicketPrice 
		{
			get;// { return Get (() => DataContext.TicketPrice); }
			set;// { Set (() => DataContext.TicketPrice, value); }
		}
		
		[Bind(ValueConverterType = typeof(PercentConverter))]
		public float CriticsRating 
		{
			get;// { return Get (() => DataContext.CriticsRating); }
			set;// { Set (() => DataContext.CriticsRating, value); }
		}
		
		[Caption("Shown in 3D?")]
		public bool ShownIn3D
		{
			get;// { return Get(()=>DataContext.ShownIn3D); }
			set;// { Set(()=>DataContext.ShownIn3D, value); }
		}
		
		public AddressView AddressView { get; set; }
		
//		public EnumCollection<Location> Location 
//		{
//			get;// { return Get(() => DataContext.Location); }
//			set;// { Set(() => DataContext.Location, value); }
//		}

		[Section(Order = 1)]
		[Button]
		public void MakeMovieRestricted()
		{
			Rating = Rating.R;
		}

		[Button]
		[Caption("Toggle 3D")]
		public void Toggle3D()
		{
			((MovieViewModel)DataContext).ShownIn3D = !((MovieViewModel)DataContext).ShownIn3D;
		}
		
		[ToolbarButton(UIBarButtonSystemItem.Add)]
		public void Add()
		{
			var color = ((MovieViewModel)DataContext).TextColor;
			if (UIColor.Equals(color, UIColor.Red))
				color = UIColor.Green;
			else
				color = UIColor.Red;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}