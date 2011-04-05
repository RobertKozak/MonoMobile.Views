using System;
using MonoMobile.MVVM;
namespace Samples
{
	public class MovieListViewModel: ViewModel
	{
		public MovieListViewModel()
		{
			LoginView = new LoginView();
		}
		
		public LoginView LoginView { get { return Get(()=>LoginView); } set { Set(()=>LoginView, value); } }

		public string TestEntry { get; set;}
		public string TestEntry2 { get { return Get(()=>TestEntry2); } set { Set(()=>TestEntry2, value); } }
	}
}

