using System;
using System.Collections.Generic;
using MonoMobile.Views;
using System.Collections.ObjectModel;
using MonoTouch.Foundation;

namespace Samples
{
	[Preserve(AllMembers = true)]
	public class ThemeSampleView: View
	{
		public Theme Selected { get; set; }
		
		[List(SelectionAction = SelectionAction.Multiselection, SelectedItemMemberName = "Selected")]
		public ObservableCollection<Theme> Themes { get; set; }	

		public ThemeSampleView()
		{
			DataContext = new ThemeSampleViewModel();
		}
	}
}

