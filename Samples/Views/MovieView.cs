namespace Samples
{
	using MonoTouch.Dialog;
	using MonoTouch.MVVM;
	using MonoTouch.UIKit;

	public class MovieView: View<MovieViewModel>
	{
		[Section]
		[Entry]
		public string Name 
		{ 
			get { return Get(()=>DataContext.Name); } 
			set { Set(()=>DataContext.Name, value); }
		}

		[PopOnSelection]
		public Genre Genre 
		{
			get { return Get(()=>DataContext.Genre); } 
			set { Set(()=>DataContext.Genre, value); }
		}
		
		[PopOnSelection]
		public Rating Rating
		{
			get { return Get(()=>DataContext.Rating); } 
			set { Set(()=>DataContext.Rating, value); }
		}
		
		[Bind(ValueConverterType = typeof(MoneyConverter))]
		public float TicketPrice 
		{
			get { return Get (() => DataContext.TicketPrice); }
			set { Set (() => DataContext.TicketPrice, value); }
		}
		
		[Bind(ValueConverterType = typeof(PercentConverter))]
		public float CriticsRating 
		{
			get { return Get (() => DataContext.CriticsRating); }
			set { Set (() => DataContext.CriticsRating, value); }
		}
		
		[Caption("Shown in 3D?")]
		public bool ShownIn3D
		{
			get { return Get(()=>DataContext.ShownIn3D); }
			set { Set(()=>DataContext.ShownIn3D, value); }
		}
		
		public EnumCollection<Location> Location 
		{
			get { return Get(() => DataContext.Location); }
			set { Set(() => DataContext.Location, value); }
		}

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
			ShownIn3D = !ShownIn3D;
		}

		[ToolbarButton(UIBarButtonSystemItem.Add)]
		public void Add()
		{
			ShownIn3D = !ShownIn3D;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}

