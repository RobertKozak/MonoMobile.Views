using MonoTouch.UIKit;
namespace Samples
{
	using System;
	using MonoMobile.MVVM;
	using System.ComponentModel;
	
	public enum Rating
	{
		NC17,
		R,
		[Description("PG-13")]
		PG13,
		PG,
		G
	}

	public enum Genre
	{
		[Description("Romantic Comedy")]
		RomanticComedy,
		Romance,
		[Description("Action & Adventure")]
		ActionAdventure,
		Scifi,
		Drama,
		Horror,
		Comedy,
		Animated,
		Childrens
	}
	
	public enum Location
	{
		[Description("East Coast")]
		EastCoast,
		Midwest,
		South,
		[Description("West Coast")]
		WestCoast
	}
	
	public class MovieViewModel : ViewModel
	{
		public string Name 
		{
			get { return Get(() => Name); }
			set { Set(() => Name, value); }
		}
		
		public UIColor TextColor
		{
			get { return Get(()=>TextColor); }
			set { Set(()=>TextColor, value); }
		}

		public Rating Rating 
		{
			get { return Get(() => Rating, Rating.PG); }
			set { Set(() => Rating, value); }
		}

		public Genre Genre 
		{
			get { return Get(() => Genre, Genre.Comedy); }
			set { Set(() => Genre, value); }
		}

		public float TicketPrice
		{
			get { return Get(()=>TicketPrice); }
			set { Set(()=>TicketPrice, value); }
		}

		public float CriticsRating 
		{
			get { return Get(() => CriticsRating); }
			set { Set(() => CriticsRating, value); }
		}

		public bool ShownIn3D
		{
			get { return Get(()=>ShownIn3D); }
			set { Set(()=>ShownIn3D, value); }
		}

		public EnumCollection<Location> Location
		{
			get { return Get(() => Location); }
			set { Set(() => Location, value); }
		}

		public override string ToString()
		{
			return string.Format("{0}", Name);
		}
	

		public override void BeginInit()
		{
			TextColor = UIColor.Red;
		}

		public override void EndInit()
		{

		}

}
}

