namespace Samples
{
	using MonoMobile.Views;
	using System;
	using System.Collections.ObjectModel;

	public class MovieDataModel
	{
		public ObservableCollection<MovieViewModel> Movies { get; set; }

		public void Load(int numberToLoad)
		{
			Movies = new ObservableCollection<MovieViewModel>();

			Movies.Add(new MovieViewModel() { Name = "Forrest Gump", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 89f});
			Movies.Add(new MovieViewModel() { Name = "Tangled", Genre = Genre.Animated, Rating = Rating.G, TicketPrice = 12f, CriticsRating = 91f });
			Movies.Add(new MovieViewModel() { Name = "The Matrix", Genre = Genre.Scifi, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 82f });
			Movies.Add(new MovieViewModel() { Name = "Star Wars IV : A New Hope", Genre = Genre.ActionAdventure, Rating = Rating.PG, TicketPrice = 10f, CriticsRating = 78f });
			Movies.Add(new MovieViewModel() { Name = "Die Hard", Genre = Genre.ActionAdventure, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 84f });
			Movies.Add(new MovieViewModel() { Name = "Toy Story 3", Genre = Genre.Animated, Rating = Rating.PG, TicketPrice = 16f, CriticsRating = 98f });
			Movies.Add(new MovieViewModel() { Name = "True Grit", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 97f });
			Movies.Add(new MovieViewModel() { Name = "Black Swan", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 88f });
			Movies.Add(new MovieViewModel() { Name = "Little Fockers", Genre = Genre.Comedy, Rating = Rating.PG13, TicketPrice = 12f, CriticsRating = 10f });
			Movies.Add(new MovieViewModel() { Name = "The Green Hornet 3D", Genre = Genre.ActionAdventure, Rating = Rating.PG13, TicketPrice = 12f, CriticsRating = 44f, ShownIn3D = true });
			Movies.Add(new MovieViewModel() { Name = "The Fighter", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 89f });
		
			for(int index = 0; index < numberToLoad; index++)
			{

				Movies.Add(new MovieViewModel() { Name = "Forrest Gump", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 89f});
				Movies.Add(new MovieViewModel() { Name = "Tangled", Genre = Genre.Animated, Rating = Rating.G, TicketPrice = 12f, CriticsRating = 91f });
				Movies.Add(new MovieViewModel() { Name = "The Matrix", Genre = Genre.Scifi, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 82f });
				Movies.Add(new MovieViewModel() { Name = "Star Wars IV : A New Hope", Genre = Genre.ActionAdventure, Rating = Rating.PG, TicketPrice = 10f, CriticsRating = 78f });
				Movies.Add(new MovieViewModel() { Name = "Die Hard", Genre = Genre.ActionAdventure, Rating = Rating.R, TicketPrice = 10f, CriticsRating = 84f });
				Movies.Add(new MovieViewModel() { Name = "Toy Story 3", Genre = Genre.Animated, Rating = Rating.PG, TicketPrice = 16f, CriticsRating = 98f });
				Movies.Add(new MovieViewModel() { Name = "True Grit", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 97f });
				Movies.Add(new MovieViewModel() { Name = "Black Swan", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 88f });
				Movies.Add(new MovieViewModel() { Name = "Little Fockers", Genre = Genre.Comedy, Rating = Rating.PG13, TicketPrice = 12f, CriticsRating = 10f });
				Movies.Add(new MovieViewModel() { Name = "The Green Hornet 3D", Genre = Genre.ActionAdventure, Rating = Rating.PG13, TicketPrice = 12f, CriticsRating = 44f, ShownIn3D = true });
				Movies.Add(new MovieViewModel() { Name = "The Fighter", Genre = Genre.Drama, Rating = Rating.R, TicketPrice = 12f, CriticsRating = 89f });
			}
		}
	}
}

